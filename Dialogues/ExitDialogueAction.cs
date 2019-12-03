using DarkBestiary.Data;
using DarkBestiary.UI.Elements;

namespace DarkBestiary.Dialogues
{
    public class ExitDialogueAction : DialogueAction
    {
        public ExitDialogueAction(DialogueActionData data) : base(data)
        {
        }

        public override void Execute()
        {
            DialogueView.Instance.Hide();
        }
    }
}