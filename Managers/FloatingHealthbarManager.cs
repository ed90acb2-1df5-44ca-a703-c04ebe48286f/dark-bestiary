using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary.Managers
{
    public class FloatingHealthbarManager : Singleton<FloatingHealthbarManager>
    {
        [SerializeField] private FloatingHealthBar healthBarPrefab;
        [SerializeField] private Canvas canvas;

        private void Start()
        {
            Component.AnyComponentInitialized += OnComponentInitialized;
        }

        private void OnComponentInitialized(Component component)
        {
            var health = component as HealthComponent;

            if (health == null || health.gameObject.IsDummy() || CharacterManager.Instance.Character == null || health.gameObject.IsCharacter())
            {
                return;
            }

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                Instantiate(this.healthBarPrefab, this.canvas.transform).Initialize(health);
            });
        }
    }
}