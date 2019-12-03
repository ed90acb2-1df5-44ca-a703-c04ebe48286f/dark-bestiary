using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.GameStates;
using DarkBestiary.Managers;
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
            View.Cancel += OnCancel;
            View.Create += OnCreate;
            View.Start += OnStart;

            CreateCharacters();
            View.RedrawCharacters(this.characters);
        }

        protected override void OnTerminate()
        {
            DestroyCharacters();

            View.DeleteCharacter -= OnDeleteCharacter;
            View.SelectCharacter -= OnSelectCharacter;
            View.Cancel -= OnCancel;
            View.Create -= OnCreate;
            View.Start -= OnStart;
        }

        private void CreateCharacters()
        {
            this.characters = this.characterRepository
                .FindAll()
                .OrderByDescending(character => character.Data.Timestamp)
                .ToList();

            foreach (var character in this.characters)
            {
                character.Entity.transform.position = new Vector3(-100, 0, 0);
                character.Entity.transform.localScale *= 3;
            }
        }

        private void DestroyCharacters()
        {
            foreach (var character in this.characters)
            {
                if (this.characterManager.Character == character)
                {
                    continue;
                }

                character.Entity.Terminate();
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
            if (this.selected.Data.IsDead)
            {
                return;
            }

            this.selected.Entity.transform.position = new Vector3(-100, 0, 0);
            this.selected.Entity.transform.localScale = Vector3.one;
            this.characterManager.Select(this.selected);
            Game.Instance.SwitchState(new TownGameState());
        }

        private void OnDeleteCharacter(Character character)
        {
            this.characterRepository.Delete(character.Id);
            this.characters.Remove(character);
            character.Entity.Terminate();
            View.RedrawCharacters(this.characters);
        }

        private void OnSelectCharacter(Character character)
        {
            if (this.selected != null)
            {
                this.selected.Entity.transform.position = new Vector3(-100, 0, 0);
            }

            this.selected = character;
            this.selected.Entity.transform.position = new Vector3(-7, -2, 0);
        }
    }
}