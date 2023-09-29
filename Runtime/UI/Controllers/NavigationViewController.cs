using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class NavigationViewController : ViewController<INavigationView>
    {
        private readonly TalentsComponent m_Talents;
        private readonly AttributesComponent m_Attributes;
        private readonly ReliquaryComponent m_Reliquary;

        private IView? m_ActiveView;

        public NavigationViewController(INavigationView view) : base(view)
        {
            m_Talents = Game.Instance.Character.Entity.GetComponent<TalentsComponent>();
            m_Attributes = Game.Instance.Character.Entity.GetComponent<AttributesComponent>();
            m_Reliquary = Game.Instance.Character.Entity.GetComponent<ReliquaryComponent>();
        }

        protected override void OnInitialize()
        {
            View.ToggleAchievements += OnToggleAchievements;
            View.ToggleMasteries += OnToggleMasteries;
            View.ToggleAttributes += OnToggleAttributes;
            View.ToggleCombatLog += OnToggleCombatLog;
            View.ToggleDamageMeter += OnToggleDamageMeter;
            View.ToggleEquipment += OnToggleEquipment;
            View.ToggleReliquary += OnToggleReliquary;
            View.ToggleMail += OnToggleMail;
            View.ToggleMenu += OnToggleMenu;
            View.ToggleTalents += OnToggleTalents;

            Mailbox.Instance.Updated += OnMailboxUpdated;
            OnMailboxUpdated();

            m_Reliquary.Unlocked += OnReliquaryUpdated;
            m_Reliquary.Equipped += OnReliquaryUpdated;
            m_Reliquary.Unequipped += OnReliquaryUpdated;
            OnReliquaryUpdated(null);

            m_Attributes.PointsChanged += OnAttributePointsChanged;
            UpdateAttributePointsBadge();

            m_Talents.PointsChanged += OnPointsChanged;
            UpdateTalentPointsBadge();
        }

        protected override void OnTerminate()
        {
            View.ToggleAchievements -= OnToggleAchievements;
            View.ToggleMasteries -= OnToggleMasteries;
            View.ToggleAttributes -= OnToggleAttributes;
            View.ToggleCombatLog -= OnToggleCombatLog;
            View.ToggleDamageMeter -= OnToggleDamageMeter;
            View.ToggleEquipment -= OnToggleEquipment;
            View.ToggleReliquary -= OnToggleReliquary;
            View.ToggleMail -= OnToggleMail;
            View.ToggleMenu -= OnToggleMenu;
            View.ToggleTalents -= OnToggleTalents;

            Mailbox.Instance.Updated -= OnMailboxUpdated;

            m_Reliquary.Unlocked -= OnReliquaryUpdated;
            m_Reliquary.Equipped -= OnReliquaryUpdated;
            m_Reliquary.Unequipped -= OnReliquaryUpdated;

            m_Attributes.PointsChanged -= OnAttributePointsChanged;

            m_Talents.PointsChanged -= OnPointsChanged;
        }

        private void OnReliquaryUpdated(Relic _)
        {
            if (m_Reliquary.Slots.Any(slot => slot.IsEmpty) &&
                m_Reliquary.Available.Any(relic => !relic.IsEquipped))
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
            if (m_Talents.Points > 0)
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
            if (m_Attributes.Points > 0)
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
            Game.Instance.SwitchView<MailboxViewController>();
        }

        private void OnToggleAchievements()
        {
            Game.Instance.SwitchView<AchievementsViewController>();
        }

        private void OnToggleMasteries()
        {
            Game.Instance.SwitchView<MasteriesViewController>();
        }

        private void OnToggleAttributes()
        {
            Game.Instance.SwitchView<AttributesViewController>();
        }

        private void OnToggleReliquary()
        {
            Game.Instance.SwitchView<ReliquaryViewController>();
        }

        private void OnToggleEquipment()
        {
            Game.Instance.SwitchView<EquipmentViewController>();
        }

        private void OnToggleMenu()
        {
            Game.Instance.SwitchView<MenuViewController>();
        }

        private void OnToggleTalents()
        {
            Game.Instance.SwitchView<TalentsViewController>();
        }

        private void OnToggleCombatLog()
        {
            Game.Instance.SwitchView<CombatLogViewController>();
        }

        private void OnToggleDamageMeter()
        {
            Game.Instance.SwitchView<DamageMeterViewController>();
        }
    }
}