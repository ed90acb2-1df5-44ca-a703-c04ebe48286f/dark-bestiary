using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Messaging;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class CharacterSelectionView : View, ICharacterSelectionView
    {
        public event Payload Cancel;
        public event Payload Create;
        public event Payload Start;
        public event Payload<Character> SelectCharacter;
        public event Payload<Character> DeleteCharacter;

        [SerializeField] private CharacterRow characterPrefab;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private Interactable createButton;
        [SerializeField] private Interactable cancelButton;
        [SerializeField] private Interactable startButton;
        [SerializeField] private GameObject backgroundImagePrefab;

        private readonly List<CharacterRow> characterRows = new List<CharacterRow>();

        private CharacterRow selected;
        private CharacterRow deleting;
        private GameObject backgroundImage;

        public void RedrawCharacters(List<Character> characters)
        {
            foreach (var characterRow in this.characterRows)
            {
                characterRow.Clicked -= OnCharacterClicked;
                characterRow.DoubleClicked -= OnCharacterDoubleClicked;
                Destroy(characterRow.gameObject);
            }

            this.characterRows.Clear();

            foreach (var character in characters)
            {
                var characterRow = Instantiate(this.characterPrefab, this.characterContainer);
                characterRow.Initialize(character);
                characterRow.Deselect();
                characterRow.Clicked += OnCharacterClicked;
                characterRow.DoubleClicked += OnCharacterDoubleClicked;
                characterRow.Delete += OnCharacterDelete;

                this.characterRows.Add(characterRow);
            }

            if (this.characterRows.Count > 0)
            {
                OnCharacterClicked(this.characterRows.First());
            }
        }

        protected override void OnInitialize()
        {
            this.backgroundImage = Instantiate(this.backgroundImagePrefab);

            this.cancelButton.PointerUp += OnCancelButtonPointerUp;
            this.createButton.PointerUp += OnCreateButtonPointerUp;
            this.startButton.PointerUp += OnStartButtonPointerUp;
        }

        protected override void OnTerminate()
        {
            Destroy(this.backgroundImage);

            this.cancelButton.PointerUp -= OnCancelButtonPointerUp;
            this.createButton.PointerUp -= OnCreateButtonPointerUp;
            this.startButton.PointerUp -= OnStartButtonPointerUp;
        }

        private void OnCharacterClicked(CharacterRow characterRow)
        {
            if (this.selected != null)
            {
                this.selected.Deselect();
            }

            this.selected = characterRow;
            this.selected.Select();

            SelectCharacter?.Invoke(this.selected.Character);
        }

        private void OnCharacterDoubleClicked(CharacterRow characterRow)
        {
            OnStartButtonPointerUp();
        }

        private void OnCreateButtonPointerUp()
        {
            Create?.Invoke();
        }

        private void OnCancelButtonPointerUp()
        {
            Cancel?.Invoke();
        }

        private void OnCharacterDelete(CharacterRow characterRow)
        {
            OnCharacterClicked(characterRow);

            this.deleting = characterRow;

            ConfirmationWindow.Instance.Cancelled += OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed += OnDeleteConfirmed;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_confirm_delete_x").ToString(characterRow.Character.Name),
                I18N.Instance.Get("ui_delete")
            );
        }

        private void OnDeleteConfirmed()
        {
            DeleteCharacter?.Invoke(this.deleting.Character);

            OnDeleteCancelled();
        }

        private void OnDeleteCancelled()
        {
            this.deleting = null;

            ConfirmationWindow.Instance.Cancelled -= OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnDeleteConfirmed;
        }

        private void OnStartButtonPointerUp()
        {
            if (this.selected == null)
            {
                return;
            }

            Start?.Invoke();
        }
    }
}