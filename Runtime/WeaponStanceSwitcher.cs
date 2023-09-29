using System.Collections.Generic;
using DarkBestiary.Components;
using DarkBestiary.Items;
using UnityEngine;
using Component = DarkBestiary.Components.Component;

namespace DarkBestiary
{
    public class WeaponStanceSwitcher : MonoBehaviour
    {
        [SerializeField] private Animator m_Animator;

        private static readonly int s_DualWield = Animator.StringToHash("is_dual_wield");
        private static readonly int s_TwoHanded = Animator.StringToHash("is_two_handed");
        private static readonly int s_Rifle = Animator.StringToHash("is_rifle");
        private static readonly int s_Staff = Animator.StringToHash("is_staff");
        private static readonly int s_Spear = Animator.StringToHash("is_spear");
        private static readonly int s_Shield = Animator.StringToHash("is_shield");
        private static readonly int s_Bow = Animator.StringToHash("is_bow");

        private static readonly List<int> s_Flags = new()
        {
            s_DualWield,
            s_TwoHanded,
            s_Rifle,
            s_Staff,
            s_Spear,
            s_Shield,
            s_Bow
        };

        private EquipmentComponent m_Equipment;

        private void Start()
        {
            m_Equipment = GetComponentInParent<EquipmentComponent>();

            if (m_Equipment == null)
            {
                return;
            }

            m_Equipment.ItemEquipped += OnItemEquipped;
            m_Equipment.ItemUnequipped += OnItemUnequipped;
            m_Equipment.Terminated += OnTerminated;

            UpdateFlags();
        }

        private void OnEnable()
        {
            if (m_Equipment == null)
            {
                return;
            }

            UpdateFlags();
        }

        private void OnTerminated(Component component)
        {
            m_Equipment.ItemEquipped -= OnItemEquipped;
            m_Equipment.ItemUnequipped -= OnItemUnequipped;
            m_Equipment.Terminated -= OnTerminated;
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

            if (m_Equipment.IsDualWielding())
            {
                m_Animator.SetBool(s_DualWield, true);
                return;
            }

            if (m_Equipment.GetSecondaryOrPrimaryWeapon().IsShield)
            {
                m_Animator.SetBool(s_Shield, true);
                return;
            }

            var primaryWeapon = m_Equipment.GetPrimaryOrSecondaryWeapon();

            if (primaryWeapon.IsStaff)
            {
                m_Animator.SetBool(s_Staff, true);
                return;
            }

            if (primaryWeapon.IsRifle || primaryWeapon.IsCrossbow)
            {
                m_Animator.SetBool(s_Rifle, true);
                return;
            }

            if (primaryWeapon.IsTwoHandedSpear)
            {
                m_Animator.SetBool(s_Spear, true);
                return;
            }

            if (primaryWeapon.IsBow)
            {
                m_Animator.SetBool(s_Bow, true);
                return;
            }

            if (primaryWeapon.IsTwoHandedMeleeWeapon)
            {
                m_Animator.SetBool(s_TwoHanded, true);
            }
        }

        public void ResetFlags()
        {
            foreach (var flag in s_Flags)
            {
                m_Animator.SetBool(flag, false);
            }
        }
    }
}