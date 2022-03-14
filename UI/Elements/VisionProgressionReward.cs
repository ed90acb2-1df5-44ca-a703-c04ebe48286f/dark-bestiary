using DarkBestiary.Talents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class VisionProgressionReward : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        public void Construct(Talent talent)
        {
            this.icon.sprite = Resources.Load<Sprite>(talent.Icon);
            this.nameText.text = talent.Name;
            this.descriptionText.text = talent.Description;
        }
    }
}