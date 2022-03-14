using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Managers;
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
        public event Payload<Character, string> RenameCharacter;
        public event Payload<Character> SelectCharacter;
        public event Payload<Character> DeleteCharacter;

        [SerializeField] private CharacterRow characterPrefab;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private RenameCharacterPopup renamePopup;
        [SerializeField] private Interactable createButton;
        [SerializeField] private Interactable cancelButton;
        [SerializeField] private Interactable startButton;
        [SerializeField] private GameObject backgroundImagePrefab;

        private readonly List<CharacterRow> characterRows = new List<CharacterRow>();

        private List<Character> characters;
        private CharacterRow selected;
        private CharacterRow deleting;
        private GameObject backgroundImage;

        public void Refresh(List<Character> characters)
        {
            this.characters = characters;

            foreach (var characterRow in this.characterRows)
            {
                characterRow.Clicked -= OnCharacterClicked;
                characterRow.DoubleClicked -= OnCharacterDoubleClicked;
                characterRow.Delete -= OnCharacterDelete;
                characterRow.Edit -= OnCharacterEdit;
                Destroy(characterRow.gameObject);
            }

            this.characterRows.Clear();

            foreach (var character in this.characters)
            {
                var characterRow = Instantiate(this.characterPrefab, this.characterContainer);
                characterRow.Initialize(character);
                characterRow.Deselect();
                characterRow.Clicked += OnCharacterClicked;
                characterRow.DoubleClicked += OnCharacterDoubleClicked;
                characterRow.Delete += OnCharacterDelete;
                characterRow.Edit += OnCharacterEdit;

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
            // this.backgroundImage.transform.localScale = UIManager.Instance.ViewCanvas.localScale;

            this.cancelButton.PointerClick += OnCancelButtonPointerClick;
            this.createButton.PointerClick += OnCreateButtonPointerClick;
            this.startButton.PointerClick += OnStartButtonPointerClick;

            this.renamePopup.Confirmed += OnRenameConfirmed;
        }

        protected override void OnTerminate()
        {
            Destroy(this.backgroundImage);

            this.cancelButton.PointerClick -= OnCancelButtonPointerClick;
            this.createButton.PointerClick -= OnCreateButtonPointerClick;
            this.startButton.PointerClick -= OnStartButtonPointerClick;
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
            OnStartButtonPointerClick();
        }

        private void OnCreateButtonPointerClick()
        {
            Create?.Invoke();
        }

        private void OnCancelButtonPointerClick()
        {
            Cancel?.Invoke();
        }

        private void OnRenameConfirmed(string name)
        {
            RenameCharacter?.Invoke(this.selected.Character, name);
        }

        private void OnCharacterEdit(CharacterRow characterRow)
        {
            this.renamePopup.Show();
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

        private void OnStartButtonPointerClick()
        {
            if (this.selected == null)
            {
                return;
            }

            Start?.Invoke();
        }
    }
}