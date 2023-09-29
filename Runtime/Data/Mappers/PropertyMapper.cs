using System;
using DarkBestiary.Properties;

namespace DarkBestiary.Data.Mappers
{
    public class PropertyMapper : Mapper<PropertyData, Property>
    {
        public override PropertyData ToData(Property target)
        {
            throw new NotImplementedException();
        }

        public override Property ToEntity(PropertyData data)
        {
            return new Property(data);
        }
    }
}