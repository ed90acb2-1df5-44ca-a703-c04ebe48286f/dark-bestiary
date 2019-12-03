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

            if (type == typeof(KillEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(ShowEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(HideEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(AttackEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(RemoveShieldsEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(LaunchAOEMissileEffect).Name) return json.ToObject<LaunchAOEMissileEffectData>(serializer);
            if (type == typeof(IfElseEffect).Name) return json.ToObject<IfElseEffectData>(serializer);
            if (type == typeof(MoveEffect).Name) return json.ToObject<MoveEffectData>(serializer);
            if (type == typeof(HookEffect).Name) return json.ToObject<HookEffectData>(serializer);
            if (type == typeof(SearchDummiesEffect).Name) return json.ToObject<SearchDummiesEffectData>(serializer);
            if (type == typeof(MirrorImageEffect).Name) return json.ToObject<MirrorImageEffectData>(serializer);
            if (type == typeof(RunCooldownEffect).Name) return json.ToObject<RunCooldownEffectData>(serializer);
            if (type == typeof(ReduceCooldownsEffect).Name) return json.ToObject<ReduceCooldownsEffectData>(serializer);
            if (type == typeof(RemoveBehaviourEffect).Name) return json.ToObject<RemoveBehaviourEffectData>(serializer);
            if (type == typeof(StealBehaviourEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(DispelEffect).Name) return json.ToObject<DispelEffectData>(serializer);
            if (type == typeof(TeleportationEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(TeleportBehindTargetEffect).Name) return json.ToObject<EmptyEffectData>(serializer);
            if (type == typeof(DamageEffect).Name) return json.ToObject<DamageEffectData>(serializer);
            if (type == typeof(WeaponDamageEffect).Name) return json.ToObject<DamageEffectData>(serializer);
            if (type == typeof(RandomElementDamageEffect).Name) return json.ToObject<DamageEffectData>(serializer);
            if (type == typeof(PerBehaviourStackDamageEffect).Name) return json.ToObject<PerBehaviourStackDamageEffectData>(serializer);
            if (type == typeof(SuckInEffect).Name) return json.ToObject<SuckInEffectData>(serializer);
            if (type == typeof(HealFromTargetHealthEffect).Name) return json.ToObject<HealFromTargetHealthEffectData>(serializer);
            if (type == typeof(ChainEffect).Name) return json.ToObject<ChainEffectData>(serializer);
            if (type == typeof(HealEffect).Name) return json.ToObject<HealEffectData>(serializer);
            if (type == typeof(ShieldEffect).Name) return json.ToObject<ShieldEffectData>(serializer);
            if (type == typeof(RewardEffect).Name) return json.ToObject<RewardEffectData>(serializer);
            if (type == typeof(DragEffect).Name) return json.ToObject<DragEffectData>(serializer);
            if (type == typeof(AddCurrencyEffect).Name) return json.ToObject<AddCurrencyEffectData>(serializer);
            if (type == typeof(RunAwayEffect).Name) return json.ToObject<RunAwayEffectData>(serializer);
            if (type == typeof(KnockbackEffect).Name) return json.ToObject<KnockbackEffectData>(serializer);
            if (type == typeof(RestoreResourceEffect).Name) return json.ToObject<RestoreResourceEffectData>(serializer);
            if (type == typeof(LaunchMissileEffect).Name) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == typeof(LaunchWeaponMissile).Name) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == typeof(LaunchMissileFromTargetEffect).Name) return json.ToObject<LaunchMissileEffectData>(serializer);
            if (type == typeof(RepeatEffect).Name) return json.ToObject<RepeatEffectData>(serializer);
            if (type == typeof(SearchBehindEffect).Name) return json.ToObject<SearchBehindEffectData>(serializer);
            if (type == typeof(SearchPerimeterEffect).Name) return json.ToObject<SearchPerimeterEffectData>(serializer);
            if (type == typeof(SearchAreaEffect).Name) return json.ToObject<SearchAreaEffectData>(serializer);
            if (type == typeof(SearchPointsEffect).Name) return json.ToObject<SearchAreaEffectData>(serializer);
            if (type == typeof(SearchLineEffect).Name) return json.ToObject<SearchLineEffectData>(serializer);
            if (type == typeof(SearchRandomPoints).Name) return json.ToObject<SearchRandomPointsData>(serializer);
            if (type == typeof(ApplyBehaviourEffect).Name) return json.ToObject<ApplyBehaviourEffectData>(serializer);
            if (type == typeof(CreateBeamEffect).Name) return json.ToObject<CreateBeamEffectData>(serializer);
            if (type == typeof(CreateSoundEffect).Name) return json.ToObject<CreateSoundEffectData>(serializer);
            if (type == typeof(CreateUnitEffect).Name) return json.ToObject<CreateUnitEffectData>(serializer);
            if (type == typeof(WaitEffect).Name) return json.ToObject<WaitEffectData>(serializer);
            if (type == typeof(RandomWaitEffect).Name) return json.ToObject<RandomWaitEffectData>(serializer);
            if (type == typeof(EffectSet).Name) return json.ToObject<EffectSetData>(serializer);
            if (type == typeof(RandomEffect).Name) return json.ToObject<RandomEffectData>(serializer);

            throw new InvalidDataException($"Unknown effect type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(EffectData);
        }
    }
}