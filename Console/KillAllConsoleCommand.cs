using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios.Scenes;
using DarkBestiary.Values;

namespace DarkBestiary.Console
{
    public class KillAllConsoleCommand : IConsoleCommand
    {
        public string GetSignature()
        {
            return "kill_all";
        }

        public string GetDescription()
        {
            return "Kill all enemies.";
        }

        public string Execute(string input)
        {
            foreach (var entity in Scene.Active.Entities.All().Where(x => x.IsEnemyOfPlayer()))
            {
                entity.GetComponent<HealthComponent>().Kill(CharacterManager.Instance.Character.Entity, new Damage());
            }

            return "Okay";
        }
    }
}