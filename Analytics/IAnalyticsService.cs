using System.Collections.Generic;

namespace DarkBestiary.Analytics
{
    public interface IAnalyticsService
    {
        void Event(string name, Dictionary<string, object> payload);
    }
}