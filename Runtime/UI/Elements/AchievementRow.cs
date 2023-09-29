using DarkBestiary.Achievements;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class AchievementRow : MonoBehaviour
    {
        [SerializeField] private Image m_FadeImage;
        [SerializeField] private Image m_ProgressImage;
        [SerializeField] private TextMeshProUGUI m_ProgressText;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DescriptionText;
        [SerializeField] private Image m_Icon;

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
            m_NameText.text = Achievement.Name;
            m_DescriptionText.text = Achievement.Description;
            m_Icon.sprite = Resources.Load<Sprite>(Achievement.Icon);

            m_ProgressImage.fillAmount = (float) Achievement.Quantity / Achievement.RequiredQuantity;
            m_ProgressText.text = $"{Achievement.Quantity}/{Achievement.RequiredQuantity}";

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
            m_FadeImage.color = new Color(0, 0, 0, 0.5f);
        }

        private void Unlock()
        {
            m_FadeImage.color = new Color(0, 0, 0, 0f);
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