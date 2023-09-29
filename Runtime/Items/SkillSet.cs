using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;

namespace DarkBestiary.Items
{
    public class SkillSet
    {
        public int Id { get; }
        public I18NString Name { get; }
        public string Icon { get; }
        public List<int> SkillIds { get; }
        public Dictionary<int, List<Behaviour>> Behaviours { get; }

        public SkillSet(SkillSetData data, Dictionary<int, List<Behaviour>> behaviours)
        {
            Id = data.Id;
            Icon = data.Icon;
            Name = I18N.Instance.Get(data.NameKey);
            Behaviours = behaviours;
            SkillIds = data.Skills;
        }
    }
}