using System;
using System.IO;
using DarkBestiary.Behaviours;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBestiary.Data.Readers.Json
{
    public class BehaviourJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var type = json["Type"].Value<string>();

            if (type == typeof(MarkerBehaviour).Name) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == typeof(MultishotBehaviour).Name) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == typeof(ChangeOwnerBehaviour).Name) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == typeof(CageBehaviour).Name) return json.ToObject<CageBehaviourData>(serializer);
            if (type == typeof(ShieldBehaviour).Name) return json.ToObject<ShieldBehaviourData>(serializer);
            if (type == typeof(BuffBehaviour).Name) return json.ToObject<BuffBehaviourData>(serializer);
            if (type == typeof(SetBehaviour).Name) return json.ToObject<SetBehaviourData>(serializer);
            if (type == typeof(CleaveBehaviour).Name) return json.ToObject<CleaveBehaviourData>(serializer);
            if (type == typeof(SpiritLinkBehaviour).Name) return json.ToObject<SpiritLinkBehaviourData>(serializer);
            if (type == typeof(UnlockSkillBehaviour).Name) return json.ToObject<UnlockSkillBehaviourData>(serializer);
            if (type == typeof(BackstabDamageBehaviour).Name) return json.ToObject<BackstabDamageBehaviourData>(serializer);
            if (type == typeof(ItemBasedBehaviour).Name) return json.ToObject<ItemBasedBehaviourData>(serializer);
            if (type == typeof(ChangeModelBehaviour).Name) return json.ToObject<ChangeModelBehaviourData>(serializer);
            if (type == typeof(DualWieldBehaviour).Name) return json.ToObject<DualWieldBehaviourData>(serializer);
            if (type == typeof(ModifyStatsBehaviour).Name) return json.ToObject<ModifyStatsBehaviourData>(serializer);
            if (type == typeof(CreateLineBehaviour).Name) return json.ToObject<CreateLineBehaviourData>(serializer);
            if (type == typeof(SpheresBehaviour).Name) return json.ToObject<SpheresBehaviourData>(serializer);
            if (type == typeof(PerUnitInRangeBehaviour).Name) return json.ToObject<AuraBehaviourData>(serializer);
            if (type == typeof(AuraBehaviour).Name) return json.ToObject<AuraBehaviourData>(serializer);

            if (type == typeof(OnStatusEffectBehaviour).Name) return json.ToObject<OnStatusEffectBehaviourData>(serializer);
            if (type == typeof(OnStatusEffectRemovedBehaviour).Name) return json.ToObject<OnStatusEffectRemovedBehaviourData>(serializer);
            if (type == typeof(OnCombatStartBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnScenarioStartBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnEpisodeStartBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnEpisodeCompleteBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnHealthDropsBelowBehaviour).Name) return json.ToObject<OnHealthDropsBelowBehaviourData>(serializer);
            if (type == typeof(OnBlockBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnDealDamageBehaviour).Name) return json.ToObject<OnDealDamageBehaviourData>(serializer);
            if (type == typeof(OnTakeDamageBehaviour).Name) return json.ToObject<OnTakeDamageBehaviourData>(serializer);
            if (type == typeof(OnDealHealBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnTakeHealBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnEnterCellBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnExitCellBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnAnyoneDiedBehaviour).Name) return json.ToObject<OnKillBehaviourData>(serializer);
            if (type == typeof(OnKillBehaviour).Name) return json.ToObject<OnKillBehaviourData>(serializer);
            if (type == typeof(OnDeathBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == typeof(OnUseSkillBehaviour).Name) return json.ToObject<OnUseSkillBehaviourData>(serializer);
            if (type == typeof(OnContactBehaviour).Name) return json.ToObject<EffectBehaviourData>(serializer);

            if (type == typeof(StatusEffectDamageBehaviour).Name) return json.ToObject<StatusEffectDamageBehaviourData>(serializer);
            if (type == typeof(PerSurroundingEnemyDamageBehaviour).Name) return json.ToObject<PerSurroundingEnemyDamageBehaviourData>(serializer);
            if (type == typeof(HealthFractionDamageBehaviour).Name) return json.ToObject<HealthFractionDamageBehaviourData>(serializer);
            if (type == typeof(PerMissingHealthPercentDamageBehaviour).Name) return json.ToObject<PerMissingHealthPercentDamageBehaviourData>(serializer);
            if (type == typeof(RangeDamageBehaviour).Name) return json.ToObject<RangeDamageBehaviourData>(serializer);
            if (type == typeof(PerRangeDamageBehaviour).Name) return json.ToObject<PerRangeDamageBehaviourData>(serializer);

            throw new InvalidDataException($"Unknown behaviour type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BehaviourData);
        }
    }
}