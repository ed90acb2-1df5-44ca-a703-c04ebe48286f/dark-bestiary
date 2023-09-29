using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class MirrorImageEffect : Effect
    {
        private readonly MirrorImageEffectData m_Data;
        private readonly IUnitRepository m_UnitRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;

        public MirrorImageEffect(MirrorImageEffectData data, List<ValidatorWithPurpose> validators,
            IUnitRepository unitRepository, IBehaviourRepository behaviourRepository) : base(data, validators)
        {
            m_Data = data;
            m_UnitRepository = unitRepository;
            m_BehaviourRepository = behaviourRepository;
        }

        protected override Effect New()
        {
            return new MirrorImageEffect(m_Data, Validators, m_UnitRepository, m_BehaviourRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var casterUnit = caster.GetComponent<UnitComponent>();
            var casterActor = caster.GetComponent<ActorComponent>();

            var positions = new List<Vector3> {caster.transform.position};
            var entities = new List<GameObject> {caster};

            casterActor.Hide();
            casterActor.Model.LookAt(new Vector2(Rng.Range(-30, 30), Rng.Range(-30, 30)));
            casterActor.transform.position = new Vector3(-100, 0, 0);

            var behaviour = m_BehaviourRepository.FindOrFail(m_Data.BehaviourId);

            for (var i = 0; i < m_Data.Count; i++)
            {
                var cell = BoardNavigator.Instance.WithinCircle(positions[0], 10)
                    .Where(c => !c.IsOccupied && c.IsWalkable)
                    .OrderBy(c => (c.transform.position - positions[0]).magnitude)
                    .FirstOrDefault();

                if (cell == null)
                {
                    break;
                }

                var entity = m_UnitRepository.FindOrFail(casterUnit.Id);

                var unit = entity.GetComponent<UnitComponent>();
                unit.ChangeOwner(casterUnit);
                unit.Level = casterUnit.Level;

                var behaviours = entity.GetComponent<BehavioursComponent>();
                behaviours.SyncWith(caster.GetComponent<BehavioursComponent>());
                behaviours.ApplyAllStacks(behaviour, caster);

                entity.GetComponent<HealthComponent>().SyncHealthFraction(caster.GetComponent<HealthComponent>());
                entity.GetComponent<SpellbookComponent>().SyncCooldowns(caster.GetComponent<SpellbookComponent>());

                var actor = entity.GetComponent<ActorComponent>();
                actor.Hide();
                actor.Model.LookAt(new Vector2(Rng.Range(-30, 30), Rng.Range(-30, 30)));

                entity.AddComponent<SummonedComponent>().Construct(caster, m_Data.Duration, true, true);
                entity.transform.position = new Vector3(-100, 0, 0);

                entities.Add(entity);
                positions.Add(cell.transform.position);
            }

            var queue = new Queue<Vector3>(positions.Shuffle());

            foreach (var entity in entities)
            {
                entity.transform.position = queue.Dequeue().Snapped();
                entity.GetComponent<ActorComponent>().Show();

                var prefab = Resources.Load<GameObject>(m_Data.Prefab);

                if (prefab != null)
                {
                    Object.Instantiate(prefab, entity.transform.position, Quaternion.identity).DestroyAsVisualEffect();
                }
            }

            TriggerFinished();
        }
    }
}