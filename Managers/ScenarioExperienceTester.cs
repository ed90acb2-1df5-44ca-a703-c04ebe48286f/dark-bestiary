using System.Linq;
using System.Reflection;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class ScenarioExperienceTester : MonoBehaviour
    {
        [SerializeField] private int startingLevel;
        [SerializeField] private int scenarioIndexStart;
        [SerializeField] private int scenarioIndexEnd;

        public void Test()
        {
            var experience = gameObject.AddComponent<ExperienceComponent>();
            experience.Construct(this.startingLevel, ExperienceComponent.RequiredExperienceAtLevel(this.startingLevel));

            var scenarioRepository = Container.Instance.Resolve<IScenarioDataRepository>();
            var unitRepository = Container.Instance.Resolve<IUnitDataRepository>();

            ClearLog();

            foreach (var scenario in scenarioRepository.FindAll()
                .Where(scenario => scenario.Index.InRange(this.scenarioIndexStart, this.scenarioIndexEnd) &&
                                   scenario.Type == ScenarioType.Campaign)
                .OrderBy(scenario => scenario.Index))
            {
                Debug.Log($"----- {I18N.Instance.Get(scenario.NameKey)} -----");

                foreach (var episode in scenario.Episodes)
                {
                    foreach (var unitTableUnit in episode.Encounter.UnitTable.Units)
                    {
                        var unit = unitRepository.Find(unitTableUnit.UnitId);

                        var killExperience = UnitComponent.CalculateKillExperience(
                            experience.Experience.Level, unit.ChallengeRating);

                        experience.Experience.Add(killExperience);
                    }
                }

                Debug.Log("Level: " + (experience.Experience.Level + experience.Experience.GetObtainedFraction()));
            }

            Destroy(experience);
        }

        public void ClearLog()
        {
#if UNITY_EDITOR
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
#endif
        }
    }
}