using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary.Testers
{
    public class RandomEncounterTester : MonoBehaviour
    {

        [Space(20)]
        [SerializeField] private int m_ChallengeRating;
        [SerializeField] private int m_EnvironmentId;

        public List<UnitData> Test()
        {
            var units = Container.Instance.Resolve<IUnitDataRepository>().FindAll()
                .Where(unit => m_EnvironmentId == 0 || unit.Environment.Id == m_EnvironmentId)
                .Where(unit => !unit.Flags.HasFlag(UnitFlags.Boss) &&
                               !unit.Flags.HasFlag(UnitFlags.Summoned) &&
                               !unit.Flags.HasFlag(UnitFlags.Dummy) &&
                               !unit.Flags.HasFlag(UnitFlags.Playable))
                .ToList();

            var result = new List<UnitData>();
            var iterations = 0;

            while (true)
            {
                var possible = units
                    .Where(u1 => u1.ChallengeRating <= m_ChallengeRating - result.Sum(u2 => u2.ChallengeRating))
                    .ToList();

                if (possible.Count == 0)
                {
                    break;
                }

                result.Add(possible.Random());

                if (++iterations > 1000)
                {
                    Debug.LogWarning("Maximum iterations hit!");
                    break;
                }
            }

            return result;
        }
    }
}