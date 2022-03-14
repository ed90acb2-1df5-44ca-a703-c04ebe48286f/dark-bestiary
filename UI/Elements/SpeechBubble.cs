using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DarkBestiary.UI.Elements
{
    public class SpeechBubble : Singleton<SpeechBubble>, IPointerUpHandler
    {
        public event Payload Hidden;

        [SerializeField] private TextMeshProUGUI text;

        private readonly Vector3 offset = new Vector3(0, 2, 0);

        private Camera mainCamera;
        private Transform target;
        private bool isTemporary;
        private float duration;

        private void Start()
        {
            Instance.Hide();
        }

        public void Show(string text, Transform target, float duration)
        {
            this.isTemporary = true;
            this.duration = duration;

            OnShow(text, target);
        }

        public void Show(string text, Transform target)
        {
            this.isTemporary = false;

            OnShow(text, target);
        }

        private void OnShow(string text, Transform target)
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);

            this.mainCamera = Camera.main;
            this.text.text = text;
            this.target = target;

            Update();
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
            {
                return;
            }

            gameObject.SetActive(false);
            Hidden?.Invoke();
        }

        private void Update()
        {
            transform.position = this.mainCamera.WorldToScreenPoint(this.target.position + this.offset);

            if (!this.isTemporary)
            {
                return;
            }

            this.duration -= Time.deltaTime;

            if (this.duration <= 0)
            {
                this.isTemporary = false;
                this.duration = 0;
                Hide();
            }
        }

        public void OnPointerUp(PointerEventData pointer)
        {
            Hide();
        }
    }
}