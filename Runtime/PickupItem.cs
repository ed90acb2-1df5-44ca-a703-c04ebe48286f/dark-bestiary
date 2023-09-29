using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private int m_ItemId;


        [FMODUnity.EventRef]
        [SerializeField] private string m_Sound;
        [SerializeField] private GameObject m_Particles;

        private void OnMouseEnter()
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Interact);
        }

        private void OnMouseExit()
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
        }

        private void OnMouseUp()
        {
            CursorManager.Instance.ChangeState(CursorManager.CursorState.Normal);
            AudioManager.Instance.PlayOneShot(m_Sound);

            var item = Container.Instance.Resolve<IItemRepository>().Find(m_ItemId);

            if (item == null)
            {
                return;
            }

            Game.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(item);
            Instantiate(m_Particles, transform.position, Quaternion.identity).DestroyAsVisualEffect();
            Destroy(gameObject);
        }
    }
}