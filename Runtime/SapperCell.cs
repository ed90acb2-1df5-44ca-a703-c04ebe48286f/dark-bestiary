using System;
using TMPro;
using UnityEngine;

namespace DarkBestiary
{
    public class SapperCell : MonoBehaviour
    {
        public event Action<SapperCell> Opened;

        public bool IsBomb { get; set; }
        public bool IsOpened { get; private set; }
        public bool IsMarked => m_Marker.gameObject.activeSelf;
        public int BombsNearby { get; set; }

        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private SpriteRenderer m_Background;
        [SerializeField] private SpriteRenderer m_Marker;
        [SerializeField] private GameObject m_Explosion;

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

            m_Text.text = BombsNearby == 0 || IsBomb ? "" : BombsNearby.ToString();
            m_Background.gameObject.SetActive(false);
        }

        public void Close()
        {
            IsOpened = false;

            m_Text.text = "";
            m_Background.gameObject.SetActive(true);
        }

        public void ToggleMarker()
        {
            SetMarked(!m_Marker.gameObject.activeSelf);
        }

        public void SetMarked(bool isMarked)
        {
            m_Marker.gameObject.SetActive(isMarked);
        }
    }
}