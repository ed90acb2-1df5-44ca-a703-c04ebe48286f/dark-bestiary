using System;
using DarkBestiary.Behaviours;
using DarkBestiary.Data;
using DarkBestiary.Scenarios;
using DarkBestiary.UI.Elements;
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
                case VendorPanel.Category.Weapon:
                    return I18N.Instance.Get("item_category_weapon");
                case VendorPanel.Category.Armor:
                    return I18N.Instance.Get("item_category_armor");
                case VendorPanel.Category.Miscellaneous:
                    return I18N.Instance.Get("ui_miscellaneous");
                case VendorPanel.Category.Buyout:
                    return I18N.Instance.Get("ui_buyout");
                default:
                    return new I18NString(new I18NStringData(category.ToString()));
            }
        }

        public static I18NString Translate(ScenarioType scenarioType)
        {
            switch (scenarioType)
            {
                case ScenarioType.Campaign:
                    return I18N.Instance.Get("enum_scenario_type_campaign");
                case ScenarioType.Patrol:
                    return I18N.Instance.Get("enum_scenario_type_patrol");
                case ScenarioType.Nightmare:
                    return I18N.Instance.Get("enum_scenario_type_nightmare");
                case ScenarioType.Adventure:
                    return I18N.Instance.Get("enum_scenario_type_adventure");
                case ScenarioType.Arena:
                    return I18N.Instance.Get("enum_scenario_type_arena");
                default:
                    return new I18NString(new I18NStringData(scenarioType.ToString()));
            }
        }

        public static I18NString Translate(DamageType damageType)
        {
            switch (damageType)
            {
                case DamageType.Chaos:
                    return I18N.Instance.Get("enum_damage_type_true");
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
    }
}