using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.UI.Views.Unity;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ObjectTooltip : Singleton<ObjectTooltip>
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private TextMeshProUGUI info;

        private void Start()
        {
            Instance.Hide();
            GameState.AnyGameStateExit += OnAnyGameStateExit;
            View.AnyViewShowing += OnAnyViewShowing;
        }

        private void OnAnyViewShowing(View view)
        {
            Hide();
        }

        private void OnAnyGameStateExit(GameState gameState)
        {
            Hide();
        }

        public void Show(string text, string info)
        {
            Show(text, info, Input.mousePosition);
        }

        public void Show(string text, string info, Vector3 position)
        {
            gameObject.SetActive(true);

            transform.position = position;
            GetComponent<RectTransform>().ClampPositionToParent();

            if (!string.IsNullOrEmpty(text))
            {
                this.text.text = text;
                this.text.gameObject.SetActive(true);
            }
            else
            {
                this.text.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(info))
            {
                this.info.text = info;
                this.info.gameObject.SetActive(true);
            }
            else
            {
                this.info.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}