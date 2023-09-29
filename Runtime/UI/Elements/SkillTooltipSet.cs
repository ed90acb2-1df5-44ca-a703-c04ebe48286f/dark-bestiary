using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class SkillTooltipSet : PoolableMonoBehaviour
    {
        [SerializeField]
        private Image m_Icon = null!;

        [SerializeField]
        private TextMeshProUGUI m_Title = null!;

        [SerializeField]
        private TextMeshProUGUI m_Description = null!;

        public void Construct(SkillSet set, SpellbookComponent spellbook)
        {
            m_Title.text = set.Name + $" ({spellbook.GetSkillSetPiecesEquipped(set)})";
            m_Icon.sprite = Resources.Load<Sprite>(set.Icon);
            m_Description.text = GetDescription(set, spellbook);
        }

        private static string GetDescription(SkillSet set, SpellbookComponent spellbook)
        {
            var pieces = spellbook.GetSkillSetPiecesEquipped(set);

            var bestAvailable = set.Behaviours
                .Where(b => b.Key <= pieces)
                .OrderByDescending(b => b.Key)
                .FirstOrDefault();

            var text = "";

            foreach (var bonus in set.Behaviours)
            {
                if (bestAvailable.Key == bonus.Key)
                {
                    text += "<color=white>";
                }
                else
                {
                    text += "<color=#555>";
                }

                text += $"({bonus.Key}) {bonus.Value.First().Description.ToString(new StringVariableContext(spellbook.gameObject))}\n";
                text += "</color>";
            }

            return text;
        }
    }
}