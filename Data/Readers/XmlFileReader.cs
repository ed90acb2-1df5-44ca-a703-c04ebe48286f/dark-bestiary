using System.IO;
using System.Xml.Serialization;

namespace DarkBestiary.Data.Readers
{
    public class XmlFileReader : IFileReader
    {
        public void Write<T>(T model, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stream = new FileStream(path, FileMode.Create);

            serializer.Serialize(stream, model);

            stream.Close();
        }

        public T Read<T>(string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var stream = new FileStream(path, FileMode.Open);
            var model = (T) serializer.Deserialize(stream);

            stream.Close();

            return model;
        }
    }
}