using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class NavigationViewController : ViewController<INavigationView>
    {
        private readonly TalentsComponent talents;
        private readonly AttributesComponent attributes;

        private CombatLogViewController combatLogViewController;
        private MasteriesViewController masteriesViewController;
        private EquipmentViewController equipmentViewController;
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
        }

        protected override void OnInitialize()
        {
            View.ToggleAchievements += OnToggleAchievements;
            View.ToggleMasteries += OnToggleMasteries;
            View.ToggleAttributes += OnToggleAttributes;
            View.ToggleCombatLog += OnToggleCombatLog;
            View.ToggleEquipment += OnToggleEquipment;
            View.ToggleMail += OnToggleMail;
            View.ToggleMenu += OnToggleMenu;
            View.ToggleSkills += OnToggleSkills;
            View.ToggleTalents += OnToggleTalents;

            Mailbox.Instance.MailSent += OnMailboxUpdated;
            Mailbox.Instance.MailRemoved += OnMailboxUpdated;
            OnMailboxUpdated(null);

            this.attributes.PointsChanged += OnAttributePointsChanged;
            UpdateAttributePointsBadge();

            this.talents.PointsChanged += OnPointsChanged;
            UpdateTalentPointsBadge();

            this.masteriesViewController = Container.Instance.Instantiate<MasteriesViewController>();
            this.masteriesViewController.Initialize();
            this.masteriesViewController.View.Hide();

            this.achievementsViewController = Container.Instance.Instantiate<AchievementsViewController>();
            this.achievementsViewController.Initialize();
            this.achievementsViewController.View.Hide();

            this.attributesViewController = Container.Instance.Instantiate<AttributesViewController>();
            this.attributesViewController.Initialize();
            this.attributesViewController.View.Hide();

            this.combatLogViewController = Container.Instance.Instantiate<CombatLogViewController>();
            this.combatLogViewController.Initialize();
            this.combatLogViewController.View.Hide();

            this.equipmentViewController = Container.Instance.Instantiate<EquipmentViewController>();
            this.equipmentViewController.Initialize();
            this.equipmentViewController.View.Hide();

            this.mailboxViewController = Container.Instance.Instantiate<MailboxViewController>();
            this.mailboxViewController.Initialize();
            this.mailboxViewController.View.Hide();

            this.menuViewController = Container.Instance.Instantiate<MenuViewController>();
            this.menuViewController.Initialize();
            this.menuViewController.View.Hide();

            this.spellbookViewController = Container.Instance.Instantiate<SpellbookViewController>();
            this.spellbookViewController.Initialize();
            this.spellbookViewController.View.Hide();

            this.talentsViewController = Container.Instance.Instantiate<TalentsViewController>();
            this.talentsViewController.Initialize();
            this.talentsViewController.View.Hide();
        }

        protected override void OnTerminate()
        {
            View.ToggleAchievements -= OnToggleAchievements;
            View.ToggleMasteries -= OnToggleMasteries;
            View.ToggleAttributes -= OnToggleAttributes;
            View.ToggleCombatLog -= OnToggleCombatLog;
            View.ToggleEquipment -= OnToggleEquipment;
            View.ToggleMail -= OnToggleMail;
            View.ToggleMenu -= OnToggleMenu;
            View.ToggleSkills -= OnToggleSkills;
            View.ToggleTalents -= OnToggleTalents;

            Mailbox.Instance.MailSent -= OnMailboxUpdated;
            Mailbox.Instance.MailRemoved -= OnMailboxUpdated;

            this.attributes.PointsChanged -= OnAttributePointsChanged;
            this.talents.PointsChanged -= OnPointsChanged;

            this.masteriesViewController.Terminate();
            this.achievementsViewController.Terminate();
            this.attributesViewController.Terminate();
            this.combatLogViewController.Terminate();
            this.equipmentViewController.Terminate();
            this.mailboxViewController.Terminate();
            this.menuViewController.Terminate();
            this.spellbookViewController.Terminate();
            this.talentsViewController.Terminate();
        }

        private void OnMailboxUpdated(Item item)
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

        private void OnToggleMasteries()
        {
            SwitchView(this.masteriesViewController.View);
        }

        private void OnToggleAttributes()
        {
            SwitchView(this.attributesViewController.View);
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