using System;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Leaderboards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryView : MonoBehaviour, IPointerClickHandler
    {
        public event Action<LeaderboardEntryView> Clicked;

        [SerializeField] private TextMeshProUGUI m_RankText;
        [SerializeField] private TextMeshProUGUI m_NameText;
        [SerializeField] private TextMeshProUGUI m_DepthText;
        [SerializeField] private RawImage m_AvatarImage;
        [SerializeField] private LeaderboardEntrySkillView m_SkillPrefab;
        [SerializeField] private Transform m_SkillContainer;

        public ILeaderboardEntry Entry
        {
            get;
            private set;
        }

        public void Construct(ILeaderboardEntry entry)
        {
            Entry = entry;

            m_AvatarImage.texture = entry.GetAvatar();
            m_RankText.text = $"#{entry.GetRank().ToString()}";
            m_NameText.text = entry.GetName();
            m_DepthText.text = entry.GetScore().ToString();

            var skillDataRepository = Container.Instance.Resolve<ISkillDataRepository>();

            foreach (var skillId in entry.GetSkills())
            {
                var skill = skillDataRepository.Find(skillId);
                Instantiate(m_SkillPrefab, m_SkillContainer).Construct(skill);
            }
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}