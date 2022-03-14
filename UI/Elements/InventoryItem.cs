using System;
using DarkBestiary.Extensions;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
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
        public static event Payload<InventoryItem> AnyBeginDrag;
        public static event Payload<InventoryItem> AnyEndDrag;

        public event Payload<ItemDroppedEventData> Dropped;
        public event Payload<InventoryItem> Blocked;
        public event Payload<InventoryItem> Unblocked;
        public event Payload<InventoryItem> DoubleClicked;
        public event Payload<InventoryItem> ControlClicked;
        public event Payload<InventoryItem> RightClicked;
        public event Payload<InventoryItem> Clicked;
        public event Payload<InventoryItem> BeginDrag;
        public event Payload<InventoryItem> EndDrag;

        public Item Item { get; private set; } = Item.CreateEmpty();
        public bool ShowTooltip { get; set; } = true;
        public bool IsDraggable { get; set; } = true;
        public bool IsBlocked { get; private set; }

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI stackCountText;
        [SerializeField] private Image fade;
        [SerializeField] private Image rarityBorder;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private TextMeshProUGUI sharpeningText;
        [SerializeField] private GameObject illusoryParticles;
        [SerializeField] private GameObject outline;

        private RectTransform rectTransform;
        private RectTransform parentTransform;
        private Transform dragTransform;
        private float lastClickTime;
        private bool isDragging;
        private TickableManager tickableManager;

        private void Start()
        {
            this.rectTransform = GetComponent<RectTransform>();
            this.parentTransform = transform.parent.GetComponent<RectTransform>();
            this.dragTransform = UIManager.Instance.ViewCanvas.transform;

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
            if (this.tickableManager != null)
            {
                return;
            }

            this.tickableManager = Container.Instance.Resolve<TickableManager>();
            this.tickableManager.Add(this);
        }

        protected override void OnDespawn()
        {
            if (this.tickableManager == null)
            {
                return;
            }

            this.tickableManager.Remove(this);
            this.tickableManager = null;
        }

        public void Tick()
        {
            // Note: Fuck you Unity
            // https://stackoverflow.com/questions/41537243/c-sharp-unity-ienddraghandler-onenddrag-not-always-called

            if (this.isDragging && !Input.GetMouseButton(0))
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

            this.icon.color = new Color(1, 1, 1, 1);
            this.icon.sprite = Resources.Load<Sprite>(Item.Icon);
            this.icon.enabled = !Item.IsEmpty;

            if (this.illusoryParticles != null)
            {
                this.illusoryParticles.SetActive(Item.IsMarkedAsIllusory && Game.Instance.IsVisions);
            }

            UpdateStackCount();
            UpdateRarityBorderColor();

            if (Item.IsOnCooldown)
            {
                UpdateCooldown();
            }

            UpdateSharpening();

            ItemTooltip.Instance.Hide();
        }

        private void SubscribeToItemEvents(Item item)
        {
            item.StackCountChanged += UpdateStackCount;
            item.SocketInserted += OnSocketInserted;
            item.Enchanted += OnEnchanted;
            item.Forged += OnForged;
            item.SharpeningSuccess += OnSharpeningSuccess;
            item.SharpeningFailed += OnSharpeningFailed;
            item.SharpeningLevelChanged += OnSharpeningLevelChanged;
            item.CooldownStarted += OnCooldownUpdated;
            item.CooldownUpdated += OnCooldownUpdated;
            item.CooldownFinished += OnCooldownFinished;
        }

        private void UnsubscribeFromItemEvents(Item item)
        {
            item.StackCountChanged -= UpdateStackCount;
            item.SocketInserted -= OnSocketInserted;
            item.Enchanted -= OnEnchanted;
            item.Forged -= OnForged;
            item.SharpeningSuccess -= OnSharpeningSuccess;
            item.SharpeningFailed -= OnSharpeningFailed;
            item.SharpeningLevelChanged -= OnSharpeningLevelChanged;
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
            this.outline.gameObject?.SetActive(true);
        }

        public void Deselect()
        {
            this.outline.gameObject?.SetActive(false);
        }

        private void OnSharpeningLevelChanged(Item item)
        {
            UpdateSharpening();
        }

        private void OnSharpeningSuccess(Item item)
        {
            AudioManager.Instance.PlayCraftSharpenSuccess();
            Instantiate(UIManager.Instance.SparksParticle, transform).DestroyAsVisualEffect();
        }

        private void OnSharpeningFailed(Item item)
        {
            AudioManager.Instance.PlayCraftSharpenFailed();
            Instantiate(UIManager.Instance.PuffParticle, transform).DestroyAsVisualEffect();
        }

        private void OnForged(Item item)
        {
            AudioManager.Instance.PlayCraftForge();
            Instantiate(UIManager.Instance.SparksParticle, transform).DestroyAsVisualEffect();
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

        private void UpdateSharpening()
        {
            if (this.sharpeningText == null)
            {
                return;
            }

            this.sharpeningText.gameObject.SetActive(Item.SharpeningLevel > 0);
            this.sharpeningText.text = "+" + Item.SharpeningLevel;
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

            if (Time.time - this.lastClickTime < 0.25f)
            {
                DoubleClicked?.Invoke(this);
            }

            this.lastClickTime = Time.time;
        }

        private void FitParent()
        {
            this.rectTransform.localScale = Vector3.one;
            this.rectTransform.anchoredPosition = Vector3.zero;
            this.rectTransform.sizeDelta = new Vector2(this.parentTransform.rect.width, this.parentTransform.rect.height);
        }

        public void OnBeginDrag(PointerEventData pointer)
        {
            if (Item.IsEmpty || IsBlocked || !IsDraggable || pointer.button == PointerEventData.InputButton.Right ||
                Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand))
            {
                pointer.pointerDrag = null;
                return;
            }

            this.isDragging = true;

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
            transform.position = pointer.position + new Vector2(16, -16);
        }

        public void OnEndDrag(PointerEventData pointer)
        {
            if (!this.isDragging)
            {
                return;
            }

            this.isDragging = false;

            transform.SetParent(this.parentTransform);

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