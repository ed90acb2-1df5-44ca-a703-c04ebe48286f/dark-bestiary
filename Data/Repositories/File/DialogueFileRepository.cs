using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Mappers;
using DarkBestiary.Data.Readers;
using DarkBestiary.Dialogues;

namespace DarkBestiary.Data.Repositories.File
{
    public class DialogueFileRepository : FileRepository<int, DialogueData, Dialogue>, IDialogueRepository
    {
        public DialogueFileRepository(IFileReader reader, DialogueMapper mapper) : base(reader, mapper)
        {
        }

        protected override string GetFilename()
        {
            return Environment.StreamingAssetsPath + "/compiled/data/dialogues.json";
        }

        public List<Dialogue> Find(Func<DialogueData, bool> predicate)
        {
            return LoadData()
                .Where(predicate)
                .Select(this.Mapper.ToEntity)
                .ToList();
        }
    }
}