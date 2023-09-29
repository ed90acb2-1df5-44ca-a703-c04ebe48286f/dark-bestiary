using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Managers;
using DarkBestiary.Properties;
using DarkBestiary.Validators;
using UnityEngine;

namespace DarkBestiary.Effects
{
    public class CreateUnitEffect : Effect
    {
        private readonly IUnitRepository m_UnitRepository;
        private readonly CreateUnitEffectData m_Data;
        private readonly BoardNavigator m_BoardNavigator;

        public CreateUnitEffect
        (
            CreateUnitEffectData data, List<ValidatorWithPurpose> validators, BoardNavigator boardNavigator,
            IUnitRepository unitRepository
        ) : base(data, validators)
        {
            m_Data = data;
            m_BoardNavigator = boardNavigator;
            m_UnitRepository = unitRepository;
        }

        protected override Effect New()
        {
            return new CreateUnitEffect(m_Data, Validators, m_BoardNavigator, m_UnitRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var entity = m_UnitRepository.Find(m_Data.UnitId);
            entity.transform.position = target.Snapped();
            entity.GetComponent<ActorComponent>().Model.transform.rotation =
                caster.GetComponent<ActorComponent>().Model.transform.rotation;

            var casterUnit = caster.GetComponent<UnitComponent>();
            var createdUnit = entity.GetComponent<UnitComponent>();
            createdUnit.TeamId = casterUnit.TeamId;
            createdUnit.Owner = DetermineOwner(casterUnit);
            createdUnit.Level = casterUnit.Level;

            if (!createdUnit.IsDummy)
            {
                IncreaseSummonedStats(caster, entity);
            }

            entity.AddComponent<SummonedComponent>().Construct(
                caster, m_Data.Duration, m_Data.KillOnCasterDeath, m_Data.KillOnEpisodeComplete
            ).Initialize();

            var health = entity.GetComponent<HealthComponent>();
            health.Health = health.HealthMax * m_Data.HealthFraction;
            health.IsInvulnerable = true;

            Timer.Instance.WaitForFixedUpdate(() =>
                {
                    health.Health = health.HealthMax * m_Data.HealthFraction;
                    health.IsInvulnerable = false;
                }
            );

            TriggerFinished();
        }

        private Owner DetermineOwner(UnitComponent caster)
        {
            if (m_Data.Owner == Owner.Auto && caster.gameObject.IsCharacter())
            {
                return SettingsManager.Instance.SummonedUnitsControlledByAi ? Owner.Neutral : caster.Owner;
            }

            return m_Data.Owner == Owner.Auto ? caster.Owner : m_Data.Owner;
        }

        private static void IncreaseSummonedStats(GameObject caster, GameObject entity)
        {
            var casterProperties = caster.GetComponent<PropertiesComponent>();

            var properties = entity.GetComponent<PropertiesComponent>();

            var health = properties.Properties.Values
                .First(property => property.Type == PropertyType.Health);

            var damage = properties.Properties.Values
                .First(property => property.Type == PropertyType.DamageIncrease);

            health.Increase(health.Value() * casterProperties.Get(PropertyType.MinionHealth).Value());
            damage.Increase(casterProperties.Get(PropertyType.MinionDamage).Value());
        }
    }
}