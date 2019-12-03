using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Validators;

namespace DarkBestiary.Dialogues
{
    public class Dialogue
    {
        public static event Payload<Dialogue> AnyDialogueRead;

        public int Id { get; }
        public bool IsParent { get; }
        public I18NString Title { get; }
        public I18NString Text { get; }
        public Narrator Narrator { get; }
        public IReadOnlyCollection<DialogueAction> Actions { get; }

        private readonly IReadOnlyCollection<Validator> validators;
        private readonly CharacterManager characterManager;

        public Dialogue(DialogueData data, IReadOnlyCollection<DialogueAction> actions,
            IReadOnlyCollection<Validator> validators, CharacterManager characterManager)
        {
            Id = data.Id;
            Title = I18N.Instance.Get(data.TitleKey);
            Text = I18N.Instance.Get(data.TextKey);
            Narrator = data.Narrator;
            IsParent = data.IsParent;
            Actions = actions;

            this.validators = validators;
            this.characterManager = characterManager;
        }

        public bool IsAvailable()
        {
            var character = this.characterManager.Character;

            if (character == null)
            {
                return false;
            }

            return this.validators.All(v => v.Validate(character.Entity, character.Entity));
        }

        public void OnRead()
        {
            if (!IsParent)
            {
                return;
            }

            var character = CharacterManager.Instance.Character;

            if (!character.Data.ReadDialogues.Contains(Id))
            {
                character.Data.ReadDialogues.Add(Id);
            }

            AnyDialogueRead?.Invoke(this);
        }
    }
}