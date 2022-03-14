using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using UnityEngine;

namespace DarkBestiary.Testers
{
    public class SpecializationSkillTester : MonoBehaviour
    {
        public void Test()
        {
            var specializations = Container.Instance.Resolve<ISpecializationDataRepository>().FindAll();
            var skills = new Dictionary<int, int>();

            foreach (var specialization in specializations)
            {
                GetSkillIds(specialization.Skills, skills);
            }

            var sets = Container.Instance.Resolve<ISkillSetRepository>().FindAll();

            foreach (var set in sets)
            {
                foreach (var skillId in set.SkillIds)
                {
                    if (skills.ContainsKey(skillId))
                    {
                        continue;
                    }

                    Debug.Log(skillId.ToString());
                }
            }
        }

        private void GetSkillIds(List<SpecializationSkillData> skills, Dictionary<int, int> dictionary)
        {
            foreach (var skill in skills)
            {
                dictionary.Add(skill.SkillId, skill.SkillId);
                GetSkillIds(skill.Skills, dictionary);
            }
        }
    }
}