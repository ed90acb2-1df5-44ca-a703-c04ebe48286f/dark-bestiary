using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;

namespace DarkBestiary
{
    public class SapperCell : MonoBehaviour
    {
        public event Payload<SapperCell> Opened;

        public bool IsBomb { get; set; }
        public bool IsOpened { get; private set; }
        public bool IsMarked => this.marker.gameObject.activeSelf;
        public int BombsNearby { get; set; }

        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer marker;
        [SerializeField] private GameObject explosion;

        public void Open()
        {
            if (IsOpened)
            {
                return;
            }

            OpenSilently();

            Opened?.Invoke(this);
        }

        public void OpenSilently()
        {
            IsOpened = true;

            this.text.text = BombsNearby == 0 || IsBomb ? "" : BombsNearby.ToString();
            this.background.gameObject.SetActive(false);
        }

        public void Close()
        {
            IsOpened = false;

            this.text.text = "";
            this.background.gameObject.SetActive(true);
        }

        public void ToggleMarker()
        {
            SetMarked(!this.marker.gameObject.activeSelf);
        }

        public void SetMarked(bool isMarked)
        {
            this.marker.gameObject.SetActive(isMarked);
        }
    }
}