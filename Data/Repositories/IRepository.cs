using System;
using System.Collections.Generic;

namespace DarkBestiary.Data.Repositories
{
    public interface IRepository<TKey, TValue>
    {
        void Save(TValue data);

        void Save(List<TValue> data);

        void Delete(TKey key);

        TValue Find(TKey key);

        List<TValue> Find(List<TKey> keys);

        List<TValue> FindAll();

        TValue FindOrFail(TKey key);
    }
}