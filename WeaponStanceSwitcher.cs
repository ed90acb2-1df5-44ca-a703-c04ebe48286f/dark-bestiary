using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary
{
    public class WeaponStanceSwitcher : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private static readonly int DualWield = Animator.StringToHash("is_dual_wield");
        private static readonly int TwoHanded = Animator.StringToHash("is_two_handed");
        private static readonly int Rifle = Animator.StringToHash("is_rifle");
        private static readonly int Staff = Animator.StringToHash("is_staff");
        private static readonly int Spear = Animator.StringToHash("is_spear");
        private static readonly int Shield = Animator.StringToHash("is_shield");
        private static readonly int Bow = Animator.StringToHash("is_bow");

        private static readonly List<int> Flags = new List<int>
        {
            DualWield,
            TwoHanded,
            Rifle,
            Staff,
            Spear,
            Shield,
            Bow
        };

        private EquipmentComponent equipment;

        private void Start()
        {
            this.equipment = GetComponentInParent<EquipmentComponent>();

            if (this.equipment == null)
            {
                return;
            }

            this.equipment.ItemEquipped += OnItemEquipped;
            this.equipment.ItemUnequipped += OnItemUnequipped;
            this.equipment.Terminated += OnTerminated;

            UpdateFlags();
        }

        private void OnEnable()
        {
            if (this.equipment == null)
            {
                return;
            }

            UpdateFlags();
        }

        private void OnTerminated(Component component)
        {
            this.equipment.ItemEquipped -= OnItemEquipped;
            this.equipment.ItemUnequipped -= OnItemUnequipped;
            this.equipment.Terminated -= OnTerminated;
        }

        private void OnItemUnequipped(Item item)
        {
            UpdateFlags();
        }

        private void OnItemEquipped(Item item)
        {
            UpdateFlags();
        }

        private void UpdateFlags()
        {
            ResetFlags();

            if (this.equipment.IsDualWielding())
            {
                this.animator.SetBool(DualWield, true);
                return;
            }

            if (this.equipment.GetSecondaryOrPrimaryWeapon().IsShield)
            {
                this.animator.SetBool(Shield, true);
                return;
            }

            var primaryWeapon = this.equipment.GetPrimaryOrSecondaryWeapon();

            if (primaryWeapon.IsStaff)
            {
                this.animator.SetBool(Staff, true);
                return;
            }

            if (primaryWeapon.IsRifle || primaryWeapon.IsCrossbow)
            {
                this.animator.SetBool(Rifle, true);
                return;
            }

            if (primaryWeapon.IsTwoHandedSpear)
            {
                this.animator.SetBool(Spear, true);
                return;
            }

            if (primaryWeapon.IsBow)
            {
                this.animator.SetBool(Bow, true);
                return;
            }

            if (primaryWeapon.IsTwoHandedMeleeWeapon)
            {
                this.animator.SetBool(TwoHanded, true);
            }
        }

        public void ResetFlags()
        {
            foreach (var flag in Flags)
            {
                this.animator.SetBool(flag, false);
            }
        }
    }
}