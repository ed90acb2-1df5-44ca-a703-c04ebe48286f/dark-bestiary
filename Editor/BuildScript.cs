using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    public class BuildScript : MonoBehaviour
    {
        [MenuItem("Build/Everything")]
        public static void Build()
        {
            var levels = new[] { "Assets/main.unity" };

            BuildPipeline.BuildPlayer(
                levels, $"Builds/Windows/{Application.productName}.exe", BuildTarget.StandaloneWindows64, BuildOptions.None);
            BuildPipeline.BuildPlayer(
                levels, $"Builds/Linux/{Application.productName}.x64_86", BuildTarget.StandaloneLinux64, BuildOptions.None);
            BuildPipeline.BuildPlayer(
                levels, $"Builds/Mac/{Application.productName}.app", BuildTarget.StandaloneOSX, BuildOptions.None);
        }
    }
}