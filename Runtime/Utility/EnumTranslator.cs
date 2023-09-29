using System;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;
using DarkBestiary.UI.Elements;
using DarkBestiary.UI.Views.Unity;
using UnityEngine;

namespace DarkBestiary.Utility
{
    public static class EnumTranslator
    {
        public static I18NString Translate(FullScreenMode fullScreenMode)
        {
            switch (fullScreenMode)
            {
                case FullScreenMode.ExclusiveFullScreen:
                    return I18N.Instance.Get("enum_fullscreen_mode_exclusive_fullscreen");
                case FullScreenMode.FullScreenWindow:
                    return I18N.Instance.Get("enum_fullscreen_mode_windowed_fullscreen");
                case FullScreenMode.MaximizedWindow:
                    return I18N.Instance.Get("enum_fullscreen_mode_maximized");
                case FullScreenMode.Windowed:
                    return I18N.Instance.Get("enum_fullscreen_mode_windowed");
                default:
                    return new I18NString(new I18NStringData(fullScreenMode.ToString()));
            }
        }

        public static I18NString Translate(DamageMeterView.FilterType damageMeterFilterType)
        {
            switch (damageMeterFilterType)
            {
                case DamageMeterView.FilterType.DamageDoneAlly: return I18N.Instance.Get("enum_damage_meter_filter_damage_done_ally");
                case DamageMeterView.FilterType.HealingDoneAlly: return I18N.Instance.Get("enum_damage_meter_filter_healing_done_ally");
                case DamageMeterView.FilterType.DamageTakenAlly: return I18N.Instance.Get("enum_damage_meter_filter_damage_taken_ally");
                case DamageMeterView.FilterType.HealingTakenAlly: return I18N.Instance.Get("enum_damage_meter_filter_healing_taken_ally");

                case DamageMeterView.FilterType.DamageDoneEnemy: return I18N.Instance.Get("enum_damage_meter_filter_damage_done_enemy");
                case DamageMeterView.FilterType.HealingDoneEnemy: return I18N.Instance.Get("enum_damage_meter_filter_healing_done_enemy");
                case DamageMeterView.FilterType.DamageTakenEnemy: return I18N.Instance.Get("enum_damage_meter_filter_damage_taken_enemy");
                case DamageMeterView.FilterType.HealingTakenEnemy: return I18N.Instance.Get("enum_damage_meter_filter_healing_taken_enemy");
                default:
                    return new I18NString(new I18NStringData(damageMeterFilterType.ToString()));
            }
        }

        public static I18NString Translate(FoodType type)
        {
            switch (type)
            {
                case FoodType.Entree:
                    return I18N.Instance.Get("enum_food_type_entree");
                case FoodType.Dessert:
                    return I18N.Instance.Get("enum_food_type_dessert");
                case FoodType.Drink:
                    return I18N.Instance.Get("enum_food_type_drink");
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public static I18NString Translate(Shape shape)
        {
            switch (shape)
            {
                case Shape.Circle:
                    return I18N.Instance.Get("enum_shape_circle");
                case Shape.Cross:
                    return I18N.Instance.Get("enum_shape_cross");
                case Shape.Line:
                    return I18N.Instance.Get("enum_shape_line");
                case Shape.Cleave:
                    return I18N.Instance.Get("enum_shape_cleave");
                case Shape.Cone2:
                    return I18N.Instance.Get("enum_shape_cone");
                case Shape.Cone3:
                    return I18N.Instance.Get("enum_shape_cone");
                case Shape.Cone5:
                    return I18N.Instance.Get("enum_shape_cone");
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }

        public static I18NString Translate(VendorPanel.Category category)
        {
            switch (category)
            {
                case VendorPanel.Category.All:
                    return I18N.Instance.Get("item_category_all");
                case VendorPanel.Category.Weapon:
                    return I18N.Instance.Get("item_category_weapon");
                case VendorPanel.Category.Armor:
                    return I18N.Instance.Get("item_category_armor");
                case VendorPanel.Category.Miscellaneous:
                    return I18N.Instance.Get("ui_misc");
                case VendorPanel.Category.Buyout:
                    return I18N.Instance.Get("ui_buyout");
                default:
                    return new I18NString(new I18NStringData(category.ToString()));
            }
        }

        public static string Translate(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.Alpha0: return "0";
                case KeyCode.Alpha1: return "1";
                case KeyCode.Alpha2: return "2";
                case KeyCode.Alpha3: return "3";
                case KeyCode.Alpha4: return "4";
                case KeyCode.Alpha5: return "5";
                case KeyCode.Alpha6: return "6";
                case KeyCode.Alpha7: return "7";
                case KeyCode.Alpha8: return "8";
                case KeyCode.Alpha9: return "9";
                case KeyCode.Escape: return "Esc";
                case KeyCode.Plus: return "+";
                case KeyCode.Equals: return "=";
                case KeyCode.Minus: return "-";
            }

            return keyCode.ToString();
        }

        public static I18NString Translate(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Chaos:
                    return I18N.Instance.Get("enum_damage_type_chaos");
                case DamageType.Crushing:
                    return I18N.Instance.Get("enum_damage_type_crushing");
                case DamageType.Slashing:
                    return I18N.Instance.Get("enum_damage_type_slashing");
                case DamageType.Piercing:
                    return I18N.Instance.Get("enum_damage_type_piercing");
                case DamageType.Fire:
                    return I18N.Instance.Get("enum_damage_type_fire");
                case DamageType.Cold:
                    return I18N.Instance.Get("enum_damage_type_cold");
                case DamageType.Holy:
                    return I18N.Instance.Get("enum_damage_type_holy");
                case DamageType.Shadow:
                    return I18N.Instance.Get("enum_damage_type_shadow");
                case DamageType.Arcane:
                    return I18N.Instance.Get("enum_damage_type_arcane");
                case DamageType.Poison:
                    return I18N.Instance.Get("enum_damage_type_poison");
                case DamageType.Health:
                    return I18N.Instance.Get("enum_damage_type_health");
                case DamageType.Lightning:
                    return I18N.Instance.Get("enum_damage_type_lightning");
                default:
                    return new I18NString(new I18NStringData(damageType.ToString()));
            }
        }

        public static I18NString Translate(StatusFlags statusFlags)
        {
            switch (statusFlags)
            {
                case StatusFlags.None:
                    return I18N.Instance.Get(null);
                case StatusFlags.Slow:
                    return I18N.Instance.Get("enum_status_flag_slow");
                case StatusFlags.Stun:
                    return I18N.Instance.Get("enum_status_flag_stun");
                case StatusFlags.Swiftness:
                    return I18N.Instance.Get("enum_status_flag_swiftness");
                case StatusFlags.Disarm:
                    return I18N.Instance.Get("enum_status_flag_disarm");
                case StatusFlags.Silence:
                    return I18N.Instance.Get("enum_status_flag_silence");
                case StatusFlags.Confusion:
                    return I18N.Instance.Get("enum_status_flag_confusion");
                case StatusFlags.Weakness:
                    return I18N.Instance.Get("enum_status_flag_weakness");
                case StatusFlags.Invisibility:
                    return I18N.Instance.Get("enum_status_flag_invisibility");
                case StatusFlags.Immobilization:
                    return I18N.Instance.Get("enum_status_flag_immobilization");
                case StatusFlags.Invulnerability:
                    return I18N.Instance.Get("enum_status_flag_invulnerability");
                default:
                    return new I18NString(new I18NStringData(statusFlags.ToString()));
            }
        }

        public static string Translate(KeyType keyType)
        {
            switch (keyType)
            {
                case KeyType.Move:
                    return I18N.Instance.Get("ui_move");
                case KeyType.Stop:
                    return I18N.Instance.Get("ui_stop");
                case KeyType.Skill1:
                    return I18N.Instance.Get("ui_skill") + " 1";
                case KeyType.Skill2:
                    return I18N.Instance.Get("ui_skill") + " 2";
                case KeyType.Skill3:
                    return I18N.Instance.Get("ui_skill") + " 3";
                case KeyType.Skill4:
                    return I18N.Instance.Get("ui_skill") + " 4";
                case KeyType.Skill5:
                    return I18N.Instance.Get("ui_skill") + " 5";
                case KeyType.Skill6:
                    return I18N.Instance.Get("ui_skill") + " 6";
                case KeyType.Skill7:
                    return I18N.Instance.Get("ui_skill") + " 7";
                case KeyType.Skill8:
                    return I18N.Instance.Get("ui_skill") + " 8";
                case KeyType.Skill9:
                    return I18N.Instance.Get("ui_skill") + " 9";
                case KeyType.Skill10:
                    return I18N.Instance.Get("ui_skill") + " 10";
                case KeyType.ConsumablesBag:
                    return I18N.Instance.Get("ui_potion_bag");
                case KeyType.SwapWeapon:
                    return I18N.Instance.Get("ui_swap_weapon");
                case KeyType.EndTurn:
                    return I18N.Instance.Get("ui_end_turn");
                case KeyType.Equipment:
                    return I18N.Instance.Get("ui_equipment");
                case KeyType.Reliquary:
                    return I18N.Instance.Get("ui_reliquary");
                case KeyType.Masteries:
                    return I18N.Instance.Get("ui_masteries");
                case KeyType.Attributes:
                    return I18N.Instance.Get("ui_attributes");
                case KeyType.Talents:
                    return I18N.Instance.Get("ui_talents");
                case KeyType.Skills:
                    return I18N.Instance.Get("ui_skills");
                case KeyType.Achievements:
                    return I18N.Instance.Get("ui_achievements");
                case KeyType.CombatLog:
                    return I18N.Instance.Get("ui_combat_log");
                case KeyType.DamageMeter:
                    return I18N.Instance.Get("ui_damage_meter");
                case KeyType.Mailbox:
                    return I18N.Instance.Get("ui_mailbox");
                case KeyType.Menu:
                    return I18N.Instance.Get("ui_menu");
                case KeyType.IncreaseGameSpeed:
                    return I18N.Instance.Get("ui_increase_game_speed");
                case KeyType.DecreaseGameSpeed:
                    return I18N.Instance.Get("ui_decrease_game_speed");
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyType), keyType, null);
            }
        }
    }
}