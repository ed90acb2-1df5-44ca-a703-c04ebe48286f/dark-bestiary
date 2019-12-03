using System;
using System.Linq;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public class MailboxViewController : ViewController<IMailboxView>
    {
        private const int PerPage = 8;

        private readonly Mailbox mailbox;
        private readonly Character character;

        private bool isRefreshing;

        private int currentPage;
        private int totalPages;

        public MailboxViewController(IMailboxView view, CharacterManager characterManager, Mailbox mailbox) : base(view)
        {
            this.character = characterManager.Character;
            this.mailbox = mailbox;
        }

        protected override void OnInitialize()
        {
            this.mailbox.MailSent += OnMailboxUpdated;
            this.mailbox.MailTaken += OnMailboxUpdated;
            this.mailbox.MailRemoved += OnMailboxUpdated;

            View.Pick += OnPick;
            View.Remove += OnRemove;
            View.NextPage += OnNextPage;
            View.PreviousPage += OnPreviousPage;
        }

        protected override void OnViewInitialized()
        {
            Refresh();
        }

        protected override void OnTerminate()
        {
            this.mailbox.MailSent -= OnMailboxUpdated;
            this.mailbox.MailTaken -= OnMailboxUpdated;
            this.mailbox.MailRemoved -= OnMailboxUpdated;

            View.Pick -= OnPick;
            View.Remove -= OnRemove;
            View.NextPage -= OnNextPage;
            View.PreviousPage -= OnPreviousPage;
        }

        private void OnMailboxUpdated(Item item)
        {
            if (this.isRefreshing)
            {
                return;
            }

            this.isRefreshing = true;

            Timer.Instance.Wait(1, Refresh);
        }

        private void Refresh()
        {
            if (IsTerminated)
            {
                return;
            }

            this.isRefreshing = false;

            this.totalPages = this.mailbox.Items.Count / PerPage;
            this.currentPage = Math.Min(this.currentPage, this.totalPages);

            var items = this.mailbox.Items
                .Skip(this.currentPage * PerPage)
                .Take(PerPage)
                .Select(item =>
                {
                    item.ChangeOwner(this.character.Entity);
                    return item;
                })
                .ToList();

            View.Display(items, this.currentPage, this.totalPages);
        }

        private void OnPreviousPage()
        {
            this.currentPage = Math.Max(this.currentPage - 1, 0);
            Refresh();
        }

        private void OnNextPage()
        {
            this.currentPage = Math.Min(this.currentPage + 1, this.totalPages);
            Refresh();
        }

        private void OnRemove(Item item)
        {
            this.isRefreshing = true;
            this.mailbox.RemoveMail(item);
            Refresh();
        }

        private void OnPick(Item item)
        {
            this.isRefreshing = true;
            this.mailbox.TakeMail(item, this.character.Entity);
            Refresh();
        }
    }
}