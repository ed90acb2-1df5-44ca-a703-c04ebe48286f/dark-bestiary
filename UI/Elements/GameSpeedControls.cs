using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class GameSpeedControls : MonoBehaviour
    {
        private static float timeScale = 1;

        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Interactable changeButton;
        [SerializeField] private Interactable resetButton;

        private readonly LinkedList<float> modes = new LinkedList<float>();

        private void Start()
        {
            this.changeButton.PointerUp += OnChangeButtonPointerUp;
            this.resetButton.PointerUp += OnResetButtonPointerUp;

            ResetModes();
            ChangeTimeScale(timeScale);
        }

        private void ResetModes()
        {
            this.modes.Clear();
            this.modes.AddLast(1f);
            this.modes.AddLast(1.25f);
            this.modes.AddLast(1.5f);
            this.modes.AddLast(2f);
        }

        private void OnResetButtonPointerUp()
        {
            ResetModes();
            ChangeTimeScale(this.modes.First.Value);
        }

        private void OnChangeButtonPointerUp()
        {
            var first = this.modes.First;
            this.modes.RemoveFirst();
            this.modes.AddLast(first);

            ChangeTimeScale(this.modes.First.Value);
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
    }
}