using System;
using DarkBestiary.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class TowerBuff : Interactable
    {
        public event Action<TowerBuff> Clicked;

        public Behaviour Behaviour { get; private set; }

        [SerializeField] private Image m_Icon;
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private TextMeshProUGUI m_Description;
        [SerializeField] private Image m_Outline;

        public void Construct(Behaviour behaviour)
        {
            Behaviour = behaviour;

            m_Icon.sprite = Resources.Load<Sprite>(behaviour.Icon);
            m_Title.text = behaviour.Name;
            m_Description.text = behaviour.Description.ToString(Game.Instance.Character.Entity);
        }

        public void Select()
        {
            m_Outline.color = m_Outline.color.With(a: 1);
        }

        public void Deselect()
        {
            m_Outline.color = m_Outline.color.With(a: 0);
        }

        protected override void OnPointerClick()
        {
            Clicked?.Invoke(this);
        }
    }
}