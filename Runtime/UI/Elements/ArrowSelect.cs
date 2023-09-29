using System;
using UnityEngine;

namespace DarkBestiary.UI.Elements
{
    public class ArrowSelect : MonoBehaviour
    {
        public event Action<int> Changed;

        [SerializeField] private Interactable m_LeftArrow;
        [SerializeField] private Interactable m_RightArrow;

        private int m_Index;
        private int m_Count;

        private void Start()
        {
            m_LeftArrow.PointerClick += OnLeftArrowPointerClick;
            m_RightArrow.PointerClick += OnRightArrowPointerClick;
        }

        public void Initialize(int count)
        {
            m_Count = count;
        }

        public void Random()
        {
            m_Index = Rng.Range(0, m_Count - 1);
            Changed?.Invoke(m_Index);
        }

        private void OnLeftArrowPointerClick()
        {
            m_Index--;

            if (m_Index < 0)
            {
                m_Index = m_Count - 1;
            }

            Changed?.Invoke(m_Index);
        }

        private void OnRightArrowPointerClick()
        {
            m_Index++;

            if (m_Index > m_Count - 1)
            {
                m_Index = 0;
            }

            Changed?.Invoke(m_Index);
        }
    }
}