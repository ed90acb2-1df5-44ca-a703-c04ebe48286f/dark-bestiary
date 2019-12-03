using System.Collections.Generic;
using System.Linq;

namespace DarkBestiary.Data.Mappers
{
    public abstract class Mapper<TData, TEntity> : IMapper<TData, TEntity>
    {
        public abstract TData ToData(TEntity entity);

        public List<TData> ToData(List<TEntity> targets)
        {
            return targets.Select(ToData).ToList();
        }

        public abstract TEntity ToEntity(TData data);

        public List<TEntity> ToEntity(List<TData> targets)
        {
            return targets.Select(ToEntity).ToList();
        }
    }
}