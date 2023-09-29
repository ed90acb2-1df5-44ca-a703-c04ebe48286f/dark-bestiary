using DarkBestiary.Extensions;
using DarkBestiary.UI.Views;
using DarkBestiary.UI.Views.Unity;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ObjectTooltip : Singleton<ObjectTooltip>
    {
        [SerializeField]
        private TextMeshProUGUI m_Text = null!;

        [SerializeField]
        private TextMeshProUGUI m_Info = null!;

        private void Start()
        {
            Instance.Hide();
            Game.Instance.SceneSwitched += OnSceneSwitched;
            View.AnyViewShown += OnAnyViewShown;
        }

        private void OnAnyViewShown(IView view)
        {
            Hide();
        }

        private void OnSceneSwitched()
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
                m_Text.text = text;
                m_Text.gameObject.SetActive(true);
            }
            else
            {
                m_Text.gameObject.SetActive(false);
            }

            if (!string.IsNullOrEmpty(info))
            {
                m_Info.text = info;
                m_Info.gameObject.SetActive(true);
            }
            else
            {
                m_Info.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}