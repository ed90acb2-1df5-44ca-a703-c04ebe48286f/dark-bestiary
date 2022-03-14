using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.Managers
{
    public class FloatingActionbarManager : Singleton<FloatingActionbarManager>
    {
        public bool IsEnabled { get; set; } = true;

        [SerializeField] private FloatingActionBar actionBarPrefab;
        [SerializeField] private Canvas canvas;

        private void Start()
        {
            Component.AnyComponentInitialized += OnComponentInitialized;
        }

        private void OnComponentInitialized(Component component)
        {
            if (!IsEnabled)
            {
                return;
            }

            var spellbook = component as SpellbookComponent;

            if (spellbook == null || spellbook.gameObject.IsDummy() || !IsAppropriateGameState())
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                Instantiate(this.actionBarPrefab, this.canvas.transform).Initialize(spellbook);
            });
        }

        private static bool IsAppropriateGameState()
        {
            return Game.Instance != null &&
                   Game.Instance.State != null && (Game.Instance.State.IsScenario || Game.Instance.State.IsHub);
        }
    }
}