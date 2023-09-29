using DarkBestiary.AI;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;

namespace DarkBestiary.Data.Repositories.File
{
    public class BehaviourTreeFileRepository : FileRepository<int, BehaviourTreeData, BehaviourTree>, IBehaviourTreeRepository
    {
        public BehaviourTreeFileRepository(IFileReader reader, BehaviourTreeMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.s_StreamingAssetsPath + "/compiled/data/ai.json";
        }
    }
}