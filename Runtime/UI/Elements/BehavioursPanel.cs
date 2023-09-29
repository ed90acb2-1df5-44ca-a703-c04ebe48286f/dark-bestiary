using System.Collections.Generic;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class BehavioursPanel : MonoBehaviour
    {
        [SerializeField] private BehaviourView m_BehaviourPrefab;
        [SerializeField] private Transform m_BehaviourContainer;

        private readonly List<BehaviourView> m_BehaviourViews = new();

        public void Add(Behaviour behaviour)
        {
            var item = Instantiate(m_BehaviourPrefab, m_BehaviourContainer);
            item.Initialize(behaviour);
            m_BehaviourViews.Add(item);
        }

        public void Remove(Behaviour behaviour)
        {
            var behaviourView = m_BehaviourViews.Find(item => item.Behaviour.Equals(behaviour));

            if (behaviourView == null)
            {
                return;
            }

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
            m_BehaviourViews.Remove(behaviourView);
        }

        public void Clear()
        {
            foreach (var behaviourView in m_BehaviourViews)
            {
                behaviourView.Terminate();
                Destroy(behaviourView.gameObject);
            }

            m_BehaviourViews.Clear();
        }
    }
}