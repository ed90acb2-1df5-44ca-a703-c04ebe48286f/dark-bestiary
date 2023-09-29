using System;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using Zenject;

namespace DarkBestiary.Managers
{
    public class BestiaryManager2 : IInitializable
    {
        public static event Action<UnitComponent>? BestiaryUpdated;

        public void Initialize()
        {
            HealthComponent.AnyEntityDied += OnAnyEntityDied;
        }

        private void OnAnyEntityDied(EntityDiedEventData data)
        {
            if (!data.Killer.IsAllyOfPlayer() ||
                data.Victim.IsDummy() ||
                data.Victim.IsSummoned() ||
                data.Victim.IsAllyOfPlayer())
            {
                return;
            }

            var unit = data.Victim.GetComponent<UnitComponent>();

            if (Game.Instance.Character.Data.UnlockedMonsters.Any(unitId => unit.Id == unitId))
            {
                return;
            }

            Game.Instance.Character.Data.UnlockedMonsters.Add(unit.Id);

            BestiaryUpdated?.Invoke(unit);
        }
    }
}