using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.UI.Elements;
using UnityEngine;

namespace DarkBestiary.UI.Views.Unity
{
    public class CharacterSelectionView : View, ICharacterSelectionView
    {
        public event Action Cancel;
        public event Action Create;
        public event Action Start;
        public event Action<Character, string> RenameCharacter;
        public event Action<Character> SelectCharacter;
        public event Action<Character> DeleteCharacter;

        [SerializeField] private CharacterRow m_CharacterPrefab;
        [SerializeField] private Transform m_CharacterContainer;
        [SerializeField] private RenameCharacterPopup m_RenamePopup;
        [SerializeField] private Interactable m_CreateButton;
        [SerializeField] private Interactable m_CancelButton;
        [SerializeField] private Interactable m_StartButton;
        [SerializeField] private GameObject m_BackgroundImagePrefab;

        private readonly List<CharacterRow> m_CharacterRows = new();

        private List<Character> m_Characters;
        private CharacterRow m_Selected;
        private CharacterRow m_Deleting;
        private GameObject m_BackgroundImage;

        public void Refresh(List<Character> characters)
        {
            m_Characters = characters;

            foreach (var characterRow in m_CharacterRows)
            {
                characterRow.Clicked -= OnCharacterClicked;
                characterRow.DoubleClicked -= OnCharacterDoubleClicked;
                characterRow.Delete -= OnCharacterDelete;
                characterRow.Edit -= OnCharacterEdit;
                Destroy(characterRow.gameObject);
            }

            m_CharacterRows.Clear();

            foreach (var character in m_Characters)
            {
                var characterRow = Instantiate(m_CharacterPrefab, m_CharacterContainer);
                characterRow.Initialize(character);
                characterRow.Deselect();
                characterRow.Clicked += OnCharacterClicked;
                characterRow.DoubleClicked += OnCharacterDoubleClicked;
                characterRow.Delete += OnCharacterDelete;
                characterRow.Edit += OnCharacterEdit;

                m_CharacterRows.Add(characterRow);
            }

            if (m_CharacterRows.Count > 0)
            {
                OnCharacterClicked(m_CharacterRows.First());
            }
        }

        protected override void OnInitialize()
        {
            m_BackgroundImage = Instantiate(m_BackgroundImagePrefab);
            // this.backgroundImage.transform.localScale = UIManager.Instance.ViewCanvas.localScale;

            m_CancelButton.PointerClick += OnCancelButtonPointerClick;
            m_CreateButton.PointerClick += OnCreateButtonPointerClick;
            m_StartButton.PointerClick += OnStartButtonPointerClick;

            m_RenamePopup.Confirmed += OnRenameConfirmed;
        }

        protected override void OnTerminate()
        {
            Destroy(m_BackgroundImage);

            m_CancelButton.PointerClick -= OnCancelButtonPointerClick;
            m_CreateButton.PointerClick -= OnCreateButtonPointerClick;
            m_StartButton.PointerClick -= OnStartButtonPointerClick;
        }

        private void OnCharacterClicked(CharacterRow characterRow)
        {
            if (m_Selected != null)
            {
                m_Selected.Deselect();
            }

            m_Selected = characterRow;
            m_Selected.Select();

            SelectCharacter?.Invoke(m_Selected.Character);
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
            RenameCharacter?.Invoke(m_Selected.Character, name);
        }

        private void OnCharacterEdit(CharacterRow characterRow)
        {
            m_RenamePopup.Show();
        }

        private void OnCharacterDelete(CharacterRow characterRow)
        {
            OnCharacterClicked(characterRow);

            m_Deleting = characterRow;

            ConfirmationWindow.Instance.Cancelled += OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed += OnDeleteConfirmed;
            ConfirmationWindow.Instance.Show(
                I18N.Instance.Get("ui_confirm_delete_x").ToString(characterRow.Character.Name),
                I18N.Instance.Get("ui_delete")
            );
        }

        private void OnDeleteConfirmed()
        {
            DeleteCharacter?.Invoke(m_Deleting.Character);

            OnDeleteCancelled();
        }

        private void OnDeleteCancelled()
        {
            m_Deleting = null;

            ConfirmationWindow.Instance.Cancelled -= OnDeleteCancelled;
            ConfirmationWindow.Instance.Confirmed -= OnDeleteConfirmed;
        }

        private void OnStartButtonPointerClick()
        {
            if (m_Selected == null)
            {
                return;
            }

            Start?.Invoke();
        }
    }
}