using UnityEngine;

namespace DarkBestiary.Skills
{
    public struct SkillUseEventData
    {
        public GameObject Caster { get; }
        public object Target { get; }
        public Skill Skill { get; }

        public SkillUseEventData(GameObject caster, object target, Skill skill)
        {
            Caster = caster;
            Target = target;
            Skill = skill;
        }
    }
}