using System;
using System.IO;
using DarkBestiary.Effects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBestiary.Data.Readers.Json
{
    public class EffectJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var type = json["Type"].Value<string>();

            if (type == nameof(KillEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(ShowEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(HideEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(CreateCorpseEffect)) return json.ToObject<CreateCorpseEffectData>(serializer);
            if (type == nameof(ChangeOwnerEffect)) return json.ToObject<ChangeOwnerEffectData>(serializer);
            if (type == nameof(AttackEffect)) return json.ToObject<AttackEffectData>(serializer);
            if (type == nameof(GiveItemEffect)) return json.ToObject<GiveItemEffectData>(serializer);
            if (type == nameof(DestroyEquippedItemEffect)) return json.ToObject<DestroyEquippedItemEffectData>(serializer);
            if (type == nameof(RemoveShieldsEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(LaunchAOEMissileEffect)) return json.ToObject<LaunchAoeMissileEffectData>(serializer);
            if (type == nameof(IfElseEffect)) return json.ToObject<IfElseEffectData>(serializer);
            if (type == nameof(MoveEffect)) return json.ToObject<MoveEffectData>(serializer);
            if (type == nameof(HookEffect)) return json.ToObject<HookEffectData>(serializer);
            if (type == nameof(SearchDummiesEffect)) return json.ToObject<SearchDummiesEffectData>(serializer);
            if (type == nameof(MirrorImageEffect)) return json.ToObject<MirrorImageEffectData>(serializer);
            if (type == nameof(RunCooldownEffect)) return json.ToObject<RunCooldownEffectData>(serializer);
            if (type == nameof(ReduceCooldownsEffect)) return json.ToObject<ReduceCooldownsEffectData>(serializer);
            if (type == nameof(RemoveBehaviourEffect)) return json.ToObject<RemoveBehaviourEffectData>(serializer);
            if (type == nameof(RemoveBehaviourStackEffect)) return json.ToObject<RemoveBehaviourStackEffectData>(serializer);
            if (type == nameof(StealBehaviourEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(DispelEffect)) return json.ToObject<DispelEffectData>(serializer);
            if (type == nameof(TeleportationEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(TeleportBehindTargetEffect)) return json.ToObject<EmptyEffectData>(serializer);
            if (type == nameof(DamageEffect)) return json.ToObject<DamageEffectData>(serializer);
            if (type == nameof(WeaponDamageEffect)) return json.ToObject<DamageEffectData>(serializer);
            if (type == nameof(RandomElementDamageEffect)) return json.ToObject<DamageEffectData>(serializer);
            if (type == nameof(PerBehaviourStackDamageEffect)) return json.ToObject<PerBehaviourStackDamageEffectData>(serializer);
            if (type == nameof(SuckInEffect)) return json.ToObject<SuckInEffectData>(serializer);
            if (type == nameof(HealFromTargetHealthEffect)) return json.ToObject<HealFromTargetHealthEffectData>(serializer);
            if (type == nameof(ChainEffect)) return json.ToObject<ChainEffectData>(serializer);
            if (type == nameof(HealEffect)) return json.ToObject<HealEffectData>(serializer);
            if (type == nameof(ShieldEffect)) return json.ToObject<ShieldEffectData>(serializer);
            if (type == nameof(RewardEffect)) return json.ToObject<RewardEffectData>(serializer);
            if (type == nameof(DragEffect)) return json.ToObject<DragEffectData>(serializer);
            if (type == nameof(AddCurrencyEffect)) return json.ToObject<AddCurrencyEffectData>(serializer);
            if (type == nameof(RunAwayEffect)) return json.ToObject<RunAwayEffectData>(serializer);
            if (type == nameof(KnockbackEffect)) return json.ToObject<KnockbackEffectData>(serializer);
            if (type == nameof(RestoreResourceEffect)) return json.ToObject<RestoreResourceEffectData>(serializer);
            if (type == nameof(LaunchMissileEffect)) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == nameof(LaunchWeaponMissileEffect)) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == nameof(LaunchMissileFromTargetEffect)) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == nameof(RepeatEffect)) return json.ToObject<RepeatEffectData>(serializer);
            if (type == nameof(SearchBehindEffect)) return json.ToObject<SearchBehindEffectData>(serializer);
            if (type == nameof(SearchPerimeterEffect)) return json.ToObject<SearchPerimeterEffectData>(serializer);
            if (type == nameof(SearchAreaEffect)) return json.ToObject<SearchAreaEffectData>(serializer);
            if (type == nameof(SearchPointsEffect)) return json.ToObject<SearchAreaEffectData>(serializer);
            if (type == nameof(SearchCorpsesEffect)) return json.ToObject<SearchAreaEffectData>(serializer);
            if (type == nameof(SearchLineEffect)) return json.ToObject<SearchLineEffectData>(serializer);
            if (type == nameof(SearchRandomPoints)) return json.ToObject<SearchRandomPointsData>(serializer);
            if (type == nameof(ApplyBehaviourEffect)) return json.ToObject<ApplyBehaviourEffectData>(serializer);
            if (type == nameof(CreateBeamEffect)) return json.ToObject<CreateBeamEffectData>(serializer);
            if (type == nameof(CreateSoundEffect)) return json.ToObject<CreateSoundEffectData>(serializer);
            if (type == nameof(CreateUnitEffect)) return json.ToObject<CreateUnitEffectData>(serializer);
            if (type == nameof(WaitEffect)) return json.ToObject<WaitEffectData>(serializer);
            if (type == nameof(RandomWaitEffect)) return json.ToObject<RandomWaitEffectData>(serializer);
            if (type == nameof(EffectSet)) return json.ToObject<EffectSetData>(serializer);
            if (type == nameof(RandomEffect)) return json.ToObject<RandomEffectData>(serializer);

            throw new InvalidDataException($"Unknown effect type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EffectData);
        }
    }
}