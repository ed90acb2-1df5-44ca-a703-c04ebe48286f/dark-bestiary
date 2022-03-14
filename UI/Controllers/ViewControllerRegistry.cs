using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DarkBestiary.Analysis;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.UI.Views;

namespace DarkBestiary.UI.Controllers
{
    public static class ViewControllerRegistry
    {
        private static readonly Dictionary<Type, IController> Persistent = new Dictionary<Type, IController>()
        {
            {typeof(TransmutationViewController), null},
            {typeof(GambleViewController), null},
            {typeof(ItemForgingViewController), null},
            {typeof(NavigationViewController), null},
            {typeof(EquipmentViewController), null},
            {typeof(CombatLogViewController), null},
            {typeof(TalentsViewController), null},
            {typeof(StashViewController), null},
            {typeof(DismantlingViewController), null},
            {typeof(SpellbookViewController), null},
            {typeof(BlacksmithViewController), null},
            {typeof(AlchemyViewController), null},
            {typeof(RemoveGemsViewController), null},
            {typeof(SocketingViewController), null},
            {typeof(SphereCraftViewController), null},
            {typeof(SkillVendorViewController), null},
        };

        static ViewControllerRegistry()
        {
            GameState.AnyGameStateEnter += OnAnyGameStateEnter;
        }

        public static bool IsPersistent(Type type)
        {
            return Persistent.ContainsKey(type);
        }

        public static T Get<T>() where T : IController
        {
            return (T) Persistent[typeof(T)];
        }

        private static void OnAnyGameStateEnter(GameState gameState)
        {
            if (!gameState.IsMainMenu && !gameState.IsCharacterSelection)
            {
                return;
            }

            foreach (var key in Persistent.Keys.ToList())
            {
                Persistent[key]?.Terminate();
                Persistent[key] = null;
            }
        }

        public static TController Initialize<TController>(IEnumerable<object> parameters = null)
            where TController : class, IViewController<IView>
        {
            var type = typeof(TController);

            if (Persistent.GetValueOrDefault(type, null) is TController controller)
            {
                return controller;
            }

            controller = Instantiate<TController>(parameters);

            if (!IsPersistent(type))
            {
                return controller;
            }

            Persistent[type] = controller;

            return controller;
        }

        public static void Terminate<TController>(TController controller)
            where TController : class, IViewController<IView>
        {
            if (!IsPersistent(typeof(TController)))
            {
                controller.Terminate();
                return;
            }

            controller.View.Hide();
        }

        private static TController Instantiate<TController>(IEnumerable<object> parameters = null)
            where TController : class, IViewController<IView>
        {
            var controller = Container.Instance.Instantiate<TController>(parameters ?? new object[0]);
            controller.View.Hide();
            controller.Initialize();

            return controller;
        }
    }
}