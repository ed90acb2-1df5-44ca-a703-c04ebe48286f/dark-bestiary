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
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static MasteryMapper()
        {
            Assembly.GetAssembly(typeof(Mastery))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(Mastery)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => Mapping.Add(type.Name, type));
        }

        private readonly IItemModifierRepository itemModifierRepository;

        public MasteryMapper(IItemModifierRepository itemModifierRepository)
        {
            this.itemModifierRepository = itemModifierRepository;
        }

        public override MasteryData ToData(Mastery entity)
        {
            throw new NotImplementedException();
        }

        public override Mastery ToEntity(MasteryData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception("Unknown mastery type " + data.Type);
            }

            try
            {
                return Container.Instance.Instantiate(
                    Mapping[data.Type],
                    new object[]
                    {
                        data,
                        this.itemModifierRepository.Find(data.ModifierId)
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