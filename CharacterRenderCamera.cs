using DarkBestiary.Components;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary
{
    public class CharacterRenderCamera : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;

        private ActorComponent actor;

        private void Start()
        {
            CharacterManager.CharacterSelected += OnCharacterSelected;
        }

        private void OnCharacterSelected(Character character)
        {
            this.actor = character.Entity.GetComponent<ActorComponent>();
        }

        private void Update()
        {
            if (this.actor == null)
            {
                return;
            }

            transform.position = this.actor.Model.transform.position + this.offset;
            transform.rotation = this.actor.Model.transform.rotation;
        }
    }
}