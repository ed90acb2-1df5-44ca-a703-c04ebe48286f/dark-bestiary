namespace DarkBestiary.Data.Readers
{
    public interface IFileReader
    {
        void Write<T>(T model, string path);

        T? Read<T>(string path);
    }
}