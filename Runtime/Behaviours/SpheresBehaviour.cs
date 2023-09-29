using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class SpheresBehaviour : Behaviour
    {
        private readonly List<GameObject> m_Spheres = new();
        private readonly SpheresBehaviourData m_Data;

        public SpheresBehaviour(SpheresBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            CreateSphere();
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            foreach (var sphere in m_Spheres)
            {
                Object.Destroy(sphere.gameObject);
            }

            m_Spheres.Clear();
        }

        protected override void OnStackCountChanged(Behaviour _, int delta)
        {
            if (!IsApplied)
            {
                return;
            }

            while (m_Spheres.Count < StackCount)
            {
                CreateSphere();
            }

            while (m_Spheres.Count > StackCount)
            {
                DestroySphere();
            }
        }

        private void CreateSphere()
        {
            var prefab = Resources.Load<GameObject>(m_Data.Prefab);
            m_Spheres.Add(Object.Instantiate(prefab, Target.transform));
        }

        private void DestroySphere()
        {
            var sphere = m_Spheres.Last();
            Object.Destroy(sphere.gameObject);
            m_Spheres.Remove(sphere);
        }
    }
}