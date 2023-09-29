using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class CharacterSelectionViewController : ViewController<ICharacterSelectionView>
    {
        private readonly ICharacterRepository m_CharacterRepository;

        private List<Character> m_Characters;
        private Character m_Selected;
        private int m_SelectedCharacterId;

        public CharacterSelectionViewController(ICharacterSelectionView view, ICharacterRepository repository) : base(view)
        {
            m_CharacterRepository = repository;
        }

        protected override void OnInitialize()
        {
            View.DeleteCharacter += OnDeleteCharacter;
            View.SelectCharacter += OnSelectCharacter;
            View.RenameCharacter += OnRenameCharacter;
            View.Cancel += OnCancel;
            View.Create += OnCreate;
            View.Start += OnStart;

            try
            {
                CreateCharacters();
            }
            catch (Exception exception)
            {
                UiErrorFrame.Instance.ShowException(exception);
                throw;
            }

            View.Refresh(m_Characters);
        }

        protected override void OnTerminate()
        {
            foreach (var character in m_Characters)
            {
                character.Entity.Terminate();
            }

            View.DeleteCharacter -= OnDeleteCharacter;
            View.SelectCharacter -= OnSelectCharacter;
            View.RenameCharacter -= OnRenameCharacter;
            View.Cancel -= OnCancel;
            View.Create -= OnCreate;
            View.Start -= OnStart;
        }

        private void CreateCharacters()
        {
            InventoryComponent.IsAutoIngredientStashEnabled = false;
            m_Characters = m_CharacterRepository.FindAll().OrderByDescending(character => character.Data.Timestamp).ToList();

            foreach (var character in m_Characters)
            {
                character.Entity.transform.position = new Vector3(-100, 0, 0);
                character.Entity.transform.localScale *= 3;
            }
        }

        private void OnCreate()
        {
            Game.Instance.ToCharacterCreation();
        }

        private void OnCancel()
        {
            Game.Instance.ToMainMenu();
        }

        private void OnStart()
        {
            InventoryComponent.IsAutoIngredientStashEnabled = true;
            var character = m_CharacterRepository.FindOrFail(m_SelectedCharacterId);
            character.Entity.transform.position = new Vector3(-100, 0, 0);
            character.Entity.transform.localScale = Vector3.one;

            Game.Instance.EnterTownWithNewCharacter(character);
        }

        private void OnRenameCharacter(Character character, string name)
        {
            character.Name = name;
            m_CharacterRepository.Save(character);
            View.Refresh(m_Characters);
        }

        private void OnDeleteCharacter(Character character)
        {
            m_CharacterRepository.Delete(character.Id);
            m_Characters.Remove(character);
            character.Entity.Terminate();
            View.Refresh(m_Characters);
        }

        private void OnSelectCharacter(Character character)
        {
            if (m_Selected != null)
            {
                m_Selected.Entity.SetActive(false);
                m_Selected.Entity.transform.position = new Vector3(-100, 0, 0);
            }

            m_Selected = character;
            m_Selected.Entity.transform.position = new Vector3(-7, -2, 0);
            m_Selected.Entity.SetActive(true);

            m_SelectedCharacterId = character.Id;
        }
    }
}