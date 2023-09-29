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

            if (type == nameof(MarkerBehaviour)) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == nameof(MultishotBehaviour)) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == nameof(MulticastBehaviour)) return json.ToObject<MulticastBehaviourData>(serializer);
            if (type == nameof(ChangeOwnerBehaviour)) return json.ToObject<EmptyBehaviourData>(serializer);
            if (type == nameof(CageBehaviour)) return json.ToObject<CageBehaviourData>(serializer);
            if (type == nameof(ShieldBehaviour)) return json.ToObject<ShieldBehaviourData>(serializer);
            if (type == nameof(BuffBehaviour)) return json.ToObject<BuffBehaviourData>(serializer);
            if (type == nameof(SetBehaviour)) return json.ToObject<SetBehaviourData>(serializer);
            if (type == nameof(CleaveBehaviour)) return json.ToObject<CleaveBehaviourData>(serializer);
            if (type == nameof(SpiritLinkBehaviour)) return json.ToObject<SpiritLinkBehaviourData>(serializer);
            if (type == nameof(BackstabDamageBehaviour)) return json.ToObject<BackstabDamageBehaviourData>(serializer);
            if (type == nameof(ItemBasedBehaviour)) return json.ToObject<ItemBasedBehaviourData>(serializer);
            if (type == nameof(ChangeModelBehaviour)) return json.ToObject<ChangeModelBehaviourData>(serializer);
            if (type == nameof(DualWieldBehaviour)) return json.ToObject<DualWieldBehaviourData>(serializer);
            if (type == nameof(ModifyStatsBehaviour)) return json.ToObject<ModifyStatsBehaviourData>(serializer);
            if (type == nameof(CreateLineBehaviour)) return json.ToObject<CreateLineBehaviourData>(serializer);
            if (type == nameof(SpheresBehaviour)) return json.ToObject<SpheresBehaviourData>(serializer);
            if (type == nameof(PerUnitInRangeBehaviour)) return json.ToObject<AuraBehaviourData>(serializer);
            if (type == nameof(AuraBehaviour)) return json.ToObject<AuraBehaviourData>(serializer);
            if (type == nameof(MaxRageBehaviour)) return json.ToObject<MaxRageBehaviourData>(serializer);

            if (type == nameof(OnStatusEffectBehaviour)) return json.ToObject<OnStatusEffectBehaviourData>(serializer);
            if (type == nameof(OnStatusEffectRemovedBehaviour)) return json.ToObject<OnStatusEffectRemovedBehaviourData>(serializer);
            if (type == nameof(OnAnyoneDiedBehaviour)) return json.ToObject<OnKillBehaviourData>(serializer);
            if (type == nameof(OnUseSkillBehaviour)) return json.ToObject<OnUseSkillBehaviourData>(serializer);
            if (type == nameof(OnUsingSkillBehaviour)) return json.ToObject<OnUseSkillBehaviourData>(serializer);
            if (type == nameof(OnKillBehaviour)) return json.ToObject<OnKillBehaviourData>(serializer);
            if (type == nameof(OnHealthDropsBelowBehaviour)) return json.ToObject<OnHealthDropsBelowBehaviourData>(serializer);
            if (type == nameof(OnDealDamageBehaviour)) return json.ToObject<OnDealDamageBehaviourData>(serializer);
            if (type == nameof(OnTakeDamageBehaviour)) return json.ToObject<OnTakeDamageBehaviourData>(serializer);

            if (type == nameof(OnCombatStartBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnScenarioStartBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnEpisodeStartBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnEpisodeCompleteBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnEndTurnBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnStartTurnBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnBlockBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnDodgeBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnDealHealBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnTakeHealBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnEnterCellBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnExitCellBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnDeathBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnContactBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);
            if (type == nameof(OnSummonBehaviour)) return json.ToObject<EffectBehaviourData>(serializer);

            if (type == nameof(StatusEffectDamageBehaviour)) return json.ToObject<StatusEffectDamageBehaviourData>(serializer);
            if (type == nameof(SummonedDamageBehaviour)) return json.ToObject<SummonedDamageBehaviourData>(serializer);
            if (type == nameof(PerSurroundingEnemyDamageBehaviour)) return json.ToObject<PerSurroundingEnemyDamageBehaviourData>(serializer);
            if (type == nameof(HealthFractionDamageBehaviour)) return json.ToObject<HealthFractionDamageBehaviourData>(serializer);
            if (type == nameof(PerMissingHealthPercentDamageBehaviour)) return json.ToObject<PerMissingHealthPercentDamageBehaviourData>(serializer);
            if (type == nameof(RangeDamageBehaviour)) return json.ToObject<RangeDamageBehaviourData>(serializer);
            if (type == nameof(PerRangeDamageBehaviour)) return json.ToObject<PerRangeDamageBehaviourData>(serializer);

            throw new InvalidDataException($"Unknown behaviour type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BehaviourData);
        }
    }
}