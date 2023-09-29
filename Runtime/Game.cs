using System;
using System.Collections.Generic;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.Map;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Controllers;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Screen = DarkBestiary.GameStates.Screen;

namespace DarkBestiary
{
    public class Game
    {
        public static Game Instance { get; private set; } = null!;

        public event Action? ViewSwitched;
        public event Action? SceneSwitched;
        public event Action? CharacterSwitched;

        public Character Character { get; private set; } = null!;

        public bool IsTown => m_Screen is TownScreen;
        public bool IsScenario => m_Screen is ScenarioScreen;

        private readonly Dictionary<Type, IViewController> m_Controllers = new();
        private readonly ICharacterRepository m_CharacterRepository;

        private IViewController? m_CurrentController;
        private Screen? m_Screen;
        private bool m_RecreateControllers;

        public Game()
        {
            Instance = this;

            if (Debug.isDebugBuild)
            {
                Container.Instance.Instantiate<DeveloperConsoleViewController>().Initialize();
            }

            m_CharacterRepository = Container.Instance.Resolve<ICharacterRepository>();

            SwitchScreen(() => new LogosScreen());
        }

        public void ToScenario(int scenarioId)
        {
            SwitchScreen(() =>
            {
                var scenario = Container.Instance.Resolve<IScenarioRepository>().FindOrFail(scenarioId);
                return new ScenarioScreen(scenario, Character);
            });
        }

        public void ToScenario(Scenario scenario)
        {
            SwitchScreen(() => new ScenarioScreen(scenario, Character));
        }

        public void ToMainMenu()
        {
            SwitchScreen(() =>
            {
                Cleanup();
                return new MainMenuScreen();
            });
        }

        public void ToTown()
        {
            SwitchScreen(() => new TownScreen());
        }

        public void ToMap()
        {
            SwitchScreen(() => new MapScreen());
        }

        public void ToCredits()
        {
            SwitchScreen(() => new CreditsScreen());
        }

        public void ToCharacterCreation()
        {
            SwitchScreen(() => new CharacterCreationScreen());
        }

        public void ToCharacterSelection()
        {
            SwitchScreen(() => new CharacterSelectionScreen());
        }

        public void Tick(float delta)
        {
            m_Screen?.Tick(delta);
        }

        public void Save()
        {
            if (Character == null)
            {
                return;
            }

            m_CharacterRepository.Save(Character);
        }

        public static void Quit()
        {
            Application.Quit();
        }

        public void EnterTownWithNewCharacter(Character character)
        {
            SwitchScreen(() =>
            {
                Cleanup();

                Character = character;
                Character.Data.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                CharacterSwitched?.Invoke();

                Setup();

                return new TownScreen();
            });
        }

        private void Cleanup()
        {
            foreach (var controller in m_Controllers.Values)
            {
                controller.Terminate();
            }

            m_Controllers.Clear();

            Character?.Entity.Terminate();
            Character = null!;

            m_CurrentController = null;
        }

        private void Setup()
        {
            RegisterController(Container.Instance.Instantiate<MasteriesViewController>());
            RegisterController(Container.Instance.Instantiate<AchievementsViewController>());
            RegisterController(Container.Instance.Instantiate<AttributesViewController>());
            RegisterController(Container.Instance.Instantiate<CombatLogViewController>());
            RegisterController(Container.Instance.Instantiate<DamageMeterViewController>());
            RegisterController(Container.Instance.Instantiate<ReliquaryViewController>());
            RegisterController(Container.Instance.Instantiate<MailboxViewController>());
            RegisterController(Container.Instance.Instantiate<MenuViewController>());
            RegisterController(Container.Instance.Instantiate<TalentsViewController>());

            RegisterController(Container.Instance.Instantiate<NavigationViewController>());

            RegisterController(Container.Instance.Instantiate<RuneCraftViewController>());
            RegisterController(Container.Instance.Instantiate<EateryViewController>());
            RegisterController(Container.Instance.Instantiate<ForgottenDepthsViewController>());
            RegisterController(Container.Instance.Instantiate<LeaderboardViewController>());
            RegisterController(Container.Instance.Instantiate<BestiaryViewController>());
            RegisterController(Container.Instance.Instantiate<AlchemyViewController>());
            RegisterController(Container.Instance.Instantiate<BlacksmithViewController>());
            RegisterController(Container.Instance.Instantiate<EquipmentViewController>());

            RegisterController(Container.Instance.Instantiate<SkillRemoveViewController>());
            RegisterController(Container.Instance.Instantiate<SkillSelectViewController>());

            RegisterController(Container.Instance.Instantiate<StashViewController>());
            RegisterController(Container.Instance.Instantiate<TransmutationViewController>());
            RegisterController(Container.Instance.Instantiate<SphereCraftViewController>());
            RegisterController(Container.Instance.Instantiate<GambleViewController>());
            RegisterController(Container.Instance.Instantiate<DismantlingViewController>());
            RegisterController(Container.Instance.Instantiate<RemoveGemsViewController>());
            RegisterController(Container.Instance.Instantiate<SocketingViewController>());
            RegisterController(Container.Instance.Instantiate<RuneInscriptionViewController>());

            RegisterController(Container.Instance.Instantiate<VendorViewController>(new object[]
            {
                new List<VendorPanel.Category>
                {
                    VendorPanel.Category.All,
                    VendorPanel.Category.Armor,
                    VendorPanel.Category.Weapon,
                    VendorPanel.Category.Miscellaneous,
                    VendorPanel.Category.Buyout
                }
            }));

            RegisterController(Container.Instance.Instantiate<TownViewController>());
            RegisterController(Container.Instance.Instantiate<MapViewController>());

            var equipmentView = GetController<EquipmentViewController>().View;
            equipmentView.Connect(GetController<StashViewController>().View);
            equipmentView.Connect(GetController<TransmutationViewController>().View);
            equipmentView.Connect(GetController<SphereCraftViewController>().View);
            equipmentView.Connect(GetController<GambleViewController>().View);
            equipmentView.Connect(GetController<DismantlingViewController>().View);
            equipmentView.Connect(GetController<RemoveGemsViewController>().View);
            equipmentView.Connect(GetController<SocketingViewController>().View);
            equipmentView.Connect(GetController<VendorViewController>().View);
            equipmentView.Connect(GetController<RuneInscriptionViewController>().View);

            foreach (var controller in m_Controllers.Values)
            {
                controller.Initialize();
            }

            return;

            void RegisterController(IViewController controller)
            {
                m_Controllers.Add(controller.GetType(), controller);
            }
        }

        public TController GetController<TController>() where TController : IViewController
        {
            return (TController) m_Controllers[typeof(TController)];
        }

        public void SwitchView<TController>() where TController : class, IViewController
        {
            m_CurrentController?.HideView();
            m_CurrentController = GetController<TController>();
            m_CurrentController.ShowView();

            ViewSwitched?.Invoke();
        }

        private void SwitchScreen(Func<Screen> constructor)
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.None);

            ScreenFade.Instance.To(() =>
            {
                UIManager.HideAllTooltips();

                Save();

                m_Screen?.Exit();

                if (m_RecreateControllers)
                {
                    Setup();
                    m_RecreateControllers = false;
                }

                m_Screen = constructor();
                m_Screen.Enter();

                CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);

                SceneSwitched?.Invoke();
            }, true);
        }
    }
}