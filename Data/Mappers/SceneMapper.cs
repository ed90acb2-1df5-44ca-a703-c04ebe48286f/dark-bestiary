using DarkBestiary.Scenarios.Scenes;
using UnityEngine;

namespace DarkBestiary.Data.Mappers
{
    public class SceneMapper : Mapper<SceneData, Scene>
    {
        private static readonly Transform Container = new GameObject("Scenes").transform;

        public SceneMapper()
        {
        }

        public override SceneData ToData(Scene entity)
        {
            throw new System.NotImplementedException();
        }

        public override Scene ToEntity(SceneData data)
        {
            var scene = Object.Instantiate(Resources.Load<Scene>(data.Prefab.Replace(".prefab", "")), Container);
            scene.Construct(data);

            return scene;
        }
    }
}