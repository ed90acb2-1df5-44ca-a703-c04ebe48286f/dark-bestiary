using System.Collections.Generic;
using DarkBestiary.Data;
using DarkBestiary.Validators;
using DarkBestiary.Visuals;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class CreateLineBehaviour : Behaviour
    {
        private readonly CreateLineBehaviourData m_Data;

        private Line m_Line;

        public CreateLineBehaviour(CreateLineBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        protected override void OnApply(GameObject caster, GameObject target)
        {
            var prefab = Resources.Load<Line>(m_Data.Prefab);

            if (prefab == null)
            {
                Debug.Log("Not found prefab: " + m_Data.Prefab);
                return;
            }

            m_Line = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            m_Line.Construct(caster.transform, target.transform);
        }

        protected override void OnRemoved(GameObject source, GameObject target)
        {
            if (m_Line == null)
            {
                return;
            }

            Object.Destroy(m_Line.gameObject);
            m_Line = null;
        }
    }
}