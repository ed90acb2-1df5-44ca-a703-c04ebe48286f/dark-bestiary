using System.Collections.Generic;
using DarkBestiary.Analytics;
using UnityEngine;
using Zenject;

namespace DarkBestiary.Managers
{
    public class UnityExceptionManager : IInitializable
    {
        private readonly IAnalyticsService analytics;

        public UnityExceptionManager(IAnalyticsService analytics)
        {
            this.analytics = analytics;
        }

        public void Initialize()
        {
            Application.logMessageReceived += Handler;
        }

        private void Handler(string condition, string stacktrace, LogType type)
        {
            if (type == LogType.Log || type == LogType.Warning || string.IsNullOrEmpty(stacktrace))
            {
                return;
            }

            this.analytics.Event("db_exception", new Dictionary<string, object>
            {
                {"Condition", condition},
                {"Trace", stacktrace},
            });
        }
    }
}