using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class GameSpeedButtonController : Interactable
    {
        private static float s_TimeScale = 1;

        [SerializeField] private TextMeshProUGUI m_Label;

        private readonly float[] m_TimeScaleOptions = new float[5];

        private int m_TimeScaleIndex;

        private void Start()
        {
            ResetModes();
            Timer.Instance.Wait(1, () => ChangeTimeScale(s_TimeScale));
        }

        private void ResetModes()
        {
            m_TimeScaleOptions[0] = 1f;
            m_TimeScaleOptions[1] = 1.25f;
            m_TimeScaleOptions[2] = 1.5f;
            m_TimeScaleOptions[3] = 2f;
            m_TimeScaleOptions[4] = 3f;
        }

        protected override void OnPointerEnter()
        {
            Tooltip.Instance.Show(I18N.Instance.Get("ui_game_speed"), GetComponent<RectTransform>());
        }

        protected override void OnPointerExit()
        {
            Tooltip.Instance.Hide();
        }

        protected override void OnPointerClick()
        {
            m_TimeScaleIndex++;

            if (m_TimeScaleIndex >= m_TimeScaleOptions.Length)
            {
                m_TimeScaleIndex = 0;
            }

            ChangeTimeScaleIndex(m_TimeScaleIndex);
        }

        private void IncreaseGameSpeed()
        {
            ChangeTimeScaleIndex(m_TimeScaleIndex + 1);
        }

        private void DecreaseGameSpeed()
        {
            ChangeTimeScaleIndex(m_TimeScaleIndex - 1);
        }

        private void ChangeTimeScaleIndex(int index)
        {
            m_TimeScaleIndex = Mathf.Clamp(index, 0, m_TimeScaleOptions.Length - 1);
            ChangeTimeScale(m_TimeScaleOptions[m_TimeScaleIndex]);
        }

        private void ChangeTimeScale(float value)
        {
            s_TimeScale = value;
            Time.timeScale = s_TimeScale;
            m_Label.text = $"x{s_TimeScale:F2}";
        }

        private void OnDestroy()
        {
            Time.timeScale = 1;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyBindings.Get(KeyType.IncreaseGameSpeed)))
            {
                IncreaseGameSpeed();
            }

            if (Input.GetKeyDown(KeyBindings.Get(KeyType.DecreaseGameSpeed)))
            {
                DecreaseGameSpeed();
            }
        }
    }
}