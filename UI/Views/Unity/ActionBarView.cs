using System.Collections.Generic;
using System.Linq;
using DarkBestiary.GameBoard;
using DarkBestiary.Interaction;
using DarkBestiary.Messaging;
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

        private readonly List<SpellbookSlot> spellbookSlots = new List<SpellbookSlot>();

        [SerializeField] private SpellbookSlot slotPrefab;
        [SerializeField] private Transform slotContainerLeft;
        [SerializeField] private Transform slotContainerRight;
        [SerializeField] private EndTurnButton endTurnButton;
        [SerializeField] private ActionPoint actionPointPrefab;
        [SerializeField] private Transform actionPointContainer;
        [SerializeField] private BehavioursPanel behaviourFrame;
        [SerializeField] private HealthFrame healthFrame;
        [SerializeField] private RectTransform tooltipAnchor;
        [SerializeField] private Interactable swapWeaponButton;
        [SerializeField] private Image rageFiller;
        [SerializeField] private TextMeshProUGUI rageText;

        private readonly List<ActionPoint> actionPointDots = new List<ActionPoint>();

        private bool isEnabled = true;

        protected override void OnInitialize()
        {
            Active = this;

            this.endTurnButton.PointerUp += OnEndTurnButtonPointerUp;
            this.swapWeaponButton.PointerUp += OnSwapWeaponButtonPointerUp;
            this.swapWeaponButton.PointerEnter += OnSwapWeaponButtonPointerEnter;
            this.swapWeaponButton.PointerExit += OnSwapWeaponButtonPointerExit;
        }

        protected override void OnTerminate()
        {
            Active = null;

            this.endTurnButton.PointerUp -= OnEndTurnButtonPointerUp;
            this.swapWeaponButton.PointerUp -= OnSwapWeaponButtonPointerUp;
            this.swapWeaponButton.PointerEnter -= OnSwapWeaponButtonPointerEnter;
            this.swapWeaponButton.PointerExit -= OnSwapWeaponButtonPointerExit;
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

        public void EnableSkills()
        {
            this.spellbookSlots.ForEach(button => button.Enable());
            this.isEnabled = true;
        }

        public void DisableSkills()
        {
            this.spellbookSlots.ForEach(button => button.Disable());
            this.isEnabled = false;
        }

        public void CreateSkills(IReadOnlyList<SkillSlot> slots)
        {
            foreach (var slot in slots)
            {
                var spellbookSlot = Instantiate(
                    this.slotPrefab, slot.Index < 5 ? this.slotContainerLeft : this.slotContainerRight);

                spellbookSlot.PointerUp += OnSlotPointerUp;
                spellbookSlot.SkillDroppedIn += OnSlotSkillDroppedIn;
                spellbookSlot.SkillDroppedOut += OnSkillDroppedOut;
                spellbookSlot.PointerEnter += OnSlotPointerEnter;
                spellbookSlot.PointerExit += OnSlotPointerExit;
                spellbookSlot.Selected += OnSkillSelected;
                spellbookSlot.ShowTooltip = false;
                spellbookSlot.Initialize(slot);
                spellbookSlot.SetHotkey(KeyCodes.Hotkeys[slot.Index]);
                this.spellbookSlots.Add(spellbookSlot);
            }
        }

        private void OnSlotPointerEnter(SpellbookSlot slot)
        {
            if (Interactor.Instance.IsSelectionState || Interactor.Instance.IsWaitState)
            {
                BoardNavigator.Instance.Board.Clear();
                BoardNavigator.Instance.HighlightSkillRangeDefault(
                    slot.Slot.Skill,
                    slot.Slot.Skill.Caster.transform.position,
                    slot.Slot.Skill.Caster.transform.position
                );
            }

            SkillTooltip.Instance.Show(slot.Slot.Skill, this.tooltipAnchor);

            ShowActionPointUsage(slot.Slot.Skill.GetCost(ResourceType.ActionPoint));
        }

        private void OnSlotPointerExit(SpellbookSlot slot)
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

        public void RemoveSkills()
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
            this.rageFiller.fillAmount = current / maximum;
            this.rageText.text = $"{(int) current}/{(int) maximum}";
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

        private void OnEndTurnButtonPointerUp()
        {
            EndTurnButtonClicked?.Invoke();
        }

        private void OnSwapWeaponButtonPointerUp()
        {
            SwapWeaponButtonClicked?.Invoke();
        }

        private void OnSwapWeaponButtonPointerEnter()
        {
            Tooltip.Instance.Show(I18N.Instance.Get("ui_swap_weapon") + " [Tab]", this.swapWeaponButton.GetComponent<RectTransform>());
        }

        private void OnSwapWeaponButtonPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        private void OnSlotPointerUp(SpellbookSlot slot)
        {
            if (slot.Slot.Skill.IsEmpty() || !this.isEnabled)
            {
                return;
            }

            SkillClicked?.Invoke(slot.Slot.Skill);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                OnSwapWeaponButtonPointerUp();
            }
        }
    }
}