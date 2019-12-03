using System.Collections.Generic;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Elements
{
    public class BehavioursPanel : MonoBehaviour
    {
        [SerializeField] private BehaviourView behaviourPrefab;
        [SerializeField] private Transform behaviourContainer;

        private readonly List<BehaviourView> behaviourViews = new List<BehaviourView>();

        public void Add(Behaviour behaviour)
        {
            var item = Instantiate(this.behaviourPrefab, this.behaviourContainer);
            item.Initialize(behaviour);
            this.behaviourViews.Add(item);
        }

        public void Remove(Behaviour behaviour)
        {
            var behaviourView = this.behaviourViews.Find(item => item.Behaviour.Equals(behaviour));

            if (behaviourView == null)
            {
                return;
            }

            behaviourView.Terminate();
            Destroy(behaviourView.gameObject);
            this.behaviourViews.Remove(behaviourView);
        }

        public void Clear()
        {
            foreach (var behaviourView in this.behaviourViews)
            {
                behaviourView.Terminate();
                Destroy(behaviourView.gameObject);
            }

            this.behaviourViews.Clear();
        }
    }
}