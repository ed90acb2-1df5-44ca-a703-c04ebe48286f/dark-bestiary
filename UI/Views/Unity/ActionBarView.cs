using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
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

        public event Payload<Skill, Skill> SkillSelected;
        public event Payload<SkillSlot, Skill> SkillPlaced;
        public event Payload<Skill> SkillRemoved;
        public event Payload<Skill> SkillClicked;
        public event Payload EndTurnButtonClicked;
        public event Payload SwapWeaponButtonClicked;

        private readonly List<SkillSlotView> spellbookSlots = new List<SkillSlotView>();

        [SerializeField] private SkillSlotView slotPrefab;
        [SerializeField] private Transform slotContainerLeft;
        [SerializeField] private Transform slotContainerRight;
        [SerializeField] private EndTurnButton endTurnButton;
        [SerializeField] private ActionPoint actionPointPrefab;
        [SerializeField] private RectTransform actionPointContainer;
        [SerializeField] private BehavioursPanel behaviourFrame;
        [SerializeField] private HealthFrame healthFrame;
        [SerializeField] private RectTransform tooltipAnchor;
        [SerializeField] private Interactable swapWeaponButton;
        [SerializeField] private Interactable potionBagButton;
        [SerializeField] private Image rageFiller;
        [SerializeField] private TextMeshProUGUI rageText;
        [SerializeField] private Interactable actionPointInteractable;
        [SerializeField] private Interactable rageInteractable;
        [SerializeField] private PotionBag potionBag;
        [SerializeField] private ParticleSystem rageParticles;

        private readonly List<ActionPoint> actionPointDots = new List<ActionPoint>();

        private bool isEnabled = true;

        private void Start()
        {
            Active = this;

            BoardCell.AnyCellMouseEnter += OnAnyCellMouseEnter;
            BoardCell.AnyCellMouseExit += OnAnyCellMouseExit;

            this.actionPointInteractable.PointerEnter += OnActionPointInteractablePointerEnter;
            this.actionPointInteractable.PointerExit += OnActionPointInteractablePointerExit;
            this.rageInteractable.PointerEnter += OnRageInteractablePointerEnter;
            this.rageInteractable.PointerExit += OnRageInteractablePointerExit;
            this.endTurnButton.PointerClick += OnEndTurnButtonPointerClick;

            this.swapWeaponButton.PointerClick += OnSwapWeaponButtonPointerClick;
            this.swapWeaponButton.PointerEnter += OnSwapWeaponButtonPointerEnter;
            this.swapWeaponButton.PointerExit += OnSwapWeaponButtonPointerExit;

            this.potionBagButton.PointerClick += OnPotionBagButtonPointerClick;
            this.potionBagButton.PointerEnter += OnPotionBagButtonPointerEnter;
            this.potionBagButton.PointerExit += OnPotionBagButtonPointerExit;

            this.potionBag.Initialize(CharacterManager.Instance.Character.Entity.GetComponent<InventoryComponent>());

            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = rectTransform.anchorMin.With(y: Screen.safeArea.position.y / Screen.height);
        }

        protected override void OnTerminate()
        {
            Active = null;

            BoardCell.AnyCellMouseEnter -= OnAnyCellMouseEnter;
            BoardCell.AnyCellMouseExit -= OnAnyCellMouseExit;

            this.actionPointInteractable.PointerEnter -= OnActionPointInteractablePointerEnter;
            this.actionPointInteractable.PointerExit -= OnActionPointInteractablePointerExit;
            this.rageInteractable.PointerEnter -= OnRageInteractablePointerEnter;
            this.rageInteractable.PointerExit -= OnRageInteractablePointerExit;
            this.endTurnButton.PointerClick -= OnEndTurnButtonPointerClick;

            this.swapWeaponButton.PointerClick -= OnSwapWeaponButtonPointerClick;
            this.swapWeaponButton.PointerEnter -= OnSwapWeaponButtonPointerEnter;
            this.swapWeaponButton.PointerExit -= OnSwapWeaponButtonPointerExit;

            this.potionBagButton.PointerClick -= OnPotionBagButtonPointerClick;
            this.potionBagButton.PointerEnter -= OnPotionBagButtonPointerEnter;
            this.potionBagButton.PointerExit -= OnPotionBagButtonPointerExit;

            this.potionBag.Terminate();
        }

        private void OnDisable()
        {
            Tooltip.Instance.Hide();
            SkillTooltip.Instance.Hide();
            this.potionBag.gameObject.SetActive(false);
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

            Tooltip.Instance.Show(unit.Name, description, this.tooltipAnchor);
        }

        private void OnAnyCellMouseExit(BoardCell cell)
        {
            Tooltip.Instance.Hide();
        }

        private void OnRageInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                I18N.Instance.Get("resource_rage"), I18N.Instance.Get("resource_rage_description"), this.tooltipAnchor);
        }

        private void OnRageInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnActionPointInteractablePointerEnter()
        {
            Tooltip.Instance.Show(
                I18N.Instance.Get("resource_action_points"), I18N.Instance.Get("resource_action_points_description"), this.tooltipAnchor);
        }

        private void OnActionPointInteractablePointerExit()
        {
            Tooltip.Instance.Hide();
        }

        public void SetPotionBagEnabled(bool isEnabled)
        {
            this.potionBagButton.gameObject.SetActive(isEnabled);
            this.potionBag.gameObject.SetActive(false);
        }

        public void SetPoisoned(bool isPoisoned)
        {
            this.healthFrame.SetPoisoned(isPoisoned);
        }

        public void AddBehaviour(Behaviour behaviour)
        {
            this.behaviourFrame.Add(behaviour);
        }

        public void RemoveBehaviour(Behaviour behaviour)
        {
            this.behaviourFrame.Remove(behaviour);
        }

        public void RemoveBehaviours()
        {
            this.behaviourFrame.Clear();
        }

        public void EnableSkillSlots()
        {
            this.spellbookSlots.ForEach(button => button.Enable());
            this.isEnabled = true;
        }

        public void DisableSkillSlots()
        {
            this.spellbookSlots.ForEach(button => button.Disable());
            this.isEnabled = false;
        }

        public void CreateSkillSlots(IReadOnlyList<SkillSlot> slots)
        {
            foreach (var slot in slots)
            {
                var spellbookSlot = Instantiate(
                    this.slotPrefab, slot.Index < 5 ? this.slotContainerLeft : this.slotContainerRight);

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
                this.spellbookSlots.Add(spellbookSlot);
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

            SkillTooltip.Instance.Show(slotView.Slot.Skill, this.tooltipAnchor);

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
            foreach (var slot in this.spellbookSlots)
            {
                slot.Terminate();
                Destroy(slot.gameObject);
            }

            this.spellbookSlots.Clear();
        }

        public void HighlightSkill(Skill skill)
        {
            this.spellbookSlots.FirstOrDefault(button => button.Slot.Skill == skill)?.Highlight();
        }

        public void UnhighlightSkill(Skill skill)
        {
            this.spellbookSlots.FirstOrDefault(button => button.Slot.Skill == skill)?.Unhighlight();
        }

        public void StartSkillCooldown(Skill skill)
        {
            var slot = this.spellbookSlots.First(b => b.Slot.Skill == skill);
            slot.ShowCooldown();
            slot.SetCooldown(skill.RemainingCooldown, skill.Cooldown);
        }

        public void UpdateSkillCooldown(Skill skill)
        {
            this.spellbookSlots.First(b => b.Slot.Skill == skill)
                .SetCooldown(skill.RemainingCooldown, skill.Cooldown);
        }

        public void StopSkillCooldown(Skill skill)
        {
            var slot = this.spellbookSlots.First(b => b.Slot.Skill == skill);
            slot.HideCooldown();
            slot.PlayCooldownPulse();
        }

        public void EnableEndTurnButton()
        {
            this.endTurnButton.Active = true;
        }

        public void DisableEndTurnButton()
        {
            this.endTurnButton.Active = false;
        }

        public void ShowActionPointUsage(float count)
        {
            ShowActionPointUsage((int) count);
        }

        public void ShowActionPointUsage(int count)
        {
            HideActionPointUsage();

            foreach (var actionPointDot in this.actionPointDots.Where(point => point.IsOn).Reverse().Take(count))
            {
                actionPointDot.Highlight();
            }
        }

        public void HideActionPointUsage()
        {
            foreach (var actionPointDot in this.actionPointDots.Where(point => point.IsHighlighted))
            {
                actionPointDot.Unhighlight();
            }
        }

        public void UpdateRageAmount(float current, float maximum)
        {
            this.rageParticles.gameObject.SetActive(Math.Abs(current - maximum) < Mathf.Epsilon);

            this.rageFiller.fillAmount = current / maximum;
            this.rageText.text = $"{((int) current).ToString()}/{((int) maximum).ToString()}";
        }

        public void UpdateActionPointsAmount(float current, float maximum)
        {
            AdjustActionPointDotCountToResourceMaxAmount(maximum);

            for (var dotIndex = 0; dotIndex < (int) maximum; dotIndex++)
            {
                var dot = this.actionPointDots[dotIndex];

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
            this.healthFrame.Refresh(health, shield, maximum);
        }

        private void AdjustActionPointDotCountToResourceMaxAmount(float maximum)
        {
            while (this.actionPointDots.Count != (int) maximum)
            {
                if (this.actionPointDots.Count > maximum)
                {
                    while (this.actionPointDots.Count > maximum)
                    {
                        var actionPoint = this.actionPointDots.Last();
                        this.actionPointDots.Remove(actionPoint);
                        Destroy(actionPoint.gameObject);
                    }

                    break;
                }

                this.actionPointDots.Add(Instantiate(this.actionPointPrefab, this.actionPointContainer));
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
            Tooltip.Instance.Show(I18N.Instance.Get("ui_swap_weapon"), this.swapWeaponButton.GetComponent<RectTransform>());
        }

        private void OnSwapWeaponButtonPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnPotionBagButtonPointerClick()
        {
            this.potionBag.gameObject.SetActive(!this.potionBag.gameObject.activeSelf);
            ItemTooltip.Instance.Hide();
        }

        private void OnPotionBagButtonPointerEnter()
        {
            Tooltip.Instance.Show(I18N.Instance.Get("ui_potion_bag"), this.potionBagButton.GetComponent<RectTransform>());
        }

        private void OnPotionBagButtonPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnSlotPointerUp(SkillSlotView slotView)
        {
            if (slotView.Slot.Skill.IsEmpty() || !this.isEnabled)
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