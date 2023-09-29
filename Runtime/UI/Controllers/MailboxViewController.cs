using System;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class MailboxViewController : ViewController<IMailboxView>
    {
        private const int c_PerPage = 8;

        private readonly Mailbox m_Mailbox;
        private readonly Character m_Character;

        private bool m_IsRefreshing;

        private int m_CurrentPage;
        private int m_TotalPages;

        public MailboxViewController(IMailboxView view, Mailbox mailbox) : base(view)
        {
            m_Character = Game.Instance.Character;
            m_Mailbox = mailbox;
        }

        protected override void OnInitialize()
        {
            m_Mailbox.Updated += OnMailboxUpdated;

            View.Pick += OnPick;
            View.Remove += OnRemove;
            View.TakeAll += OnTakeAll;
            View.NextPage += OnNextPage;
            View.PreviousPage += OnPreviousPage;

            Refresh();
        }

        protected override void OnTerminate()
        {
            m_Mailbox.Updated -= OnMailboxUpdated;

            View.Pick -= OnPick;
            View.Remove -= OnRemove;
            View.NextPage -= OnNextPage;
            View.PreviousPage -= OnPreviousPage;
        }

        private void OnMailboxUpdated()
        {
            if (m_IsRefreshing)
            {
                return;
            }

            m_IsRefreshing = true;

            Timer.Instance.Wait(1, () =>
            {
                // Note: View could be destroyed at this moment.
                if (View == null)
                {
                    return;
                }

                Refresh();
            });
        }

        private void Refresh()
        {
            m_TotalPages = m_Mailbox.Items.Count / c_PerPage;
            m_CurrentPage = Math.Min(m_CurrentPage, m_TotalPages);

            var items = m_Mailbox.Items
                .Skip(m_CurrentPage * c_PerPage)
                .Take(c_PerPage)
                .Select(item =>
                {
                    item.ChangeOwner(m_Character.Entity);
                    return item;
                })
                .ToList();

            View.Refresh(items, m_CurrentPage, m_TotalPages);

            m_IsRefreshing = false;
        }

        private void OnPreviousPage()
        {
            m_CurrentPage = Math.Max(m_CurrentPage - 1, 0);
            Refresh();
        }

        private void OnTakeAll()
        {
            m_Mailbox.TakeAll(m_Character.Entity);
        }

        private void OnNextPage()
        {
            m_CurrentPage = Math.Min(m_CurrentPage + 1, m_TotalPages);
            Refresh();
        }

        private void OnRemove(Item item)
        {
            m_IsRefreshing = true;
            m_Mailbox.RemoveMail(item);
            Refresh();
        }

        private void OnPick(Item item)
        {
            m_IsRefreshing = true;
            m_Mailbox.TakeMail(item, m_Character.Entity);
            Refresh();
        }
    }
}