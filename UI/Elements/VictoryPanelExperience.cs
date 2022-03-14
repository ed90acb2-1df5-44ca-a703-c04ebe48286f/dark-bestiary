using System;
using DarkBestiary.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VictoryPanelExperience : MonoBehaviour
    {
        private const float SimulationTime = 4;

        [SerializeField] private Animator animator;
        [SerializeField] private TextMeshProUGUI currentLevelText;
        [SerializeField] private TextMeshProUGUI nextLevelText;
        [SerializeField] private TextMeshProUGUI experienceText;
        [SerializeField] private TextMeshProUGUI experienceGainText;
        [SerializeField] private Image filler;

        private Experience experience;
        private int level;
        private int gain;
        private float start;
        private float current;
        private float obtained;
        private float toObtain;
        private float time;

        private bool simulate;

        public void Construct(Experience experience, int skillPoints = 0)
        {
            this.experience = experience;
            this.level = experience.Level;
            this.gain = experience.Pending;

            this.obtained = experience.GetObtained();
            this.toObtain = experience.GetRequired();
            this.current = experience.Current;
            this.start = experience.Current;

            this.experienceGainText.text = $"{I18N.Instance.Translate("ui_experience")}: +{this.gain.ToString()}";

            if (skillPoints > 0)
            {
                this.experienceGainText.text = $"{I18N.Instance.Translate("ui_skill_points")}: +{skillPoints.ToString()}\n{this.experienceGainText.text}";
            }

            UpdateBar();
        }

        public void Simulate()
        {
            this.simulate = this.experience.Level < this.experience.MaxLevel;
        }

        private void UpdateBar()
        {
            var fraction = this.obtained / this.toObtain;

            this.filler.fillAmount = fraction;
            this.currentLevelText.text = Math.Min(this.level, this.experience.MaxLevel).ToString();
            this.nextLevelText.text = Math.Min(this.level + 1, this.experience.MaxLevel).ToString();
            this.experienceText.text = $"{this.obtained:F0} / {this.toObtain:F0} ({fraction:0%})";
        }

        private void LevelUp()
        {
            this.level++;

            this.toObtain = this.experience.RequiredAtLevel(this.level + 1) -
                            this.experience.RequiredAtLevel(this.level);

            this.obtained = this.current - this.experience.RequiredAtLevel(this.level);

            this.animator.Play("levelup");

            if (this.level == this.experience.MaxLevel)
            {
                this.simulate = false;
                this.obtained = this.toObtain;
            }
        }

        private void Update()
        {
            if (!this.simulate || this.time >= SimulationTime)
            {
                return;
            }

            this.time = Mathf.Min(this.time + Time.deltaTime, SimulationTime);

            var previous = this.current;
            this.current = this.start + this.gain * Curves.Instance.Logarithmic.Evaluate(this.time / SimulationTime);
            this.obtained += this.current - previous;

            if (this.obtained >= this.toObtain)
            {
                LevelUp();
            }

            UpdateBar();
        }
    }
}