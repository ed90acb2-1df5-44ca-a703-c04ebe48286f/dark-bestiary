using DarkBestiary.Components;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using UnityEngine;

namespace DarkBestiary.GameStates
{
    public class ScenarioScreen : Screen
    {
        private readonly Scenario m_Scenario;
        private readonly Character m_Character;

        private ScenarioViewController m_ScenarioViewController = null!;
        private ActionBarViewController m_ActionBarViewController = null!;
        private TargetFrameViewController m_TargetFrameViewController = null!;

        public ScenarioScreen(Scenario scenario, Character character)
        {
            m_Scenario = scenario;
            m_Character = character;
        }

        protected override void OnEnter()
        {
            Board.Instance.gameObject.SetActive(true);

            m_Scenario.Initialize();

            m_ScenarioViewController = Container.Instance.Instantiate<ScenarioViewController>(new[] { m_Scenario });
            m_ScenarioViewController.Initialize();
            m_ScenarioViewController.View.Show();

            m_ActionBarViewController = Container.Instance.Instantiate<ActionBarViewController>();
            m_ActionBarViewController.Initialize();
            m_ActionBarViewController.View.Show();

            m_TargetFrameViewController = Container.Instance.Instantiate<TargetFrameViewController>();
            m_TargetFrameViewController.Initialize();
            m_TargetFrameViewController.View.Hide();

            var health = m_Character.Entity.GetComponent<HealthComponent>();
            health.Health = health.HealthMax;
        }

        protected override void OnTick(float delta)
        {
            m_Scenario.Tick(delta);
        }

        protected override void OnExit()
        {
            Timer.Instance.WaitForFixedUpdate(() => Board.Instance.gameObject.SetActive(false));
            SelectionManager.Instance.DeselectAll();

            m_ScenarioViewController.Terminate();
            m_ActionBarViewController.Terminate();
            m_TargetFrameViewController.Terminate();

            m_Scenario.Terminate();

            m_Character.Entity.transform.position = new Vector3(-100, 0, 0);
            m_Character.Entity.SetActive(true);

            var actor = m_Character.Entity.GetComponent<ActorComponent>();
            actor.Show();
            actor.PlayAnimation("idle");
            actor.Model.FlipX(false);

            m_Character.Entity.GetComponent<EquipmentComponent>().ResetAltWeaponCooldown();
            m_Character.Entity.GetComponent<SpellbookComponent>().ResetCooldowns();
            m_Character.Entity.GetComponent<HealthComponent>().Revive();
        }
    }
}