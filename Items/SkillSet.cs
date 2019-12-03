using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Skills;

namespace DarkBestiary.Items
{
    public class SkillSet
    {
        public int Id { get; }
        public I18NString Name { get; }
        public string Icon { get; }

        // TODO: Circular dependency ducttape
        public List<Skill> Skills =>
            this.skills ?? (this.skills = Container.Instance.Resolve<ISkillRepository>().Find(this.skillIds));

        public Dictionary<int, List<Behaviour>> Behaviours { get; }

        private List<Skill> skills;
        private readonly List<int> skillIds;

        public SkillSet(SkillSetData data, Dictionary<int, List<Behaviour>> behaviours)
        {
            Id = data.Id;
            Icon = data.Icon;
            Name = I18N.Instance.Get(data.NameKey);
            Behaviours = behaviours;

            this.skillIds = data.Skills;
        }
    }
}