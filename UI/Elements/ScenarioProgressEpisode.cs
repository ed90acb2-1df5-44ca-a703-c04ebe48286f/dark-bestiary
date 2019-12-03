using DarkBestiary.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ScenarioProgressEpisode : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image checkmark;

        [Space(10)]
        [Header("Colors")]
        [SerializeField] private Color defaultColor;
        [SerializeField] private Color inactiveColor;

        public void SetCurrent()
        {
            this.background.color = this.defaultColor;
            this.checkmark.color = this.checkmark.color.With(a: 0);
        }

        public void SetCompleted()
        {
            this.background.color = this.defaultColor;
            this.checkmark.color = this.checkmark.color.With(a: 1);
        }

        public void SetInactive()
        {
            this.background.color = this.inactiveColor;
            this.checkmark.color = this.checkmark.color.With(a: 0);
        }
    }
}