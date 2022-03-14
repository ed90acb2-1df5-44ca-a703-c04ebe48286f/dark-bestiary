using System;
using DarkBestiary.Data;

namespace DarkBestiary.Skills
{
    public class Specialization
    {
        public SpecializationData Data { get; }
        public Experience Experience { get; private set; }

        public Specialization(SpecializationData data)
        {
            Data = data;
        }

        public void Construct(int level, int experience)
        {
            Experience = new Experience(level, 50, experience, Formula);
        }

        private static int Formula(int level)
        {
            if (level < 2)
            {
                return 0;
            }

            return (int) Math.Round(12 * Math.Pow(level, 2.145f) / 2);
        }
    }
}