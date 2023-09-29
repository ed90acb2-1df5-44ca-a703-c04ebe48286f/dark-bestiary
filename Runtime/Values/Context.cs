using System.Collections.Generic;

namespace DarkBestiary.Values
{
    public class Context
    {
        public static readonly Context s_Empty = new(new Dictionary<string, object>());

        private readonly Dictionary<string, object> m_Payload;

        public Context()
        {
            m_Payload = new Dictionary<string, object>();
        }

        public Context(Dictionary<string, object> payload)
        {
            m_Payload = payload;
        }

        public T Get<T>(string key)
        {
            if (m_Payload.ContainsKey(key))
            {
                return (T) m_Payload[key];
            }

            return default(T);

        }

        public void Set(string key, object value)
        {
            m_Payload.Remove(key);
            m_Payload.Add(key, value);
        }
    }
}