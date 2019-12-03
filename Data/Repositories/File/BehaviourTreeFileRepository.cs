using DarkBestiary.AI;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using UnityEngine;

namespace DarkBestiary.Data.Repositories.File
{
    public class BehaviourTreeFileRepository : FileRepository<int, BehaviourTreeData, BehaviourTree>, IBehaviourTreeRepository
    {
        public BehaviourTreeFileRepository(IFileReader loader, BehaviourTreeMapper mapper) : base(loader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Application.streamingAssetsPath + "/data/ai.json";
        }
    }
}