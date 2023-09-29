using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace DarkBestiary.Data.Readers
{
    public class BinaryFileReader : IFileReader
    {
        public void Write<T>(T model, string path)
        {
            var file = File.Create(path);

            var binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(file, model);

            file.Close();
        }

        public T Read<T>(string path)
        {
            var binaryFormatter = new BinaryFormatter();

            try
            {
                var file = File.Open(path, FileMode.Open);
                var data = binaryFormatter.Deserialize(file);

                file.Close();

                return (T) data;
            }
            catch (Exception exception)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                Debug.LogError($"Unable to load file: {path} message: {exception.Message}");
            }

            return default;
        }
    }
}