using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class InventoryItem : PoolableMonoBehaviour,
        IBeginDragHandler,
        IEndDragHandler,
        IDragHandler,
        IPointerUpHandler,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public static event Payload<InventoryItem> AnyBeginDrag;
        public static event Payload<InventoryItem> AnyEndDrag;

        public event Payload<ItemDroppedEventData> Dropped;
        public event Payload<InventoryItem> Blocked;
        public event Payload<InventoryItem> Unblocked;
        public event Payload<InventoryItem> DoubleClicked;
        public event Payload<InventoryItem> RightClicked;
        public event Payload<InventoryItem> Clicked;
        public event Payload<InventoryItem> BeginDrag;
        public event Payload<InventoryItem> EndDrag;

        public Item Item { get; private set; }
        public bool ShowTooltip { get; set; } = true;
        public bool IsDraggable { get; set; } = true;
        public bool IsBlocked { get; private set; }

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stackCountText;
        [SerializeField] private Image fade;
        [SerializeField] private Image unidentifiedHighlight;
        [SerializeField] private Image rarityBorder;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private TextMeshProUGUI cooldownText;

        private RectTransform rectTransform;
        private RectTransform parentTransform;
        private Transform dragTransform;
        private float lastClickTime;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentTransform = transform.parent.GetComponent<RectTransform>();
            this.dragTransform = UIManager.Instance.ViewCanvas.transform;

            Timer.Instance.WaitForFixedUpdate(FitParent);
        }

        private void OnDestroy()
        {
            if (Item == null)
            {
                return;
            }

            CleanUp();
        }

        public void Change(Item item)
        {
            CleanUp();

            Item = item;
            Item.StackCountChanged += UpdateStackCount;
            Item.SocketInserted += OnSocketInserted;
            Item.Forged += OnForged;
            Item.CooldownStarted += OnCooldownUpdated;
            Item.CooldownUpdated += OnCooldownUpdated;
            Item.CooldownFinished += OnCooldownFinished;

            name = $"Item - {Item.Name} #{Item.GetHashCode()}";

            this.icon.color = new Color(1, 1, 1, 1);
            this.icon.sprite = Resources.Load<Sprite>(Item.Icon);
            this.icon.enabled = !Item.IsEmpty;

            UpdateStackCount();
            UpdateUnidentifiedStatus();
            UpdateRarityBorderColor();

            if (Item.IsOnCooldown)
            {
                UpdateCooldown();
            }

            ItemTooltip.Instance.Hide();
        }

        private void OnForged(Item item)
        {
            AudioManager.Instance.PlayCraftForge();
            Instantiate(UIManager.Instance.SparksParticle, transform).DestroyAsVisualEffect();
        }

        private void OnSocketInserted(Item item, Item socket)
        {
            AudioManager.Instance.PlayCraftSocket();
            Instantiate(UIManager.Instance.SparksParticle, transform).DestroyAsVisualEffect();
        }

        private void CleanUp()
        {
            Unblock();
            ClearCooldown();

            if (Item == null)
            {
                return;
            }

            Item.StackCountChanged -= UpdateStackCount;
            Item.SocketInserted -= OnSocketInserted;
            Item.Forged -= OnForged;
            Item.CooldownStarted -= OnCooldownUpdated;
            Item.CooldownUpdated -= OnCooldownUpdated;
            Item.CooldownFinished -= OnCooldownFinished;
        }

        private void OnCooldownUpdated(Item item)
        {
            UpdateCooldown();
        }

        private void OnCooldownFinished(Item item)
        {
            ClearCooldown();
        }

        private void ClearCooldown()
        {
            this.cooldownImage.fillAmount = 0;
            this.cooldownText.text = "";
        }

        private void UpdateCooldown()
        {
            this.cooldownImage.fillAmount = 1.0f - (float) (Item.Cooldown - Item.CooldownRemaining) / Item.Cooldown;;
            this.cooldownText.text = Item.CooldownRemaining.ToString();
        }

        private void UpdateRarityBorderColor()
        {
            if (Item.Rarity == null ||
                Item.Rarity.Type == RarityType.Common ||
                Item.Rarity.Type == RarityType.Junk)
            {
                this.rarityBorder.color = this.rarityBorder.color.With(a: 0);
                return;
            }

            this.rarityBorder.color = Item.Rarity.Color().With(a: 0.5f);
        }

        private void UpdateUnidentifiedStatus()
        {
            this.unidentifiedHighlight.color = this.unidentifiedHighlight.color.With(a: Item.IsIdentified ? 0 : 1);
        }

        public void UpdateStackCount()
        {
            this.stackCountText.text = Item.StackCount > 1 ? Item.StackCount.ToString() : "";
        }

        public void OverwriteStackCount(int stack)
        {
            this.stackCountText.text = stack > 1 ? stack.ToString() : "";
        }

        public void Block()
        {
            IsBlocked = true;
            this.fade.color = this.fade.color.With(a: 0.75f);

            Blocked?.Invoke(this);
        }

        public void Unblock()
        {
            IsBlocked = false;
            this.fade.color = this.fade.color.With(a: 0);

            Unblocked?.Invoke(this);
        }

        public void OnPointerUp(PointerEventData pointer)
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

            Clicked?.Invoke(this);

            if (Time.time - this.lastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
            }

            this.lastClickTime = Time.time;
        }

        private void FitParent()
        {
            if (this.parentTransform == null)
            {
                return;
            }

            this.rectTransform.sizeDelta =
                new Vector2(this.parentTransform.rect.width, this.parentTransform.rect.height);
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            if (Item.IsEmpty || IsBlocked || !IsDraggable || pointer.button == PointerEventData.InputButton.Right)
            {
                pointer.pointerDrag = null;
                return;
            }

            ItemTooltip.Instance.Hide();

            this.canvasGroup.blocksRaycasts = false;
            this.canvasGroup.interactable = false;

            this.rectTransform.SetParent(this.dragTransform);
            this.rectTransform.sizeDelta = new Vector2(64, 64);

            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);
            AudioManager.Instance.PlayItemPick(Item);

            BeginDrag?.Invoke(this);
            AnyBeginDrag?.Invoke(this);
        }

        public void OnDrag(PointerEventData pointer)
        {
            transform.position = pointer.position + new Vector2(32, -32);
        }

        public void OnEndDrag(PointerEventData pointer)
        {
            transform.SetParent(this.parentTransform);
            transform.position = this.parentTransform.position;
            transform.localScale = Vector3.one;

            this.canvasGroup.blocksRaycasts = true;
            this.canvasGroup.interactable = true;

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

            ItemTooltip.Instance.Show(Item, this.rectTransform);
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