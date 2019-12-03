using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public abstract class FileRepository<TKey, TData, TEntity> : IRepository<TKey, TEntity> where TData : Identity<TKey>
    {
        protected readonly IFileReader Loader;
        protected readonly IMapper<TData, TEntity> Mapper;

        protected FileRepository(IFileReader loader, IMapper<TData, TEntity> mapper)
        {
            this.Loader = loader;
            this.Mapper = mapper;
        }

        protected abstract string GetFilename();

        public virtual void Save(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Save(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(TKey key)
        {
            throw new NotImplementedException();
        }

        public virtual TEntity Find(TKey key)
        {
            var entry = LoadData().FirstOrDefault(d => d.Id.Equals(key));

            return entry == null ? default : this.Mapper.ToEntity(entry);
        }

        public List<TEntity> Find(List<TKey> keys)
        {
            return LoadData().Where(d => keys.Contains(d.Id)).Select(this.Mapper.ToEntity).ToList();
        }

        public virtual TEntity FindOrFail(TKey key)
        {
            var result = Find(key);

            if (result == null)
            {
                throw new Exception($"Cant find {typeof(TEntity)} with id {key}");
            }

            return result;
        }

        public virtual List<TEntity> FindAll()
        {
            return LoadData().Select(this.Mapper.ToEntity).ToList();
        }

        protected virtual List<TData> LoadData()
        {
            try
            {
                return this.Loader.Read<List<TData>>(GetFilename()) ?? new List<TData>();
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error parsing: {GetFilename()}" + exception.Message);
            }

            return new List<TData>();
        }
    }
}