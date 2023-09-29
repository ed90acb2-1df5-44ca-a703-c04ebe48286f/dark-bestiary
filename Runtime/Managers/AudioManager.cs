using DarkBestiary.Audio;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class AudioManager
    {
        public static AudioManager Instance => s_Instance ?? (s_Instance = Container.Instance.Instantiate<AudioManager>());
        private static AudioManager s_Instance;

        private readonly IAudioEngine m_Audio;

        public AudioManager(IAudioEngine audio)
        {
            m_Audio = audio;
        }

        public void PlayOneShot(string path)
        {
            m_Audio.PlayOneShot(path);
        }

        public void PlayDrinkPotion()
        {
            m_Audio.PlayOneShot("event:/SFX/Animations/Drink_Potion");
        }

        public void PlayVisionsEncounterDrop()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Vision_Encounter_Drop");
        }

        public void PlayVictory()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Victory");
        }

        public void PlayDefeat()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Defeat");
        }

        public void PlayAttributeIncrease()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Attribute_Increase");
        }

        public void PlayLevelUp()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Level_Up");
        }

        public void PlayCombatStart()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Combat_Start");
        }

        public void PlayCombatEnd()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Combat_End");
        }

        public void PlayItemBuy()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Buy");
        }

        public void PlayItemSell()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Sell");
        }

        public void PlayMouseEnter()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/interactable_Mouse_Enter");
        }

        public void PlayMouseClick()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/interactable_Mouse_Click");
        }

        public void PlayWindowOpen()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Window_Open");
        }

        public void PlayWindowClose()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Window_Close");
        }

        public void PlayItemPick()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Default_Pick");
        }

        public void PlayItemPlace()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Default_Place");
        }

        public void PlayItemPick(Item item)
        {
            PlayItemPickOrPlace(item, "Pick");
        }

        public void PlayItemPlace(Item item)
        {
            PlayItemPickOrPlace(item, "Place");
        }

        private void PlayItemPickOrPlace(Item item, string suffix)
        {
            var path = "event:/SFX/UI/Inventory_Item_Default_" + suffix;

            if (item.IsMeleeWeapon)
            {
                path = "event:/SFX/UI/Inventory_Item_Weapon_" + suffix;
            }
            else if (item.IsJewelry)
            {
                path = "event:/SFX/UI/Inventory_Item_Jewelry_" + suffix;
            }

            PlayOneShot(path);
        }

        public void PlaySkillUnlock()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Skill_Unlock");
        }

        public void PlayCraft()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Craft");
        }

        public void PlayCraftSocket()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Craft_Socket");
        }

        public void PlayCraftForge()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Craft_Forge");
        }

        public void PlayFootsteps(Vector3 position)
        {
            m_Audio.PlayOneShot("event:/SFX/Animations/Footsteps", position);
        }

        public void PlayCraftSharpenSuccess()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Craft_Forge");
        }

        public void PlayCraftSharpenFailed()
        {
            m_Audio.PlayOneShot("event:/SFX/Projectiles/Spore_Impact");
        }

        public void PlayDig(Vector3 position)
        {
            m_Audio.PlayOneShot("event:/SFX/Animations/Dig", position);
        }

        public void PlayDodge()
        {
            m_Audio.PlayOneShot("event:/SFX/Weapon/Miss");
        }

        public void PlayBlock()
        {
            m_Audio.PlayOneShot("event:/SFX/Weapon/Block");
        }

        public void PlayWhoosh()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Whoosh");
        }

        public void PlayEnchant()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Enchant");
        }

        public void PlayAlchemyCombine()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Alchemy_Combine");
        }

        public void PlayAlchemyBrew()
        {
            m_Audio.PlayOneShot("event:/SFX/UI/Alchemy_Brew");
        }
    }
}