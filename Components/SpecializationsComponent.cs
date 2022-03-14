using System.Collections.Generic;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;

namespace DarkBestiary.Components
{
    public class SpecializationsComponent : Component
    {
        public event Payload<SpecializationsComponent> SkillPointsChanged;

        public List<Specialization> Specializations { get; private set; }

        public int SkillPoints
        {
            get => this.skillPoints;
            set
            {
                this.skillPoints = value;
                SkillPointsChanged?.Invoke(this);
            }
        }

        private int skillPoints;

        public SpecializationsComponent Construct(List<Specialization> specializations, int skillPoints)
        {
            Specializations = specializations;
            SkillPoints = skillPoints;
            return this;
        }

        protected override void OnInitialize()
        {
        }

        protected override void OnTerminate()
        {
        }
    }
}