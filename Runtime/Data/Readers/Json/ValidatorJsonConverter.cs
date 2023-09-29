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

            if (type == nameof(UnitCountValidator)) return json.ToObject<UnitCountValidatorData>(serializer);
            if (type == nameof(TargetIsEnemyValidator)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetIsAllyValidator)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetHasSummonedComponent)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetHasNoSummonedComponent)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetIsVisibleValidator)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetInWeaponRangeValidator)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetActionPointsValidator)) return json.ToObject<ValueValidatorData>(serializer);
            if (type == nameof(TargetChallengeRatingValidator)) return json.ToObject<ValueValidatorData>(serializer);
            if (type == nameof(UnitsWithBehaviourValidator)) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == nameof(TargetBehaviourCountValidator)) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == nameof(CasterBehaviourCountValidator)) return json.ToObject<BehaviourCountValidatorData>(serializer);
            if (type == nameof(CasterHaveStatusFlagValidator)) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == nameof(CasterHaveNotStatusFlagValidator)) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == nameof(TargetHaveStatusFlagValidator)) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == nameof(TargetHealthFractionValidator)) return json.ToObject<TargetHealthFractionValidatorData>(serializer);
            if (type == nameof(TargetHaveNotStatusFlagValidator)) return json.ToObject<StatusFlagsValidatorData>(serializer);
            if (type == nameof(TargetHasOverhealValidator)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetHasUsedSkillThisRound)) return json.ToObject<EmptyValidatorData>(serializer);
            if (type == nameof(TargetIsUnitOfTypeValidator)) return json.ToObject<UnitValidatorData>(serializer);
            if (type == nameof(TargetIsNotUnitOfTypeValidator)) return json.ToObject<UnitValidatorData>(serializer);
            if (type == nameof(TargetUnitHaveFlagsValidator)) return json.ToObject<UnitFlagsValidatorData>(serializer);
            if (type == nameof(TargetUnitHaveNotFlagsValidator)) return json.ToObject<UnitFlagsValidatorData>(serializer);
            if (type == nameof(TargetInRangeValidator)) return json.ToObject<InRangeValidatorData>(serializer);
            if (type == nameof(CombineOrValidator)) return json.ToObject<CombineValidatorsData>(serializer);
            if (type == nameof(CombineAndValidator)) return json.ToObject<CombineValidatorsData>(serializer);

            throw new InvalidDataException($"Unknown validator type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ValidatorData);
        }
    }
}