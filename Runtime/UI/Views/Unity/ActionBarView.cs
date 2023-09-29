using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using DarkBestiary.UI.Elements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Views.Unity
{
    public class ActionBarView : View, IActionBarView
    {
        public static ActionBarView Active { get; private set; }

        public event Action<Skill, Skill> SkillSelected;
        public event Action<SkillSlot, Skill> SkillPlaced;
        public event Action<Skill> SkillRemoved;
        public event Action<Skill> SkillClicked;
        public event Action EndTurnButtonClicked;
        public event Action SwapWeaponButtonClicked;

        private readonly List<SkillSlotView> m_SpellbookSlots = new();

        [SerializeField] private SkillSlotView m_SlotPrefab;
        [SerializeField] private Transform m_SlotContainerLeft;
        [SerializeField] private Transform m_SlotContainerRight;
        [SerializeField] private EndTurnButton m_EndTurnButton;
        [SerializeField] private ActionPoint m_ActionPointPrefab;
        [SerializeField] private RectTransform m_ActionPointContainer;
        [SerializeField] private BehavioursPanel m_BehaviourFrame;
        [SerializeField] private HealthFrame m_HealthFrame;
        [SerializeField] private RectTransform m_TooltipAnchor;
        [SerializeField] private Interactable m_SwapWeaponButton;
        [SerializeField] private Interactable m_PotionBagButton;
        [SerializeField] private Image m_RageFiller;
        [SerializeField] private TextMeshProUGUI m_RageText;
        [SerializeField] private Interactable m_ActionPointInteractable;
        [SerializeField] private Interactable m_RageInteractable;
        [SerializeField] private PotionBag m_PotionBag;
        [SerializeField] private ParticleSystem m_RageParticles;

        private readonly List<ActionPoint> m_ActionPointDots = new();

        private bool m_IsEnabled = true;

        private void Start()
        {
            Active = this;

            BoardCell.AnyCellMouseEnter += OnAnyCellMouseEnter;
            BoardCell.AnyCellMouseExit += OnAnyCellMouseExit;

            m_ActionPointInteractable.PointerEnter += OnActionPointInteractablePointerEnter;
            m_ActionPointInteractable.PointerExit += OnActionPointInteractablePointerExit;
            m_RageInteractable.PointerEnter += OnRageInteractablePointerEnter;
            m_RageInteractable.PointerExit += OnRageInteractablePointerExit;
            m_EndTurnButton.PointerClick += OnEndTurnButtonPointerClick;

            m_SwapWeaponButton.PointerClick += OnSwapWeaponButtonPointerClick;
            m_SwapWeaponButton.PointerEnter += OnSwapWeaponButtonPointerEnter;
            m_SwapWeaponButton.PointerExit += OnSwapWeaponButtonPointerExit;

            m_PotionBagButton.PointerClick += OnPotionBagButtonPointerClick;
            m_PotionBagButton.PointerEnter += OnPotionBagButtonPointerEnter;
            m_PotionBagButton.PointerExit += OnPotionBagButtonPointerExit;

            m_PotionBag.Initialize(Game.Instance.Character.Entity.GetComponent<InventoryComponent>());

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = rectTransform.anchorMin.With(y: Screen.safeArea.position.y / Screen.height);
        }

        protected override void OnTerminate()
        {
            Active = null;

            BoardCell.AnyCellMouseEnter -= OnAnyCellMouseEnter;
            BoardCell.AnyCellMouseExit -= OnAnyCellMouseExit;

            m_ActionPointInteractable.PointerEnter -= OnActionPointInteractablePointerEnter;
            m_ActionPointInteractable.PointerExit -= OnActionPointInteractablePointerExit;
            m_RageInteractable.PointerEnter -= OnRageInteractablePointerEnter;
            m_RageInteractable.PointerExit -= OnRageInteractablePointerExit;
            m_EndTurnButton.PointerClick -= OnEndTurnButtonPointerClick;

            m_SwapWeaponButton.PointerClick -= OnSwapWeaponButtonPointerClick;
            m_SwapWeaponButton.PointerEnter -= OnSwapWeaponButtonPointerEnter;
            m_SwapWeaponButton.PointerExit -= OnSwapWeaponButtonPointerExit;

            m_PotionBagButton.PointerClick -= OnPotionBagButtonPointerClick;
            m_PotionBagButton.PointerEnter -= OnPotionBagButtonPointerEnter;
            m_PotionBagButton.PointerExit -= OnPotionBagButtonPointerExit;

            m_PotionBag.Terminate();
        }

        private void OnDisable()
        {
            Tooltip.Instance.Hide();
            SkillTooltip.Instance.Hide();
            m_PotionBag.gameObject.SetActive(false);
        }

        private void OnAnyCellMouseEnter(BoardCell cell)
        {
            if (!cell.IsOccupied || UIManager.Instance.IsGameFieldBlockedByUI())
            {
                return;
            }

            var properties = cell.OccupiedBy.GetComponent<PropertiesComponent>();
            var unit = cell.OccupiedBy.GetComponent<UnitComponent>();

            var damageEffect = cell.OccupiedBy.GetComponent<SpellbookComponent>()
                .Slots
                .FirstOrDefault(s => !s.IsEmpty && s.SkillType == SkillType.Weapon)?
                .Skill
                .GetFirstDamageEffect();

            var description = "";
            description += $"{I18N.Instance.Translate("ui_level")}: {unit.Level}\n";
            description += $"{I18N.Instance.Translate("ui_challenge_rating")}: {unit.ChallengeRating}\n\n";
            description += $"{I18N.Instance.Translate("ui_attack")}: {damageEffect?.GetAmountString(cell.OccupiedBy) ?? "-"}\n";
            description += $"{I18N.Instance.Translate("ui_health")}: {properties.Get(PropertyType.Health).ValueString()}\n";
            description += $"{I18N.Instance.Translate("ui_physical_resistance")}: {properties.AveragePhysicalResistance() * 100:F2}%\n";
            description += $"{I18N.Instance.Translate("ui_magical_resistance")}: {properties.AverageMagicalResistance() * 100:F2}%";

            Tooltip.Instance.Show(unit.Name, description, m_TooltipAnchor);
        }

        private void OnAnyCellMouseExit(BoardCell cell)
        {
            Tooltip.Instance.Hide();
        }

        private void OnRageInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                I18N.Instance.Get("resource_rage"), I18N.Instance.Get("resource_rage_description"), m_TooltipAnchor);
        }

        private void OnRageInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnActionPointInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                I18N.Instance.Get("resource_action_points"), I18N.Instance.Get("resource_action_points_description"), m_TooltipAnchor);
        }

        private void OnActionPointInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        public void SetPotionBagEnabled(bool isEnabled)
        {
            m_PotionBagButton.gameObject.SetActive(isEnabled);
            m_PotionBag.gameObject.SetActive(false);
        }

        public void SetPoisoned(bool isPoisoned)
        {
            m_HealthFrame.SetPoisoned(isPoisoned);
        }

        public void AddBehaviour(Behaviour behaviour)
        {
            m_BehaviourFrame.Add(behaviour);
        }

        public void RemoveBehaviour(Behaviour behaviour)
        {
            m_BehaviourFrame.Remove(behaviour);
        }

        public void RemoveBehaviours()
        {
            m_BehaviourFrame.Clear();
        }

        public void EnableSkillSlots()
        {
            m_SpellbookSlots.ForEach(button => button.Enable());
            m_IsEnabled = true;
        }

        public void DisableSkillSlots()
        {
            m_SpellbookSlots.ForEach(button => button.Disable());
            m_IsEnabled = false;
        }

        public void CreateSkillSlots(IReadOnlyList<SkillSlot> slots)
        {
            foreach (var slot in slots)
            {
                var spellbookSlot = Instantiate(
                    m_SlotPrefab, slot.Index < 5 ? m_SlotContainerLeft : m_SlotContainerRight);

                var skillKeyBindings = KeyBindings.Skills();

                spellbookSlot.PointerUp += OnSlotPointerUp;
                spellbookSlot.SkillDroppedIn += OnSlotSkillDroppedIn;
                spellbookSlot.SkillDroppedOut += OnSkillDroppedOut;
                spellbookSlot.PointerEnter += OnSlotPointerEnter;
                spellbookSlot.PointerExit += OnSlotPointerExit;
                spellbookSlot.Selected += OnSkillSelected;
                spellbookSlot.ShowTooltip = false;
                spellbookSlot.Initialize(slot);
                spellbookSlot.SetHotkey(skillKeyBindings[slot.Index]);
                m_SpellbookSlots.Add(spellbookSlot);
            }
        }

        private void OnSlotPointerEnter(SkillSlotView slotView)
        {
            if (Interactor.Instance.IsSelectionState || Interactor.Instance.IsWaitState)
            {
                BoardNavigator.Instance.Board.Clear();
                BoardNavigator.Instance.HighlightSkillRangeDefault(
                    slotView.Slot.Skill,
                    slotView.Slot.Skill.Caster.transform.position,
                    slotView.Slot.Skill.Caster.transform.position
                );
            }

            SkillTooltip.Instance.Show(slotView.Slot.Skill, m_TooltipAnchor);

            ShowActionPointUsage(slotView.Slot.Skill.GetCost(ResourceType.ActionPoint));
        }

        private void OnSlotPointerExit(SkillSlotView slotView)
        {
            if (Interactor.Instance.IsSelectionState || Interactor.Instance.IsWaitState)
            {
                BoardNavigator.Instance.Board.Clear();
            }

            SkillTooltip.Instance.Hide();

            HideActionPointUsage();
        }

        private void OnSkillSelected(Skill skillA, Skill skillB)
        {
            SkillSelected?.Invoke(skillA, skillB);
        }

        private void OnSlotSkillDroppedIn(SkillSlot slot, Skill skill)
        {
            if (slot.SkillType == SkillType.Weapon)
            {
                return;
            }

            SkillPlaced?.Invoke(slot, skill);
        }

        private void OnSkillDroppedOut(Skill skill)
        {
            if (skill.Type == SkillType.Weapon)
            {
                return;
            }

            SkillRemoved?.Invoke(skill);
        }

        public void RemoveSkillSlots()
        {
            foreach (var slot in m_SpellbookSlots)
            {
                slot.Terminate();
                Destroy(slot.gameObject);
            }

            m_SpellbookSlots.Clear();
        }

        public void HighlightSkill(Skill skill)
        {
            m_SpellbookSlots.FirstOrDefault(button => button.Slot.Skill == skill)?.Highlight();
        }

        public void UnhighlightSkill(Skill skill)
        {
            m_SpellbookSlots.FirstOrDefault(button => button.Slot.Skill == skill)?.Unhighlight();
        }

        public void StartSkillCooldown(Skill skill)
        {
            var slot = m_SpellbookSlots.First(b => b.Slot.Skill == skill);
            slot.ShowCooldown();
            slot.SetCooldown(skill.RemainingCooldown, skill.Cooldown);
        }

        public void UpdateSkillCooldown(Skill skill)
        {
            m_SpellbookSlots.First(b => b.Slot.Skill == skill)
                .SetCooldown(skill.RemainingCooldown, skill.Cooldown);
        }

        public void StopSkillCooldown(Skill skill)
        {
            var slot = m_SpellbookSlots.First(b => b.Slot.Skill == skill);
            slot.HideCooldown();
            slot.PlayCooldownPulse();
        }

        public void EnableEndTurnButton()
        {
            m_EndTurnButton.Active = true;
        }

        public void DisableEndTurnButton()
        {
            m_EndTurnButton.Active = false;
        }

        public void ShowActionPointUsage(float count)
        {
            ShowActionPointUsage((int) count);
        }

        public void ShowActionPointUsage(int count)
        {
            HideActionPointUsage();

            foreach (var actionPointDot in m_ActionPointDots.Where(point => point.IsOn).Reverse().Take(count))
            {
                actionPointDot.Highlight();
            }
        }

        public void HideActionPointUsage()
        {
            foreach (var actionPointDot in m_ActionPointDots.Where(point => point.IsHighlighted))
            {
                actionPointDot.Unhighlight();
            }
        }

        public void UpdateRageAmount(float current, float maximum)
        {
            m_RageParticles.gameObject.SetActive(Math.Abs(current - maximum) < Mathf.Epsilon);

            m_RageFiller.fillAmount = current / maximum;
            m_RageText.text = $"{((int) current).ToString()}/{((int) maximum).ToString()}";
        }

        public void UpdateActionPointsAmount(float current, float maximum)
        {
            AdjustActionPointDotCountToResourceMaxAmount(maximum);

            for (var dotIndex = 0; dotIndex < (int) maximum; dotIndex++)
            {
                var dot = m_ActionPointDots[dotIndex];

                if (dotIndex < current)
                {
                    dot.On();
                }
                else
                {
                    dot.Off();
                }
            }
        }

        public void UpdateHealth(float health, float shield, float maximum)
        {
            m_HealthFrame.Refresh(health, shield, maximum);
        }

        private void AdjustActionPointDotCountToResourceMaxAmount(float maximum)
        {
            while (m_ActionPointDots.Count != (int) maximum)
            {
                if (m_ActionPointDots.Count > maximum)
                {
                    while (m_ActionPointDots.Count > maximum)
                    {
                        var actionPoint = m_ActionPointDots.Last();
                        m_ActionPointDots.Remove(actionPoint);
                        Destroy(actionPoint.gameObject);
                    }

                    break;
                }

                m_ActionPointDots.Add(Instantiate(m_ActionPointPrefab, m_ActionPointContainer));
            }
        }

        private void OnEndTurnButtonPointerClick()
        {
            EndTurnButtonClicked?.Invoke();
        }

        private void OnSwapWeaponButtonPointerClick()
        {
            SwapWeaponButtonClicked?.Invoke();
        }

        private void OnSwapWeaponButtonPointerEnter()
        {
            Tooltip.Instance.Show(I18N.Instance.Get("ui_swap_weapon"), m_SwapWeaponButton.GetComponent<RectTransform>());
        }

        private void OnSwapWeaponButtonPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnPotionBagButtonPointerClick()
        {
            m_PotionBag.gameObject.SetActive(!m_PotionBag.gameObject.activeSelf);
            ItemTooltip.Instance.Hide();
        }

        private void OnPotionBagButtonPointerEnter()
        {
            Tooltip.Instance.Show(I18N.Instance.Get("ui_potion_bag"), m_PotionBagButton.GetComponent<RectTransform>());
        }

        private void OnPotionBagButtonPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnSlotPointerUp(SkillSlotView slotView)
        {
            if (slotView.Slot.Skill.IsEmpty() || !m_IsEnabled)
            {
                return;
            }

            SkillClicked?.Invoke(slotView.Slot.Skill);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyBindings.Get(KeyType.SwapWeapon)))
            {
                OnSwapWeaponButtonPointerClick();
            }

            if (Input.GetKeyDown(KeyBindings.Get(KeyType.ConsumablesBag)))
            {
                OnPotionBagButtonPointerClick();
            }
        }
    }
}