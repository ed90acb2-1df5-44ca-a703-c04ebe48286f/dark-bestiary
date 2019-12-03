using DarkBestiary.Data;

namespace DarkBestiary.Dialogues
{
    public abstract class DialogueAction
    {
        public I18NString Text { get; }

        protected readonly DialogueActionData Data;

        protected DialogueAction(DialogueActionData data)
        {
            Text = I18N.Instance.Get(data.TextKey);

            this.Data = data;
        }

        public abstract void Execute();
    }
}