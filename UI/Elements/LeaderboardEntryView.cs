using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Leaderboards;
using DarkBestiary.Messaging;
using Pathfinding.Util;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class LeaderboardEntryView : MonoBehaviour, IPointerClickHandler
    {
        public event Payload<LeaderboardEntryView> Clicked;

        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI depthText;
        [SerializeField] private RawImage avatarImage;
        [SerializeField] private LeaderboardEntrySkillView skillPrefab;
        [SerializeField] private Transform skillContainer;

        public ILeaderboardEntry Entry
        {
            get;
            private set;
        }

        public void Construct(ILeaderboardEntry entry)
        {
            Entry = entry;

            this.avatarImage.texture = entry.GetAvatar();
            this.rankText.text = $"#{entry.GetRank().ToString()}";
            this.nameText.text = entry.GetName();
            this.depthText.text = entry.GetScore().ToString();

            var skillDataRepository = Container.Instance.Resolve<ISkillDataRepository>();

            foreach (var skillId in entry.GetSkills())
            {
                var skill = skillDataRepository.Find(skillId);
                Instantiate(this.skillPrefab, this.skillContainer).Construct(skill);
            }
        }

        public void OnPointerClick(PointerEventData pointer)
        {
            Clicked?.Invoke(this);
        }
    }
}