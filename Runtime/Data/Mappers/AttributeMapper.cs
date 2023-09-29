using System;
using DarkBestiary.Data.Repositories;
using Attribute = DarkBestiary.Attributes.Attribute;

namespace DarkBestiary.Data.Mappers
{
    public class AttributeMapper : Mapper<AttributeData, Attribute>
    {
        private readonly IPropertyRepository m_PropertyRepository;

        public AttributeMapper(IPropertyRepository propertyRepository)
        {
            m_PropertyRepository = propertyRepository;
        }

        public override AttributeData ToData(Attribute target)
        {
            throw new NotImplementedException();
        }

        public override Attribute ToEntity(AttributeData data)
        {
            return new Attribute(data, m_PropertyRepository);
        }
    }
}