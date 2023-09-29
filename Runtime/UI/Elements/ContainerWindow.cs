using System;
using System.Collections.Generic;
using DarkBestiary.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DarkBestiary.UI.Elements
{
    public class ContainerWindow : Singleton<ContainerWindow>
    {
        public event Action? Hidden;

        [SerializeField]
        private TextMeshProUGUI m_Title = null!;

        [SerializeField]
        private Image m_Icon = null!;

        [SerializeField]
        private VictoryPanelLoot m_Loot = null!;

        [SerializeField]
        private Interactable m_OkayButton = null!;

        private void Start()
        {
            m_OkayButton.PointerClick += OnOkayButtonPointerClick;
            Instance.Hide();

            m_Loot.Initialize();
        }

        public void Show(Item container, List<Item> items)
        {
            Construct(container);
            gameObject.SetActive(true);

            m_Loot.Simulate(items);
        }

        private void Construct(Item container)
        {
            m_Loot.Clear();
            m_Title.text = container.ColoredName;
            m_Icon.sprite = Resources.Load<Sprite>(container.Icon);
        }

        private void Hide()
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