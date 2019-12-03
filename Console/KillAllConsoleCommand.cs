using DarkBestiary.Components;
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
            foreach (var entity in Scene.Active.Entities.AliveInTeam(2))
            {
                entity.GetComponent<HealthComponent>().Kill(CharacterManager.Instance.Character.Entity, new Damage());
            }

            return "Okay";
        }
    }
}