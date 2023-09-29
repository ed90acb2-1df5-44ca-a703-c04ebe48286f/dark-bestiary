using DarkBestiary.Components;
using UnityEngine;

namespace DarkBestiary
{
    public class CharacterRenderCamera : MonoBehaviour
    {
        [SerializeField]
        private Vector3 m_Offset;

        private ActorComponent m_Actor;

        private void Start()
        {
            Game.Instance.CharacterSwitched += OnCharacterSwitched;
        }

        private void OnCharacterSwitched()
        {
            m_Actor = Game.Instance.Character.Entity.GetComponent<ActorComponent>();
        }

        private void Update()
        {
            if (m_Actor == null)
            {
                return;
            }

            transform.position = m_Actor.Model.transform.position + m_Offset;
            transform.rotation = m_Actor.Model.transform.rotation;
        }
    }
}