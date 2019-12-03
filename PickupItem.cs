using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Extensions;
using DarkBestiary.Managers;
using UnityEngine;

namespace DarkBestiary
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private int itemId;

        [FMODUnity.EventRef]
        [SerializeField] private string sound;
        [SerializeField] private GameObject particles;

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
            AudioManager.Instance.PlayOneShot(this.sound);

            var item = Container.Instance.Resolve<IItemRepository>().Find(this.itemId);

            if (item == null)
            {
                return;
            }

            CharacterManager.Instance.Character.Entity.GetComponent<InventoryComponent>().Pickup(item);
            Instantiate(this.particles, transform.position, Quaternion.identity).DestroyAsVisualEffect();
            Destroy(gameObject);
        }
    }
}