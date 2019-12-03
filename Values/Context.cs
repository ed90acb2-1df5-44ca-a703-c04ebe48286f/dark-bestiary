using System.Collections.Generic;

namespace DarkBestiary.Values
{
    public class Context
    {
        public static readonly Context Empty = new Context(new Dictionary<string, object>());

        private readonly Dictionary<string, object> payload;

        public Context()
        {
            this.payload = new Dictionary<string, object>();
        }

        public Context(Dictionary<string, object> payload)
        {
            this.payload = payload;
        }

        public T Get<T>(string key)
        {
            if (this.payload.ContainsKey(key))
            {
                return (T) this.payload[key];
            }

            return default(T);

        }

        public void Set(string key, object value)
        {
            this.payload.Remove(key);
            this.payload.Add(key, value);
        }
    }
}