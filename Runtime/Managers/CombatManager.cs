using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class CombatManager : MonoBehaviour
    {
        public CombatEncounter Combat { get; private set; }

        private void Start()
        {
            Combat = CombatEncounter.Active;
            CombatEncounter.AnyCombatStarted += OnCombatStarted;
            CombatEncounter.AnyCombatEnded += OnCombatEnded;
        }

        private void OnCombatStarted(CombatEncounter combat)
        {
            Combat = combat;
        }

        private void OnCombatEnded(CombatEncounter combat)
        {
            Combat = null;
        }
    }
}