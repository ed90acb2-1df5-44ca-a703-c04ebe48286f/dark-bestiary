using System;
using System.IO;
using DarkBestiary.Rewards;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBestiary.Data.Readers.Json
{
    public class RewardJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var json = JObject.Load(reader);
            var type = json["Type"].Value<string>();

            if (type == nameof(LevelupReward)) return json.ToObject<LevelupRewardData>(serializer);
            if (type == nameof(RewardCollection)) return json.ToObject<RewardCollectionData>(serializer);
            if (type == nameof(AttributesReward)) return json.ToObject<AttributesRewardData>(serializer);
            if (type == nameof(PropertiesReward)) return json.ToObject<PropertiesRewardData>(serializer);
            if (type == nameof(RandomSkillsUnlockReward)) return json.ToObject<RandomSkillsUnlockRewardData>(serializer);
            if (type == nameof(ItemsReward)) return json.ToObject<ItemsRewardData>(serializer);
            if (type == nameof(TalentPointsReward)) return json.ToObject<TalentPointsRewardData>(serializer);
            if (type == nameof(CurrenciesReward)) return json.ToObject<CurrenciesRewardData>(serializer);
            if (type == nameof(AttributePointsReward)) return json.ToObject<AttributePointsRewardData>(serializer);

            throw new InvalidDataException($"Unknown reward type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RewardData);
        }
    }
}