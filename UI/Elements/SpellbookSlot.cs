using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Interaction;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class SpellbookSlot : PoolableMonoBehaviour, IDropHandler, IDragHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public event Payload<Skill, Skill> Selected;
        public event Payload<SpellbookSlot> PointerUp;
        public event Payload<SpellbookSlot> PointerEnter;
        public event Payload<SpellbookSlot> PointerExit;
        public event Payload<Skill> SkillDroppedOut;
        public event Payload<SkillSlot, Skill> SkillDroppedIn;

        public SkillSlot Slot { get; private set; }
        public bool ShowTooltip { get; set; } = true;

        [SerializeField] private Image icon;
        [SerializeField] private Image fade;
        [SerializeField] private Image red;
        [SerializeField] private SpellbookDraggableSkill draggableSkill;
        [SerializeField] private TextMeshProUGUI cooldownText;
        [SerializeField] private Image cooldownImage;
        [SerializeField] private Image highlight;
        [SerializeField] private Image weaponBorder;
        [SerializeField] private Image ultimateBorder;
        [SerializeField] private GameObject hotkeyBadge;
        [SerializeField] private TextMeshProUGUI hotkeyText;
        [SerializeField] private SkillMenu skillMenu;
        [SerializeField] private Animator animator;

        private KeyCode hotkey = KeyCode.None;

        public void Initialize(SkillSlot slot)
        {
            Slot = slot;
            Slot.SkillChanged += OnSkillChanged;

            this.skillMenu.Selected += OnSkillSelected;
            this.draggableSkill.DroppedOut += OnSkillDroppedOut;

            SpellbookComponent.AnySkillSetBonusApplied += OnAnySkillSetBonusApplied;
            ResourcesComponent.AnyResourceChanged += OnAnyResourceChanged;
            BehavioursComponent.AnyBehaviourApplied += OnAnyBehaviourAppliedOrRemoved;
            BehavioursComponent.AnyBehaviourRemoved += OnAnyBehaviourAppliedOrRemoved;

            var interactor = Container.Instance.Resolve<Interactor>();
            interactor.StateEnter += OnStateEnter;
            interactor.StateExit += OnStateExit;

            OnSkillChanged(Slot);
        }

        public void Terminate()
        {
            Slot.SkillChanged -= OnSkillChanged;

            this.skillMenu.Selected -= OnSkillSelected;
            this.draggableSkill.DroppedOut -= OnSkillDroppedOut;

            var interactor = Container.Instance.Resolve<Interactor>();
            interactor.StateEnter -= OnStateEnter;
            interactor.StateExit -= OnStateExit;

            SpellbookComponent.AnySkillSetBonusApplied -= OnAnySkillSetBonusApplied;
            ResourcesComponent.AnyResourceChanged -= OnAnyResourceChanged;
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourAppliedOrRemoved;
            BehavioursComponent.AnyBehaviourRemoved -= OnAnyBehaviourAppliedOrRemoved;
        }

        protected override void OnDespawn()
        {
            Terminate();
        }

        private void OnStateEnter(InteractorState state)
        {
            if (!(state is CastState castState))
            {
                return;
            }

            if (!castState.Skill.Equals(Slot.Skill))
            {
                return;
            }

            this.highlight.color = this.highlight.color.With(a: 1);
        }

        private void OnStateExit(InteractorState state)
        {
            this.highlight.color = this.highlight.color.With(a: 0);
        }

        private void OnAnySkillSetBonusApplied(Skill skill, SkillSet set)
        {
            if (Slot.Skill.IsEmpty())
            {
                return;
            }

            if (Slot.Skill.Sets.Any(s => s.Id == set.Id))
            {
                Instantiate(UIManager.Instance.FlareParticle, transform).DestroyAsVisualEffect();
            }
        }

        private void OnAnyResourceChanged(Resource resource)
        {
            if (Slot.Skill.Caster != resource.Owner)
            {
                return;
            }

            MaybeMarkAsUnavailable();
        }

        private void OnAnyBehaviourAppliedOrRemoved(Behaviour behaviour)
        {
            if (Slot.Skill.Caster != behaviour.Target)
            {
                return;
            }

            MaybeMarkAsUnavailable();
        }

        private void MaybeMarkAsUnavailable()
        {
            var isUnavailable = !Slot.Skill.HasEnoughResources() || Slot.Skill.IsDisabled();
            this.red.color = this.red.color.With(a: isUnavailable ? 0.3f : 0);
        }

        private void OnSkillSelected(Skill skill)
        {
            Selected?.Invoke(Slot.Skill, skill);
        }

        private void OnSkillDroppedOut(SpellbookDraggableSkill draggable)
        {
            SkillDroppedOut?.Invoke(draggable.Skill);
        }

        public void SetHotkey(KeyCode hotkey)
        {
            this.hotkey = hotkey;

            this.hotkeyBadge.gameObject.SetActive(true);
            this.hotkeyText.text = KeyCodes.GetLabel(this.hotkey);
        }

        public void HideHotkey()
        {
            this.hotkeyBadge.gameObject.SetActive(false);
            this.hotkeyText.text = "";
        }

        public void DisableDrag()
        {
            this.draggableSkill.gameObject.SetActive(false);
        }

        public void HideCooldown()
        {
            this.cooldownText.gameObject.SetActive(false);
            this.cooldownImage.gameObject.SetActive(false);
        }

        public void ShowCooldown()
        {
            this.cooldownText.gameObject.SetActive(true);
            this.cooldownImage.gameObject.SetActive(true);
        }

        public void SetCooldown(float remaining, float total)
        {
            this.cooldownImage.fillAmount = 1.0f - (total - remaining) / total;
            this.cooldownText.text = ((int) remaining).ToString();
        }

        public void PlayCooldownPulse()
        {
            this.animator.Play("cooldown_pulse");
        }

        private void OnSkillChanged(SkillSlot slot)
        {
            this.icon.sprite = Resources.Load<Sprite>(Slot.Skill.Icon);
            this.icon.enabled = !Slot.Skill.IsEmpty();
            this.weaponBorder.gameObject.SetActive(Slot.SkillType == SkillType.Weapon);
            this.ultimateBorder.gameObject.SetActive(Slot.Skill.Rarity?.Type == RarityType.Legendary);

            this.draggableSkill.Change(slot.Skill);

            if (slot.Skill.GetSkills().Count > 0)
            {
                this.skillMenu.gameObject.SetActive(true);
                this.skillMenu.Refresh(slot.Skill.GetSkills());
            }
            else
            {
                this.skillMenu.gameObject.SetActive(false);
            }

            if (slot.Skill.IsOnCooldown())
            {
                ShowCooldown();
                SetCooldown(slot.Skill.RemainingCooldown, slot.Skill.Cooldown);
            }
            else
            {
                HideCooldown();
            }

            MaybeMarkAsUnavailable();
        }

        public void Enable()
        {
            this.fade.color = Color.black.With(a: 0);
        }

        public void Disable()
        {
            this.fade.color = Color.black.With(a: 0.5f);
        }

        public void Available()
        {
        }

        public void Unavailable()
        {
        }

        public void Highlight()
        {
        }

        public void Unhighlight()
        {
        }

        public void OnDrag(PointerEventData pointer)
        {
        }

        public void OnDrop(PointerEventData pointer)
        {
            var skill = pointer.pointerDrag.GetComponent<SpellbookDraggableSkill>();

            if (skill == null)
            {
                return;
            }

            SkillDroppedIn?.Invoke(Slot, skill.Skill);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (Slot.Skill.IsEmpty())
            {
                return;
            }

            PointerEnter?.Invoke(this);

            if (ShowTooltip)
            {
                SkillTooltip.Instance.Show(Slot.Skill, GetComponent<RectTransform>());
            }
        }

        public void OnPointerExit(PointerEventData pointer)
        {
            if (Slot.Skill.IsEmpty())
            {
                return;
            }

            PointerExit?.Invoke(this);

            if (ShowTooltip)
            {
                SkillTooltip.Instance.Hide();
            }
        }

        public void OnPointerDown(PointerEventData pointer)
        {
            TriggerPointerDown();
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            if (this.draggableSkill.IsDragging)
            {
                return;
            }

            TriggerPointerUp();
        }

        private void TriggerPointerUp()
        {
            PointerUp?.Invoke(this);
        }

        private void TriggerPointerDown()
        {
            SkillTooltip.Instance.Hide();
        }

        private void Update()
        {
            if (this.hotkey == KeyCode.None || UIManager.Instance.ViewStack.Count > 0)
            {
                return;
            }

            if (Input.GetKeyDown(this.hotkey))
            {
                TriggerPointerDown();
            }

            if (Input.GetKeyUp(this.hotkey))
            {
                TriggerPointerUp();
            }
        }
    }
}