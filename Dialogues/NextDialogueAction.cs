using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.Dialogues
{
    public class NextDialogueAction : DialogueAction
    {
        private readonly IDialogueRepository dialogueRepository;

        public NextDialogueAction(DialogueActionData data, IDialogueRepository dialogueRepository) : base(data)
        {
            this.dialogueRepository = dialogueRepository;
        }

        public override void Execute()
        {
            DialogueView.Instance.Show(this.dialogueRepository.Find(this.Data.NextDialogueId));
        }
    }
}