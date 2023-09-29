using System;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace DarkBestiary.UI.Elements
{
    public class InventoryItem : PoolableMonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerClickHandler,
        IPointerEnterHandler,
        IPointerExitHandler,
        ITickable
    {
        public static event Action<InventoryItem> AnyBeginDrag;
        public static event Action<InventoryItem> AnyEndDrag;

        public event Action<ItemDroppedEventData> Dropped;
        public event Action<InventoryItem> Blocked;
        public event Action<InventoryItem> Unblocked;
        public event Action<InventoryItem> DoubleClicked;
        public event Action<InventoryItem> ControlClicked;
        public event Action<InventoryItem> RightClicked;
        public event Action<InventoryItem> Clicked;
        public event Action<InventoryItem> BeginDrag;
        public event Action<InventoryItem> EndDrag;

        public Item Item { get; private set; } = Item.CreateEmpty();
        public bool ShowTooltip { get; set; } = true;
        public bool IsDraggable { get; set; } = true;
        public bool IsBlocked { get; private set; }

        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private TextMeshProUGUI m_StackCountText;

        [SerializeField]
        private Image m_Fade;

        [SerializeField]
        private Image m_RarityBorder;

        [SerializeField]
        private Image m_CooldownImage;

        [SerializeField]
        private TextMeshProUGUI m_CooldownText;

        [SerializeField]
        private TextMeshProUGUI m_SharpeningText;

        [SerializeField]
        private GameObject m_Outline;

        private RectTransform m_RectTransform;
        private RectTransform m_ParentTransform;
        private Transform m_DragTransform;
        private float m_LastClickTime;
        private bool m_IsDragging;
        private TickableManager m_TickableManager;

        private void Start()
        {
            m_RectTransform = GetComponent<RectTransform>();
            m_ParentTransform = transform.parent.GetComponent<RectTransform>();
            m_DragTransform = UIManager.Instance.ViewCanvas.transform;

            Timer.Instance.WaitForFixedUpdate(FitParent);

            OnSpawn();
        }

        private void OnDestroy()
        {
            CleanUp();
            OnDespawn();
        }

        protected override void OnSpawn()
        {
            if (m_TickableManager != null)
            {
                return;
            }

            m_TickableManager = Container.Instance.Resolve<TickableManager>();
            m_TickableManager.Add(this);
        }

        protected override void OnDespawn()
        {
            if (m_TickableManager == null)
            {
                return;
            }

            m_TickableManager.Remove(this);
            m_TickableManager = null;
        }

        public void Tick()
        {
            // Note: Fuck you Unity
            // https://stackoverflow.com/questions/41537243/c-sharp-unity-ienddraghandler-onenddrag-not-always-called

            if (m_IsDragging && !Input.GetMouseButton(0))
            {
                OnEndDrag(null);
            }
        }

        public void Change(Item item, bool subscribeToEvents = true)
        {
            CleanUp();

            Item = item;

            if (subscribeToEvents)
            {
                SubscribeToItemEvents(Item);
            }

            name = $"Item - {Item.Name} #{Item.GetHashCode()}";

            m_Icon.color = new Color(1, 1, 1, 1);
            m_Icon.sprite = Resources.Load<Sprite>(Item.Icon);
            m_Icon.enabled = !Item.IsEmpty;

            UpdateStackCount();
            UpdateRarityBorderColor();

            if (Item.IsOnCooldown)
            {
                UpdateCooldown();
            }

            ItemTooltip.Instance.Hide();
        }

        private void SubscribeToItemEvents(Item item)
        {
            item.StackCountChanged += UpdateStackCount;
            item.SocketInserted += OnSocketInserted;
            item.Enchanted += OnEnchanted;
            item.CooldownStarted += OnCooldownUpdated;
            item.CooldownUpdated += OnCooldownUpdated;
            item.CooldownFinished += OnCooldownFinished;
        }

        private void UnsubscribeFromItemEvents(Item item)
        {
            item.StackCountChanged -= UpdateStackCount;
            item.SocketInserted -= OnSocketInserted;
            item.Enchanted -= OnEnchanted;
            item.CooldownStarted -= OnCooldownUpdated;
            item.CooldownUpdated -= OnCooldownUpdated;
            item.CooldownFinished -= OnCooldownFinished;
        }

        private void CleanUp()
        {
            Unblock();
            ClearCooldown();
            Deselect();

            if (Item == null)
            {
                return;
            }

            UnsubscribeFromItemEvents(Item);
        }

        public void Select()
        {
            m_Outline.gameObject?.SetActive(true);
        }

        public void Deselect()
        {
            m_Outline.gameObject?.SetActive(false);
        }

        private void OnSocketInserted(Item item, Item socket)
        {
            AudioManager.Instance.PlayCraftSocket();
            Instantiate(UIManager.Instance.FlareParticle, transform).DestroyAsVisualEffect();
        }

        private void OnEnchanted(Item item, Item enchant)
        {
            AudioManager.Instance.PlayEnchant();
            Instantiate(UIManager.Instance.SparksParticle, transform).DestroyAsVisualEffect();
        }

        private void OnCooldownUpdated(Item item)
        {
            try
            {
                UpdateCooldown();
            }
            catch (Exception exception)
            {
                // ignored
            }
        }

        private void OnCooldownFinished(Item item)
        {
            try
            {
                ClearCooldown();
            }
            catch (Exception exception)
            {
                // ignored
            }
        }

        private void ClearCooldown()
        {
            m_CooldownImage.fillAmount = 0;
            m_CooldownText.text = "";
        }

        private void UpdateCooldown()
        {
            m_CooldownImage.fillAmount = 1.0f - (float) (Item.Cooldown - Item.CooldownRemaining) / Item.Cooldown;
            ;
            m_CooldownText.text = Item.CooldownRemaining.ToString();
        }

        private void UpdateRarityBorderColor()
        {
            if (Item.Rarity == null ||
                Item.Rarity.Type == RarityType.Common ||
                Item.Rarity.Type == RarityType.Junk)
            {
                m_RarityBorder.color = m_RarityBorder.color.With(a: 0);
                return;
            }

            m_RarityBorder.color = Item.Rarity.Color().With(a: 0.5f);
        }

        public void UpdateStackCount()
        {
            m_StackCountText.text = Item.StackCount > 1 ? Item.StackCount.ToString() : "";
        }

        public void OverwriteStackCount(int stack)
        {
            m_StackCountText.text = stack > 1 ? stack.ToString() : "";
        }

        public void Block()
        {
            IsBlocked = true;
            m_Fade.color = m_Fade.color.With(a: 0.75f);

            Blocked?.Invoke(this);
        }

        public void Unblock()
        {
            IsBlocked = false;
            m_Fade.color = m_Fade.color.With(a: 0);

            Unblocked?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            if (Item.IsEmpty || IsBlocked)
            {
                return;
            }

            if (pointer.button == PointerEventData.InputButton.Right)
            {
                RightClicked?.Invoke(this);
                return;
            }

            if (pointer.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) ||
                Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand))
            {
                ControlClicked?.Invoke(this);
                return;
            }

            Clicked?.Invoke(this);

            if (Time.time - m_LastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
            }

            m_LastClickTime = Time.time;
        }

        private void FitParent()
        {
            m_RectTransform.localScale = Vector3.one;
            m_RectTransform.anchoredPosition = Vector3.zero;
            m_RectTransform.sizeDelta = new Vector2(m_ParentTransform.rect.width, m_ParentTransform.rect.height);
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            if (Item.IsEmpty || IsBlocked || !IsDraggable || pointer.button == PointerEventData.InputButton.Right ||
                Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
            {
                pointer.pointerDrag = null;
                return;
            }

            m_IsDragging = true;

            ItemTooltip.Instance.Hide();

            m_CanvasGroup.blocksRaycasts = false;
            m_CanvasGroup.interactable = false;

            m_RectTransform.SetParent(m_DragTransform);
            m_RectTransform.sizeDelta = new Vector2(64, 64);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
            AudioManager.Instance.PlayItemPick(Item);

            BeginDrag?.Invoke(this);
            AnyBeginDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData pointer)
        {
            transform.position = pointer.position + new Vector2(16, -16);
        }

        public void OnEndDrag(PointerEventData pointer)
        {
            if (!m_IsDragging)
            {
                return;
            }

            m_IsDragging = false;

            transform.SetParent(m_ParentTransform);

            m_CanvasGroup.blocksRaycasts = true;
            m_CanvasGroup.interactable = true;

            FitParent();

            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

            EndDrag?.Invoke(this);
            AnyEndDrag?.Invoke(this);
        }

        public void OnDrop(ItemDroppedEventData data)
        {
            AudioManager.Instance.PlayItemPlace(Item);
            Dropped?.Invoke(data);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (Item.IsEmpty || pointer.pointerDrag != null || !ShowTooltip)
            {
                return;
            }

            ItemTooltip.Instance.Show(Item, m_RectTransform);
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            if (Item.IsEmpty || pointer.pointerDrag != null || !ShowTooltip)
            {
                return;
            }

            ItemTooltip.Instance.Hide();
        }
    }
}