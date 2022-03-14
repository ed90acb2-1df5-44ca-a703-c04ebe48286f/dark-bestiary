using System;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace DarkBestiary.Analysis
{
    public class ProfilerSection : IDisposable
    {
        public ProfilerSection(string label)
        {
            Profiler.BeginSample(label);
        }

        public ProfilerSection(string label, Object target)
        {
            Profiler.BeginSample(label, target);
        }

        public void Dispose()
        {
            Profiler.EndSample();
        }
    }
}