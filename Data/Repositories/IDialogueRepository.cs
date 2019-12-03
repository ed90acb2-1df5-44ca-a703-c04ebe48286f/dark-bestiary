using System;
using System.Collections.Generic;
using DarkBestiary.Dialogues;

namespace DarkBestiary.Data.Repositories
{
    public interface IDialogueRepository : IRepository<int, Dialogue>
    {
        List<Dialogue> Find(Func<DialogueData, bool> predicate);
    }
}