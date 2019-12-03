using System;
using System.IO;
using DarkBestiary.Validators;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBestiary.Data.Readers.Json
{
    public class ValidatorJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var type = json["Type"].Value<string>();

            if (type == typeof(UnitCountValidator).Name) return json.ToObject<UnitCountValidatorData>(serializer);
            if (type == typeof(TargetIsEnemyValidator).Name) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == typeof(TargetIsAllyValidator).Name) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == typeof(TargetIsVisibleValidator).Name) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == typeof(ScenarioIsCompletedValidator).Name) return json.ToObject<ScenarioValidatorData>(serializer);
            if (type == typeof(TargetChallengeRatingValidator).Name) return json.ToObject<ValueValidatorData>(serializer);
            if (type == typeof(UnitsWithBehaviourValidator).Name) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == typeof(TargetBehaviourCountValidator).Name) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == typeof(CasterBehaviourCountValidator).Name) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == typeof(CasterHaveStatusFlagValidator).Name) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == typeof(CasterHaveNotStatusFlagValidator).Name) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == typeof(TargetHaveStatusFlagValidator).Name) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == typeof(TargetHealthFractionValidator).Name) return json.ToObject<TargetHealthFractionValidatorData>(serializer);
            if (type == typeof(TargetHaveNotStatusFlagValidator).Name) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == typeof(TargetHasOverhealValidator).Name) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == typeof(TargetHasUsedSkillThisRound).Name) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == typeof(TargetIsUnitOfTypeValidator).Name) return json.ToObject<UnitValidatorData>(serializer);
            if (type == typeof(TargetUnitHaveFlagsValidator).Name) return json.ToObject<UnitFlagsValidatorData>(serializer);
            if (type == typeof(TargetUnitHaveNotFlagsValidator).Name) return json.ToObject<UnitFlagsValidatorData>(serializer);
            if (type == typeof(TargetInRangeValidator).Name) return json.ToObject<InRangeValidatorData>(serializer);
            if (type == typeof(CombineOrValidator).Name) return json.ToObject<CombineValidatorsData>(serializer);
            if (type == typeof(CombineAndValidator).Name) return json.ToObject<CombineValidatorsData>(serializer);

            throw new InvalidDataException($"Unknown validator type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ValidatorData);
        }
    }
}