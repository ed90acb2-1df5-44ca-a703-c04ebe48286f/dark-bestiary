using System.Linq;
using DarkBestiary.GameBoard;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Scenarios.Scenes
{
    [RequireComponent(typeof(Collider2D))]
    public class Door : MonoBehaviour
    {
        public event Payload<GameObject> Entered;

        [SerializeField] private bool disableCell;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite opened;
        [SerializeField] private Sprite closed;

        private BoardCell cell;
        private bool isOpened;

        public void Open()
        {
            this.isOpened = true;
            this.spriteRenderer.sprite = this.opened;

            if (this.disableCell)
            {
                EnableCell();
            }

            GetComponent<Collider2D>().enabled = true;
        }

        public void Close()
        {
            this.isOpened = false;
            this.spriteRenderer.sprite = this.closed;

            if (this.disableCell)
            {
                DisableCell();
            }

            GetComponent<Collider2D>().enabled = false;
        }

        public void EnableCell()
        {
            if (!Board.Instance.gameObject.activeSelf)
            {
                return;
            }

            GetCell().gameObject.SetActive(true);
        }

        public void DisableCell()
        {
            if (!Board.Instance.gameObject.activeSelf)
            {
                return;
            }

            GetCell().gameObject.SetActive(false);
        }

        private BoardCell GetCell()
        {
            if (this.cell == null)
            {
                this.cell = BoardNavigator.Instance
                    .WithinCircle(transform.position, 1)
                    .OrderBy(c => (c.transform.position - transform.position).sqrMagnitude)
                    .First();
            }

            return this.cell;
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (!this.isOpened)
            {
                return;
            }

            Entered?.Invoke(collider.gameObject);
        }
    }
}