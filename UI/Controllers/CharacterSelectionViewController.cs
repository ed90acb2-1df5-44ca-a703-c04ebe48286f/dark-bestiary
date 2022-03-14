using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views;
using UnityEngine;

namespace DarkBestiary.UI.Controllers
{
    public class CharacterSelectionViewController : ViewController<ICharacterSelectionView>
    {
        private readonly ICharacterRepository characterRepository;
        private readonly CharacterManager characterManager;

        private List<Character> characters;
        private Character selected;
        private int selectedCharacterId;

        public CharacterSelectionViewController(ICharacterSelectionView view, ICharacterRepository repository,
            CharacterManager characterManager) : base(view)
        {
            this.characterRepository = repository;
            this.characterManager = characterManager;
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

            View.Refresh(this.characters);
        }

        protected override void OnTerminate()
        {
            foreach (var character in this.characters)
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
            this.characters = this.characterRepository.FindAll().OrderByDescending(character => character.Data.Timestamp).ToList();

            foreach (var character in this.characters)
            {
                character.Entity.transform.position = new Vector3(-100, 0, 0);
                character.Entity.transform.localScale *= 3;
            }
        }

        private void OnCreate()
        {
            Game.Instance.SwitchState(new CharacterCreationGameState());
        }

        private void OnCancel()
        {
            Game.Instance.SwitchState(new MainMenuGameState());
        }

        private void OnStart()
        {
            InventoryComponent.IsAutoIngredientStashEnabled = true;
            var character = this.characterRepository.Find(this.selectedCharacterId);
            character.Entity.transform.position = new Vector3(-100, 0, 0);
            character.Entity.transform.localScale = Vector3.one;
            this.characterManager.Select(character);

            Game.Instance.SwitchState(new TownGameState());
        }

        private void OnRenameCharacter(Character character, string name)
        {
            character.Name = name;
            this.characterRepository.Save(character);
            View.Refresh(this.characters);
        }

        private void OnDeleteCharacter(Character character)
        {
            this.characterRepository.Delete(character.Id);
            this.characters.Remove(character);
            character.Entity.Terminate();
            View.Refresh(this.characters);
        }

        private void OnSelectCharacter(Character character)
        {
            if (this.selected != null)
            {
                this.selected.Entity.SetActive(false);
                this.selected.Entity.transform.position = new Vector3(-100, 0, 0);
            }

            this.selected = character;
            this.selected.Entity.transform.position = new Vector3(-7, -2, 0);
            this.selected.Entity.SetActive(true);

            this.selectedCharacterId = character.Id;
        }
    }
}