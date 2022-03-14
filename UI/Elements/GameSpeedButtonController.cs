using DarkBestiary.Managers;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class GameSpeedButtonController : Interactable
    {
        private static float timeScale = 1;

        [SerializeField] private TextMeshProUGUI label;

        private readonly float[] timeScaleOptions = new float[5];

        private int timeScaleIndex;

        private void Start()
        {
            ResetModes();
            Timer.Instance.Wait(1, () => ChangeTimeScale(timeScale));
        }

        private void ResetModes()
        {
            this.timeScaleOptions[0] = 1f;
            this.timeScaleOptions[1] = 1.25f;
            this.timeScaleOptions[2] = 1.5f;
            this.timeScaleOptions[3] = 2f;
            this.timeScaleOptions[4] = 3f;
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
            this.timeScaleIndex++;

            if (this.timeScaleIndex >= this.timeScaleOptions.Length)
            {
                this.timeScaleIndex = 0;
            }

            ChangeTimeScaleIndex(this.timeScaleIndex);
        }

        private void IncreaseGameSpeed()
        {
            ChangeTimeScaleIndex(this.timeScaleIndex + 1);
        }

        private void DecreaseGameSpeed()
        {
            ChangeTimeScaleIndex(this.timeScaleIndex - 1);
        }

        private void ChangeTimeScaleIndex(int index)
        {
            this.timeScaleIndex = Mathf.Clamp(index, 0, this.timeScaleOptions.Length - 1);
            ChangeTimeScale(this.timeScaleOptions[this.timeScaleIndex]);
        }

        private void ChangeTimeScale(float value)
        {
            timeScale = value;
            Time.timeScale = timeScale;
            this.label.text = $"x{timeScale:F2}";
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