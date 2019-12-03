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
        private readonly IUnitRepository unitRepository;
        private readonly CreateUnitEffectData data;
        private readonly BoardNavigator boardNavigator;

        public CreateUnitEffect(CreateUnitEffectData data, List<Validator> validators, BoardNavigator boardNavigator,
            IUnitRepository unitRepository) : base(data, validators)
        {
            this.data = data;
            this.boardNavigator = boardNavigator;
            this.unitRepository = unitRepository;
        }

        protected override Effect New()
        {
            return new CreateUnitEffect(this.data, this.Validators, this.boardNavigator, this.unitRepository);
        }

        protected override void Apply(GameObject caster, GameObject target)
        {
            Apply(caster, target.transform.position);
        }

        protected override void Apply(GameObject caster, Vector3 target)
        {
            var entity = this.unitRepository.Find(this.data.UnitId);
            entity.transform.position = target.Snapped();
            entity.GetComponent<ActorComponent>().Model.transform.rotation =
                caster.GetComponent<ActorComponent>().Model.transform.rotation;

            var casterUnit = caster.GetComponent<UnitComponent>();
            var createdUnit = entity.GetComponent<UnitComponent>();
            createdUnit.TeamId = casterUnit.TeamId;
            createdUnit.Owner = DetermineOwner(casterUnit);
            createdUnit.Level = casterUnit.Level;

            IncreaseSummonedStats(caster, entity);

            entity.AddComponent<SummonedComponent>().Construct(
                caster, this.data.Duration, this.data.KillOnCasterDeath, this.data.KillOnEpisodeComplete).Initialize();

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                var health = entity.GetComponent<HealthComponent>();
                health.Health = health.HealthMax * this.data.HealthFraction;
            });

            TriggerFinished();
        }

        private Owner DetermineOwner(UnitComponent caster)
        {
            if (this.data.Owner == Owner.Auto && caster.gameObject.IsCharacter())
            {
                return SettingsManager.Instance.SummonedUnitsControlledByAi ? Owner.Neutral : caster.Owner;
            }

            return this.data.Owner == Owner.Auto ? caster.Owner : this.data.Owner;
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