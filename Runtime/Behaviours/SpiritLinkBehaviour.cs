using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Extensions;
using DarkBestiary.Scenarios.Scenes;
using DarkBestiary.Validators;
using DarkBestiary.Values;
using UnityEngine;

namespace DarkBestiary.Behaviours
{
    public class SpiritLinkBehaviour : Behaviour
    {
        private readonly SpiritLinkBehaviourData m_Data;

        public SpiritLinkBehaviour(SpiritLinkBehaviourData data, List<ValidatorWithPurpose> validators) : base(data, validators)
        {
            m_Data = data;
        }

        public Damage ReduceAndDistribute(GameObject attacker, Damage damage)
        {
            var linked = Scene.Active.Entities.Find(entity =>
                entity != Target &&
                entity.IsAllyOf(Target) &&
                entity.GetComponent<BehavioursComponent>().GetStackCount(m_Data.Id) > 0);

            if (linked.Count == 0)
            {
                return damage;
            }

            var reduced = damage * (1 - m_Data.Fraction);

            Timer.Instance.WaitForFixedUpdate(() => { Distribute(attacker, damage, linked); });

            return reduced;
        }

        private void Distribute(GameObject attacker, Damage damage, List<GameObject> linked)
        {
            var toDistribute = damage * m_Data.Fraction / linked.Count;
            toDistribute.Type = DamageType.Health;
            toDistribute.InfoFlags = DamageInfoFlags.None;
            toDistribute.InfoFlags |= DamageInfoFlags.SpiritLink;
            toDistribute.InfoFlags |= DamageInfoFlags.Reflected;
            toDistribute.Flags = DamageFlags.None;
            toDistribute.Flags |= DamageFlags.CantBeBlocked;
            toDistribute.Flags |= DamageFlags.CantBeDodged;
            toDistribute.Flags |= DamageFlags.CantBeCritical;
            toDistribute.Flags |= DamageFlags.Magic;

            foreach (var entity in linked)
            {
                entity.GetComponent<HealthComponent>().Damage(attacker, toDistribute);
            }
        }
    }
}