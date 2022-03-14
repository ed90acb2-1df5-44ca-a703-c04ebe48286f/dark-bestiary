using System;
using System.Diagnostics;

namespace DarkBestiary.Analysis
{
    public class StopwatchSection : IDisposable
    {
        private readonly Stopwatch stopwatch;
        private readonly string label;

        public StopwatchSection(string label)
        {
            this.stopwatch = Stopwatch.StartNew();
            this.label = label;
        }

        public void Dispose()
        {
            UnityEngine.Debug.Log($"{this.label}: {this.stopwatch.ElapsedMilliseconds}ms");
        }
    }
}