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
        private ActionBarViewController actionBarViewController;
        private TargetFrameViewController targetFrameViewController;

        public ScenarioGameState(Scenario scenario, Character character)
        {
            this.scenario = scenario;
            this.character = character;
        }

        protected override void OnEnter()
        {
            Board.Instance.gameObject.SetActive(true);

            this.scenario.Initialize();

            this.navigationViewController = ViewControllerRegistry.Initialize<NavigationViewController>();
            this.navigationViewController.View.Show();

            this.scenarioViewController = ViewControllerRegistry.Initialize<ScenarioViewController>(new[] {this.scenario});
            this.scenarioViewController.View.Show();

            this.actionBarViewController = ViewControllerRegistry.Initialize<ActionBarViewController>();
            this.actionBarViewController.View.Show();

            this.targetFrameViewController = ViewControllerRegistry.Initialize<TargetFrameViewController>();
            this.targetFrameViewController.View.Hide();

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
            this.actionBarViewController.Terminate();
            this.targetFrameViewController.Terminate();

            this.scenario.Terminate();

            this.character.Entity.transform.position = new Vector3(-100, 0, 0);
            this.character.Entity.SetActive(true);

            var actor = this.character.Entity.GetComponent<ActorComponent>();
            actor.Show();
            actor.PlayAnimation("idle");
            actor.Model.FlipX(false);

            this.character.Entity.GetComponent<EquipmentComponent>().ResetAltWeaponCooldown();
            this.character.Entity.GetComponent<SpellbookComponent>().ResetCooldowns();
            this.character.Entity.GetComponent<HealthComponent>().Revive();

            Timer.Instance.WaitForFixedUpdate(() => Board.Instance.gameObject.SetActive(false));
        }
    }
}