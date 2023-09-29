using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Interaction;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Skills;
using DarkBestiary.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class SkillSlotView : PoolableMonoBehaviour, IDropHandler, IDragHandler,
        IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public event Action<Skill, Skill> Selected;
        public event Action<SkillSlotView> PointerUp;
        public event Action<SkillSlotView> PointerEnter;
        public event Action<SkillSlotView> PointerExit;
        public event Action<Skill> SkillDroppedOut;
        public event Action<SkillSlot, Skill> SkillDroppedIn;

        public SkillSlot Slot { get; private set; }
        public bool ShowTooltip { get; set; } = true;
        public bool ShowParticles { get; set; } = true;

        [SerializeField]
        private Image m_Icon;

        [SerializeField]
        private Image m_Fade;

        [SerializeField]
        private Image m_Red;

        [SerializeField]
        private DraggableSkill m_DraggableSkill;

        [SerializeField]
        private TextMeshProUGUI m_CooldownText;

        [SerializeField]
        private Image m_CooldownImage;

        [SerializeField]
        private Image m_Highlight;

        [SerializeField]
        private Image m_WeaponBorder;

        [SerializeField]
        private Image m_UltimateBorder;

        [SerializeField]
        private GameObject m_HotkeyBadge;

        [SerializeField]
        private TextMeshProUGUI m_HotkeyText;

        [SerializeField]
        private Animator m_Animator;

        [SerializeField]
        private TextMeshProUGUI m_CostText;

        private KeyCode m_Hotkey = KeyCode.None;

        public void Initialize(SkillSlot slot)
        {
            Slot = slot;
            Slot.SkillChanged += OnSkillChanged;

            m_DraggableSkill.DroppedOut += OnSkillDroppedOut;

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

            m_DraggableSkill.DroppedOut -= OnSkillDroppedOut;

            var interactor = Container.Instance.Resolve<Interactor>();
            interactor.StateEnter -= OnStateEnter;
            interactor.StateExit -= OnStateExit;

            SpellbookComponent.AnySkillSetBonusApplied -= OnAnySkillSetBonusApplied;
            ResourcesComponent.AnyResourceChanged -= OnAnyResourceChanged;
            BehavioursComponent.AnyBehaviourApplied -= OnAnyBehaviourAppliedOrRemoved;
            BehavioursComponent.AnyBehaviourRemoved -= OnAnyBehaviourAppliedOrRemoved;
        }

        public void SubscribeToCooldownUpdates()
        {
            Slot.Skill.CooldownUpdated += OnSkillCooldownUpdated;
        }

        public void UnsubscribeCooldownUpdates()
        {
            Slot.Skill.CooldownUpdated -= OnSkillCooldownUpdated;
        }

        private void OnSkillCooldownUpdated(Skill skill)
        {
            if (!skill.IsOnCooldown())
            {
                HideCooldown();
                return;
            }

            ShowCooldown();
            SetCooldown(skill.RemainingCooldown, skill.Cooldown);
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

            m_Highlight.color = m_Highlight.color.With(a: 1);
        }

        private void OnStateExit(InteractorState state)
        {
            m_Highlight.color = m_Highlight.color.With(a: 0);
        }

        private void OnAnySkillSetBonusApplied(Skill skill, SkillSet set)
        {
            if (Slot.Skill.IsEmpty() || !ShowParticles)
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

            UpdateCost();
            MaybeMarkAsUnavailable();
        }

        private void MaybeMarkAsUnavailable()
        {
            var isUnavailable = !Slot.Skill.HasEnoughResources() || Slot.Skill.IsDisabled();
            m_Red.color = m_Red.color.With(a: isUnavailable ? 0.3f : 0);
        }

        private void OnSkillSelected(Skill skill)
        {
            Selected?.Invoke(Slot.Skill, skill);
        }

        private void OnSkillDroppedOut(DraggableSkill draggable)
        {
            SkillDroppedOut?.Invoke(draggable.Skill);
        }

        public void SetHotkey(KeyCode hotkey)
        {
            m_Hotkey = hotkey;

            m_HotkeyBadge.gameObject.SetActive(true);
            m_HotkeyText.text = EnumTranslator.Translate(m_Hotkey);
        }

        public void HideHotkey()
        {
            m_HotkeyBadge.gameObject.SetActive(false);
            m_HotkeyText.text = "";
        }

        public void DisableDrag()
        {
            m_DraggableSkill.gameObject.SetActive(false);
        }

        public void HideCooldown()
        {
            m_CooldownText.gameObject.SetActive(false);
            m_CooldownImage.gameObject.SetActive(false);
        }

        public void ShowCooldown()
        {
            m_CooldownText.gameObject.SetActive(true);
            m_CooldownImage.gameObject.SetActive(true);
        }

        public void SetCooldown(float remaining, float total)
        {
            m_CooldownImage.fillAmount = 1.0f - (total - remaining) / total;
            m_CooldownText.text = ((int) remaining).ToString();
        }

        public void PlayCooldownPulse()
        {
            m_Animator.Play("cooldown_pulse");
        }

        private void OnSkillChanged(SkillSlot slot)
        {
            m_Icon.sprite = Resources.Load<Sprite>(Slot.Skill.Icon);
            m_Icon.enabled = !Slot.Skill.IsEmpty();
            m_WeaponBorder.gameObject.SetActive(Slot.SkillType == SkillType.Weapon);
            m_UltimateBorder.gameObject.SetActive(Slot.Skill.Rarity?.Type == RarityType.Legendary);

            m_DraggableSkill.Change(slot.Skill);

            if (slot.Skill.IsOnCooldown())
            {
                ShowCooldown();
                SetCooldown(slot.Skill.RemainingCooldown, slot.Skill.Cooldown);
            }
            else
            {
                HideCooldown();
            }

            UpdateCost();
            MaybeMarkAsUnavailable();
        }

        private void UpdateCost()
        {
            if (m_CostText == null)
            {
                return;
            }

            var cost = Slot.Skill.GetCost(ResourceType.ActionPoint);

            m_CostText.transform.parent.gameObject.SetActive(cost >= 1);
            m_CostText.text = cost.ToString("F0");
        }

        public void Enable()
        {
            m_Fade.color = Color.black.With(a: 0);
        }

        public void Disable()
        {
            m_Fade.color = Color.black.With(a: 0.5f);
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
            var draggable = pointer.pointerDrag.GetComponent<DraggableSkill>();

            if (draggable == null)
            {
                return;
            }

            SkillDroppedIn?.Invoke(Slot, draggable.Skill);
        }

        public void OnPointerEnter(PointerEventData pointer)
        {
            if (Slot.Skill.IsEmpty() || pointer.dragging)
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
            if (m_DraggableSkill.IsDragging)
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
            if (m_Hotkey == KeyCode.None || UIManager.Instance.IsAnyFullscreenUiOpen())
            {
                return;
            }

            if (Input.GetKeyDown(m_Hotkey))
            {
                TriggerPointerDown();
            }

            if (Input.GetKeyUp(m_Hotkey))
            {
                TriggerPointerUp();
            }
        }
    }
}