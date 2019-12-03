using System;
using DarkBestiary.Data.Repositories;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.Data.Mappers
{
    public class AttributeMapper : Mapper<AttributeData, Attribute>
    {
        private readonly IPropertyRepository propertyRepository;

        public AttributeMapper(IPropertyRepository propertyRepository)
        {
            this.propertyRepository = propertyRepository;
        }

        public override AttributeData ToData(Attribute target)
        {
            throw new NotImplementedException();
        }

        public override Attribute ToEntity(AttributeData data)
        {
            return new Attribute(data, this.propertyRepository);
        }
    }
}