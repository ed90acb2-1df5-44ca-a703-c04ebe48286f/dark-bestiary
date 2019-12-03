using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class SpheresBehaviour : Behaviour
    {
        private readonly List<GameObject> spheres = new List<GameObject>();
        private readonly SpheresBehaviourData data;

        public SpheresBehaviour(SpheresBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            CreateSphere();
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            foreach (var sphere in this.spheres)
            {
                Object.Destroy(sphere.gameObject);
            }

            this.spheres.Clear();
        }

        protected override void OnStackCountChanged(Behaviour behaviour)
        {
            while (this.spheres.Count < StackCount)
            {
                CreateSphere();
            }

            while (this.spheres.Count > StackCount)
            {
                DestroySphere();
            }
        }

        private void CreateSphere()
        {
            var prefab = Resources.Load<GameObject>(this.data.Prefab);
            this.spheres.Add(Object.Instantiate(prefab, Target.transform));
        }

        private void DestroySphere()
        {
            var sphere = this.spheres.Last();
            Object.Destroy(sphere.gameObject);
            this.spheres.Remove(sphere);
        }
    }
}