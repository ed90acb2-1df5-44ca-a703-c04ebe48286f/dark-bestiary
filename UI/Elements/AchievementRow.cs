using DarkBestiary.Achievements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AchievementRow : MonoBehaviour
    {
        [SerializeField] private Image fadeImage;
        [SerializeField] private Image progressImage;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image icon;

        public Achievement Achievement { get; private set; }

        public void Initialize(Achievement achievement)
        {
            Achievement = achievement;
            Achievement.Unlocked += OnAchievementUnlocked;
            Achievement.Updated += OnAchievementUpdated;

            Refresh();
        }

        public void Terminate()
        {
            Achievement.Unlocked -= OnAchievementUnlocked;
            Achievement.Updated -= OnAchievementUpdated;
        }

        private void Refresh()
        {
            this.nameText.text = Achievement.Name;
            this.descriptionText.text = Achievement.Description;
            this.icon.sprite = Resources.Load<Sprite>(Achievement.Icon);

            this.progressImage.fillAmount = (float) Achievement.Quantity / Achievement.RequiredQuantity;
            this.progressText.text = $"{Achievement.Quantity}/{Achievement.RequiredQuantity}";

            if (Achievement.IsUnlocked)
            {
                Unlock();
            }
            else
            {
                Lock();
            }
        }

        private void Lock()
        {
            this.fadeImage.color = new Color(0, 0, 0, 0.5f);
        }

        private void Unlock()
        {
            this.fadeImage.color = new Color(0, 0, 0, 0f);
            transform.SetAsFirstSibling();
        }

        private void OnAchievementUpdated(Achievement achievement)
        {
            Refresh();
        }

        private void OnAchievementUnlocked(Achievement achievement)
        {
            Unlock();
        }
    }
}