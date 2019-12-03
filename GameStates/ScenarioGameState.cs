using DarkBestiary.Components;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class ScenarioGameState : GameState
    {
        private readonly Scenario scenario;
        private readonly Character character;

        private NavigationViewController navigationViewController;
        private ScenarioViewController scenarioViewController;
        private HudController hudController;

        public ScenarioGameState(Scenario scenario, Character character)
        {
            this.scenario = scenario;
            this.character = character;
        }

        protected override void OnEnter()
        {
            Board.Instance.gameObject.SetActive(true);

            this.scenario.Initialize();

            this.navigationViewController = Container.Instance.Instantiate<NavigationViewController>();
            this.navigationViewController.Initialize();

            this.scenarioViewController = Container.Instance.Instantiate<ScenarioViewController>(new[] {this.scenario});
            this.scenarioViewController.Initialize();

            this.hudController = Container.Instance.Instantiate<HudController>();
            this.hudController.Initialize();

            var health = this.character.Entity.GetComponent<HealthComponent>();
            health.Health = health.HealthMax;
        }

        protected override void OnTick(float delta)
        {
            this.scenario.Tick(delta);
        }

        protected override void OnExit()
        {
            SelectionManager.Instance.DeselectAll();

            this.navigationViewController.Terminate();
            this.scenarioViewController.Terminate();
            this.hudController.Terminate();

            this.scenario.Terminate();

            this.character.Entity.transform.position = new Vector3(-100, 0, 0);
            this.character.Entity.SetActive(true);

            var actor = this.character.Entity.GetComponent<ActorComponent>();
            actor.Show();
            actor.PlayAnimation("idle");
            actor.Model.FlipX(false);

            this.character.Entity.GetComponent<SpellbookComponent>().ResetCooldowns();

            this.character.Entity.GetComponent<HealthComponent>().Revive();

            Timer.Instance.WaitForFixedUpdate(() => Board.Instance.gameObject.SetActive(false));
        }
    }
}