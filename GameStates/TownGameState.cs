using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Managers;
using DarkBestiary.UI.Controllers;

namespace DarkBestiary.GameStates
{
    public class TownGameState : GameState
    {
        private TownController controller;

        protected override void OnEnter()
        {
            var character = CharacterManager.Instance.Character;

            if (!character.IsStartScenarioCompleted)
            {
                Game.Instance.SwitchState(
                    new ScenarioGameState(
                        Container.Instance.Resolve<IScenarioRepository>().Starting(),
                        Container.Instance.Resolve<CharacterManager>().Character
                    )
                );

                return;
            }

            if (character.Data.IsHardcore && character.Data.IsDead)
            {
                Game.Instance.ToCharacterSelection();
                return;
            }

            // Note: this stuff is required to recalculate levels, useful when changing exp formulas
            // TODO: exp recalculation do not trigger achievements
            character.Entity.GetComponent<ExperienceComponent>().Experience.Add(0);
            character.Entity.GetComponent<ReliquaryComponent>().Available.ForEach(r => r.Experience.Add(0));

            this.controller = Container.Instance.Instantiate<TownController>();
            this.controller.Initialize();

            MusicManager.Instance.Play("event:/Music/Town");
        }

        protected override void OnExit()
        {
            this.controller?.Terminate();
        }

        protected override void OnTick(float delta)
        {
        }
    }
}