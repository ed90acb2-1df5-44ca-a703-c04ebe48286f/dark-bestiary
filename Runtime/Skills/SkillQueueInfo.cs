namespace DarkBestiary.Skills
{
    public struct SkillQueueInfo
    {
        public Skill Skill { get; }
        public object Target { get; }

        public SkillQueueInfo(Skill skill, object target)
        {
            Skill = skill;
            Target = target;
        }
    }
}