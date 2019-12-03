using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DarkBestiary.Editor
{
    public class CreateMaterialsForTextures : UnityEditor.Editor
    {
        [MenuItem("Assets/Create/MaterialsForTextures")]
        private static void CreateMaterials()
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (var texture in Selection.GetFiltered(typeof(Texture), SelectionMode.Assets).Cast<Texture>())
                {
                    var path = AssetDatabase.GetAssetPath(texture);
                    path = path.Substring(0, path.LastIndexOf(".")) + ".mat";

                    if (AssetDatabase.LoadAssetAtPath(path, typeof(Material)) != null)
                    {
                        Debug.LogWarning("Can't create material, it already exists: " + path);
                        continue;
                    }

                    var material = new Material(Shader.Find("Diffuse")) {mainTexture = texture};
                    AssetDatabase.CreateAsset(material, path);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.SaveAssets();
            }
        }
    }
}