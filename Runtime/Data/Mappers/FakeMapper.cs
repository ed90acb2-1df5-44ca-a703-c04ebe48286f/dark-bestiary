namespace DarkBestiary.Data.Mappers
{
    public class FakeMapper<T> : Mapper<T, T>
    {
        public override T ToData(T entity)
        {
            return entity;
        }

        public override T ToEntity(T data)
        {
            return data;
        }
    }
}