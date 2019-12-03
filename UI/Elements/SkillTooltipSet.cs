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
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI description;

        public void Construct(GameObject entity, SkillSet set)
        {
            var spellbook = entity.GetComponent<SpellbookComponent>();

            this.title.text = set.Name + $" ({spellbook.GetSkillSetPiecesEquipped(set)})";
            this.icon.sprite = Resources.Load<Sprite>(set.Icon);
            this.description.text = GetDescription(entity, set, spellbook);
        }

        private static string GetDescription(GameObject entity, SkillSet set, SpellbookComponent spellbook)
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

                text += $"({bonus.Key}) {bonus.Value.First().Description.ToString(new StringVariableContext(entity))}\n";
                text += "</color>";
            }

            return text;
        }
    }
}