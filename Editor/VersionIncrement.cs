using UnityEditor;
using Debug = UnityEngine.Debug;

namespace DarkBestiary.Editor
{
    public static class VersionIncrement
    {
        [UnityEditor.Callbacks.DidReloadScripts]
        public static void OnScriptsReloaded()
        {
            return;
            IncrementVersion(revision: 1);
        }

        private static void IncrementVersion(int? major = null, int? minor = null, int? build = null, int? revision = null)
        {
            var currentVersion = new System.Version(PlayerSettings.bundleVersion);

            currentVersion = new System.Version(
                (currentVersion.Major < 0 ? 0 : currentVersion.Major) + (major ?? 0),
                (currentVersion.Minor < 0 ? 0 : currentVersion.Minor) + (minor ?? 0),
                (currentVersion.Build < 0 ? 0 : currentVersion.Build) + (build ?? 0),
                (currentVersion.Revision < 0 ? 0 : currentVersion.Revision) + (revision ?? 0)
            );

            PlayerSettings.bundleVersion = currentVersion.ToString();

            Debug.Log($"Revision: {currentVersion.Revision.ToString()}");
        }
    }
}