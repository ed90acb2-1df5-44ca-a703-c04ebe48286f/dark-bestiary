using System;
using System.Diagnostics;

namespace DarkBestiary.Analysis
{
    public class StopwatchSection : IDisposable
    {
        private readonly Stopwatch m_Stopwatch;
        private readonly string m_Label;

        public StopwatchSection(string label)
        {
            m_Stopwatch = Stopwatch.StartNew();
            m_Label = label;
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log($"{m_Label}: {m_Stopwatch.ElapsedMilliseconds}ms");
        }
    }
}