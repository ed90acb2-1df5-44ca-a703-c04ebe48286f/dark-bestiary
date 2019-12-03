using System.Collections.Generic;

namespace DarkBestiary.Data.Mappers
{
    public interface IMapper<TData, TObject>
    {
        TData ToData(TObject target);

        List<TData> ToData(List<TObject> targets);

        TObject ToEntity(TData data);

        List<TObject> ToEntity(List<TData> targets);
    }
}