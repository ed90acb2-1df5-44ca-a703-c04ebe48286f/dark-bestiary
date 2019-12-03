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

            if (type == typeof(LevelupReward).Name) return json.ToObject<LevelupRewardData>(serializer);
            if (type == typeof(RewardCollection).Name) return json.ToObject<RewardCollectionData>(serializer);
            if (type == typeof(AttributesReward).Name) return json.ToObject<AttributesRewardData>(serializer);
            if (type == typeof(PropertiesReward).Name) return json.ToObject<PropertiesRewardData>(serializer);
            if (type == typeof(RandomSkillsUnlockReward).Name) return json.ToObject<RandomSkillsUnlockRewardData>(serializer);
            if (type == typeof(ItemsReward).Name) return json.ToObject<ItemsRewardData>(serializer);
            if (type == typeof(TalentPointsReward).Name) return json.ToObject<TalentPointsRewardData>(serializer);
            if (type == typeof(CurrenciesReward).Name) return json.ToObject<CurrenciesRewardData>(serializer);
            if (type == typeof(AttributePointsReward).Name) return json.ToObject<AttributePointsRewardData>(serializer);

            throw new InvalidDataException($"Unknown reward type {type}");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RewardData);
        }
    }
}