using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class NavigationViewController : ViewController<INavigationView>
    {
        private readonly TalentsComponent talents;
        private readonly AttributesComponent attributes;
        private readonly ReliquaryComponent reliquary;
        private readonly SpecializationsComponent specializations;

        private CombatLogViewController combatLogViewController;
        private SpecializationViewController specializationViewController;
        private MasteriesViewController masteriesViewController;
        private EquipmentViewController equipmentViewController;
        private ReliquaryViewController reliquaryViewController;
        private MailboxViewController mailboxViewController;
        private MenuViewController menuViewController;
        private SpellbookViewController spellbookViewController;
        private TalentsViewController talentsViewController;
        private AchievementsViewController achievementsViewController;
        private AttributesViewController attributesViewController;

        private IView activeView;

        public NavigationViewController(INavigationView view, CharacterManager characterManager) : base(view)
        {
            this.talents = characterManager.Character.Entity.GetComponent<TalentsComponent>();
            this.attributes = characterManager.Character.Entity.GetComponent<AttributesComponent>();
            this.reliquary = characterManager.Character.Entity.GetComponent<ReliquaryComponent>();
            this.specializations = characterManager.Character.Entity.GetComponent<SpecializationsComponent>();
        }

        protected override void OnInitialize()
        {
            View.ToggleAchievements += OnToggleAchievements;
            View.ToggleSpecializations += OnToggleSpecializations;
            View.ToggleMasteries += OnToggleMasteries;
            View.ToggleAttributes += OnToggleAttributes;
            View.ToggleCombatLog += OnToggleCombatLog;
            View.ToggleEquipment += OnToggleEquipment;
            View.ToggleReliquary += OnToggleReliquary;
            View.ToggleMail += OnToggleMail;
            View.ToggleMenu += OnToggleMenu;
            View.ToggleSkills += OnToggleSkills;
            View.ToggleTalents += OnToggleTalents;

            Mailbox.Instance.Updated += OnMailboxUpdated;
            OnMailboxUpdated();

            this.specializations.SkillPointsChanged += OnSkillPointsChanged;
            OnSkillPointsChanged(this.specializations);

            this.reliquary.Unlocked += OnReliquaryUpdated;
            this.reliquary.Equipped += OnReliquaryUpdated;
            this.reliquary.Unequipped += OnReliquaryUpdated;
            OnReliquaryUpdated(null);

            this.attributes.PointsChanged += OnAttributePointsChanged;
            UpdateAttributePointsBadge();

            this.talents.PointsChanged += OnPointsChanged;
            UpdateTalentPointsBadge();

            this.equipmentViewController = ViewControllerRegistry.Initialize<EquipmentViewController>();

            this.masteriesViewController = ViewControllerRegistry.Initialize<MasteriesViewController>();
            this.achievementsViewController = ViewControllerRegistry.Initialize<AchievementsViewController>();
            this.attributesViewController = ViewControllerRegistry.Initialize<AttributesViewController>();
            this.specializationViewController = ViewControllerRegistry.Initialize<SpecializationViewController>();
            this.combatLogViewController = ViewControllerRegistry.Initialize<CombatLogViewController>();
            this.reliquaryViewController = ViewControllerRegistry.Initialize<ReliquaryViewController>();
            this.mailboxViewController = ViewControllerRegistry.Initialize<MailboxViewController>();
            this.menuViewController = ViewControllerRegistry.Initialize<MenuViewController>();
            this.spellbookViewController = ViewControllerRegistry.Initialize<SpellbookViewController>();
            this.talentsViewController = ViewControllerRegistry.Initialize<TalentsViewController>();
        }

        protected override void OnTerminate()
        {
            View.ToggleAchievements -= OnToggleAchievements;
            View.ToggleSpecializations -= OnToggleSpecializations;
            View.ToggleMasteries -= OnToggleMasteries;
            View.ToggleAttributes -= OnToggleAttributes;
            View.ToggleCombatLog -= OnToggleCombatLog;
            View.ToggleEquipment -= OnToggleEquipment;
            View.ToggleReliquary -= OnToggleReliquary;
            View.ToggleMail -= OnToggleMail;
            View.ToggleMenu -= OnToggleMenu;
            View.ToggleSkills -= OnToggleSkills;
            View.ToggleTalents -= OnToggleTalents;

            this.specializations.SkillPointsChanged -= OnSkillPointsChanged;
            this.reliquary.Unlocked -= OnReliquaryUpdated;
            this.reliquary.Equipped -= OnReliquaryUpdated;
            this.reliquary.Unequipped -= OnReliquaryUpdated;

            Mailbox.Instance.Updated -= OnMailboxUpdated;

            this.attributes.PointsChanged -= OnAttributePointsChanged;
            this.talents.PointsChanged -= OnPointsChanged;

            this.masteriesViewController.Terminate();
            this.achievementsViewController.Terminate();
            this.attributesViewController.Terminate();
            this.specializationViewController.Terminate();
            this.combatLogViewController.Terminate();
            this.equipmentViewController.Terminate();
            this.reliquaryViewController.Terminate();
            this.mailboxViewController.Terminate();
            this.menuViewController.Terminate();
            this.spellbookViewController.Terminate();
            this.talentsViewController.Terminate();
        }

        protected override void OnViewHidden()
        {
            this.masteriesViewController.View.Hide();
            this.specializationViewController.View.Hide();
            this.achievementsViewController.View.Hide();
            this.attributesViewController.View.Hide();
            this.combatLogViewController.View.Hide();
            this.equipmentViewController.View.Hide();
            this.reliquaryViewController.View.Hide();
            this.mailboxViewController.View.Hide();
            this.menuViewController.View.Hide();
            this.spellbookViewController.View.Hide();
            this.talentsViewController.View.Hide();
        }

        private void OnSkillPointsChanged(SpecializationsComponent specializations)
        {
            return;

            if (specializations.SkillPoints >= 150)
            {
                View.HighlightSpecializationsButton();
            }
            else
            {
                View.UnhighlightSpecializationsButton();
            }
        }

        private void OnReliquaryUpdated(Relic _)
        {
            if (this.reliquary.Slots.Any(slot => slot.IsEmpty) &&
                this.reliquary.Available.Any(relic => !relic.IsEquipped))
            {
                View.HighlightReliquaryButton();
            }
            else
            {
                View.UnhighlightReliquaryButton();
            }
        }

        private void OnMailboxUpdated()
        {
            if (Mailbox.Instance.Items.Count > 0)
            {
                View.HighlightMailButton();
            }
            else
            {
                View.UnhighlightMailButton();
            }
        }

        private void UpdateTalentPointsBadge()
        {
            if (this.talents.Points > 0)
            {
                View.HighlightTalentsButton();
            }
            else
            {
                View.UnhighlightTalentsButton();
            }
        }

        private void UpdateAttributePointsBadge()
        {
            if (this.attributes.Points > 0)
            {
                View.HighlightAttributesButton();
            }
            else
            {
                View.UnhighlightAttributesButton();
            }
        }

        private void OnPointsChanged(TalentsComponent talents)
        {
            UpdateTalentPointsBadge();
        }

        private void OnAttributePointsChanged(AttributesComponent attributes)
        {
            UpdateAttributePointsBadge();
        }

        private void OnToggleMail()
        {
            SwitchView(this.mailboxViewController.View);
        }

        private void OnToggleAchievements()
        {
            SwitchView(this.achievementsViewController.View);
        }

        private void OnToggleSpecializations()
        {
            if (Scenario.Active != null)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_in_combat"));
                return;
            }

            SwitchView(this.specializationViewController.View);
        }

        private void OnToggleMasteries()
        {
            SwitchView(this.masteriesViewController.View);
        }

        private void OnToggleAttributes()
        {
            if (Scenario.Active != null)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_in_combat"));
                return;
            }

            SwitchView(this.attributesViewController.View);
        }

        private void OnToggleReliquary()
        {
            if (Scenario.Active != null)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_in_combat"));
                return;
            }

            SwitchView(this.reliquaryViewController.View);
        }

        private void OnToggleEquipment()
        {
            SwitchView(this.equipmentViewController.View);
        }

        private void OnToggleMenu()
        {
            SwitchView(this.menuViewController.View);
        }

        private void OnToggleSkills()
        {
            if (Scenario.Active != null)
            {
                UiErrorFrame.Instance.ShowMessage(I18N.Instance.Get("exception_in_combat"));
                return;
            }

            SwitchView(this.spellbookViewController.View);
        }

        private void OnToggleTalents()
        {
            SwitchView(this.talentsViewController.View);
        }

        private void OnToggleCombatLog()
        {
            SwitchView(this.combatLogViewController.View);
        }

        private void SwitchView(IView view)
        {
            if (this.activeView == view)
            {
                this.activeView.Toggle();
                return;
            }

            this.activeView?.Hide();
            this.activeView = view;
            this.activeView.Show();
        }
    }
}