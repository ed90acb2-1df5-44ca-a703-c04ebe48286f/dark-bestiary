using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Visuals;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class CreateLineBehaviour : Behaviour
    {
        private readonly CreateLineBehaviourData data;

        private Line line;

        public CreateLineBehaviour(CreateLineBehaviourData data, List<Validator> validators) : base(data, validators)
        {
            this.data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var prefab = Resources.Load<Line>(this.data.Prefab);

            if (prefab == null)
            {
                Debug.Log("Not found prefab: " + this.data.Prefab);
                return;
            }

            this.line = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            this.line.Construct(caster.transform, target.transform);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            if (this.line == null)
            {
                return;
            }

            Object.Destroy(this.line.gameObject);
            this.line = null;
        }
    }
}