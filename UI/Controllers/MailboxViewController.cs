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
            this.mailbox.Updated += OnMailboxUpdated;

            View.Pick += OnPick;
            View.Remove += OnRemove;
            View.TakeAll += OnTakeAll;
            View.NextPage += OnNextPage;
            View.PreviousPage += OnPreviousPage;

            Refresh();
        }

        protected override void OnTerminate()
        {
            this.mailbox.Updated -= OnMailboxUpdated;

            View.Pick -= OnPick;
            View.Remove -= OnRemove;
            View.NextPage -= OnNextPage;
            View.PreviousPage -= OnPreviousPage;
        }

        private void OnMailboxUpdated()
        {
            if (this.isRefreshing)
            {
                return;
            }

            this.isRefreshing = true;

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

            View.Refresh(items, this.currentPage, this.totalPages);

            this.isRefreshing = false;
        }

        private void OnPreviousPage()
        {
            this.currentPage = Math.Max(this.currentPage - 1, 0);
            Refresh();
        }

        private void OnTakeAll()
        {
            this.mailbox.TakeAll(this.character.Entity);
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