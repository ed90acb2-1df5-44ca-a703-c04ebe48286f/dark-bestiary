using DarkBestiary.Audio;
using DarkBestiary.Items;
using UnityEngine;

namespace DarkBestiary.Managers
{
    public class AudioManager
    {
        public static AudioManager Instance => instance ?? (instance = Container.Instance.Instantiate<AudioManager>());
        private static AudioManager instance;

        private readonly IAudioEngine audio;

        public AudioManager(IAudioEngine audio)
        {
            this.audio = audio;
        }

        public void PlayOneShot(string path)
        {
            this.audio.PlayOneShot(path);
        }

        public void PlayVictory()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Victory");
        }

        public void PlayDefeat()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Defeat");
        }

        public void PlayAttributeIncrease()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Attribute_Increase");
        }

        public void PlayLevelUp()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Level_Up");
        }

        public void PlayCombatStart()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Combat_Start");
        }

        public void PlayCombatEnd()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Combat_End");
        }

        public void PlayItemBuy()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Buy");
        }

        public void PlayItemSell()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Sell");
        }

        public void PlayWindowOpen()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Window_Open");
        }

        public void PlayWindowClose()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Window_Close");
        }

        public void PlayItemPick()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Default_Pick");
        }

        public void PlayItemPlace()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Inventory_Item_Default_Place");
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
            this.audio.PlayOneShot("event:/SFX/UI/Skill_Unlock");
        }

        public void PlayCraft()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Craft");
        }

        public void PlayCraftSocket()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Craft_Socket");
        }

        public void PlayCraftForge()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Craft_Forge");
        }

        public void PlayFootsteps(Vector3 position)
        {
            this.audio.PlayOneShot("event:/SFX/Animations/Footsteps", position);
        }

        public void PlayDig(Vector3 position)
        {
            this.audio.PlayOneShot("event:/SFX/Animations/Dig", position);
        }

        public void PlayDodge()
        {
            this.audio.PlayOneShot("event:/SFX/Weapon/Miss");
        }

        public void PlayBlock()
        {
            this.audio.PlayOneShot("event:/SFX/Weapon/Block");
        }

        public void PlayWhoosh()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Whoosh");
        }

        public void PlayAlchemyCombine()
        {
            this.audio.PlayOneShot("event:/SFX/UI/Alchemy_Combine");
        }
    }
}