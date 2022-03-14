using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using DarkBestiary.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ContainerWindow : Singleton<ContainerWindow>
    {
        public event Payload Hidden;

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        [SerializeField] private VictoryPanelLoot loot;
        [SerializeField] private Interactable okayButton;

        private void Start()
        {
            this.okayButton.PointerClick += OnOkayButtonPointerClick;
            Instance.Hide();

            this.loot.Initialize();
        }

        public void Show(Item container, Loot loot, GameObject entity)
        {
            Construct(container);
            gameObject.SetActive(true);

            loot.RollDrop(entity.GetComponent<UnitComponent>().Level, items =>
            {
                entity.GetComponent<InventoryComponent>().Pickup(items);
                Instance.Show(container, items);
            });
        }

        public void Show(Item container, List<Item> items)
        {
            Construct(container);
            gameObject.SetActive(true);

            this.loot.Simulate(items);
        }

        private void Construct(Item container)
        {
            this.loot.Clear();
            this.title.text = container.ColoredName;
            this.icon.sprite = Resources.Load<Sprite>(container.Icon);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            Hidden?.Invoke();
        }

        private void OnOkayButtonPointerClick()
        {
            Hide();
        }
    }
}