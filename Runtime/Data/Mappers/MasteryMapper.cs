using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Masteries;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class MasteryMapper : Mapper<MasteryData, Mastery>
    {
        private static readonly Dictionary<string, Type> s_Mapping = new();

        static MasteryMapper()
        {
            Assembly.GetAssembly(typeof(Mastery))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Mastery)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => s_Mapping.Add(type.Name, type));
        }

        private readonly IItemModifierRepository m_ItemModifierRepository;

        public MasteryMapper(IItemModifierRepository itemModifierRepository)
        {
            m_ItemModifierRepository = itemModifierRepository;
        }

        public override MasteryData ToData(Mastery entity)
        {
            throw new NotImplementedException();
        }

        public override Mastery ToEntity(MasteryData data)
        {
            if (!s_Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown mastery type " + data.Type);
            }

            try
            {
                return Container.Instance.Instantiate(
                    s_Mapping[data.Type],
                    new object[]
                    {
                        data,
                        m_ItemModifierRepository.Find(data.ModifierId)
                    }) as Mastery;
                ;
            }
            catch (Exception exception)
            {
                Debug.LogError(data.Type + $" ({data.NameKey}): " + exception.Message);
                throw;
            }
        }
    }
}