using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Items;
using DarkBestiary.Modifiers;
using DarkBestiary.Skills;
using DarkBestiary.Validators;

namespace DarkBestiary.Data
{
    [Serializable]
    public class BehaviourData : Identity<int>
    {
        public string Type;
        public string Icon;
        public string Label;
        public string NameKey;
        public string DescriptionKey;
        public string Tint;
        public float Scale;
        public float Transparency;
        public int RarityId;
        public int Period;
        public int Duration;
        public int StackCountMax;
        public int OnApplyEffectId;
        public int OnExpireEffectId;
        public int OnRemoveEffectId;
        public bool CasterIsBearer;
        public bool IsAncient;
        public BehaviourFlags Flags;
        public ReApplyBehaviourFlags ReApplyFlags;
        public StatusFlags StatusFlags;
        public StatusFlags StatusImmunity;
        public BehaviourEventSubject EventSubject;
        public List<int> ItemCategories = new List<int>();
        public List<AttachmentInfo> Attachments = new List<AttachmentInfo>();
        public List<ValidatorWithPurposeData> Validators = new List<ValidatorWithPurposeData>();
    }

    [Serializable]
    public class EmptyBehaviourData : BehaviourData
    {
    }

    [Serializable]
    public class MulticastBehaviourData : BehaviourData
    {
        public float Chance;
    }

    [Serializable]
    public class ShieldBehaviourData : BehaviourData
    {
        public string MaxAmountFormula;
    }

    [Serializable]
    public class MaxRageBehaviourData : BehaviourData
    {
        public int BehaviourId;
    }

    [Serializable]
    public class CreateLineBehaviourData : BehaviourData
    {
        public string Prefab;
    }

    [Serializable]
    public class SpheresBehaviourData : BehaviourData
    {
        public string Prefab;
    }

    [Serializable]
    public class SpiritLinkBehaviourData : BehaviourData
    {
        public float Fraction;
    }

    [Serializable]
    public class UnlockSkillBehaviourData : BehaviourData
    {
        public int SkillId;
    }

    [Serializable]
    public class SetBehaviourData : BehaviourData
    {
        public List<int> Behaviours = new List<int>();
    }

    [Serializable]
    public class ItemBasedBehaviourData : BehaviourData
    {
        public int BehaviourId;
        public int ItemCategoryId;
    }

    [Serializable]
    public class CageBehaviourData : BehaviourData
    {
        public int Radius;
        public int LeaveRadiusEffectId;
    }

    [Serializable]
    public class ModifyStatsBehaviourData : BehaviourData
    {
        public List<AttributeModifierData> AttributeModifiers = new List<AttributeModifierData>();
        public List<PropertyModifierData> PropertyModifiers = new List<PropertyModifierData>();
    }

    [Serializable]
    public class BuffBehaviourData : BehaviourData
    {
        public int InitialEffectId;
        public int PeriodicEffectId;
    }

    [Serializable]
    public class ChangeModelBehaviourData : BehaviourData
    {
        public string Model;
    }

    [Serializable]
    public class CleaveBehaviourData : BehaviourData
    {
        public float Fraction;
        public string Prefab;
    }

    [Serializable]
    public class DualWieldBehaviourData : BehaviourData
    {
        public int BehaviourId;
    }

    [Serializable]
    public class EffectBehaviourData : BehaviourData
    {
        public int EffectId;
    }

    [Serializable]
    public class OnHealthDropsBelowBehaviourData : BehaviourData
    {
        public float Fraction;
        public int EffectId;
    }

    [Serializable]
    public class AuraBehaviourData : BehaviourData
    {
        public int Range;
        public int BehaviourId;
    }

    [Serializable]
    public class OnKillBehaviourData : BehaviourData
    {
        public DamageFlags DamageFlags;
        public DamageInfoFlags DamageInfoFlags;
        public int EffectId;
    }

    [Serializable]
    public class OnStatusEffectRemovedBehaviourData : BehaviourData
    {
        public StatusFlags StatusFlags;
        public int EffectId;
    }

    [Serializable]
    public class OnStatusEffectBehaviourData : BehaviourData
    {
        public StatusFlags StatusFlags;
        public int BehaviourId;
    }

    [Serializable]
    public class OnTakeDamageBehaviourData : BehaviourData
    {
        public DamageFlags DamageFlags;
        public DamageInfoFlags DamageInfoFlags;
        public DamageFlags ExcludeDamageFlags;
        public DamageInfoFlags ExcludeDamageInfoFlags;
        public List<DamageType> DamageTypes = new List<DamageType>();
        public int EffectId;
    }

    [Serializable]
    public class OnDealDamageBehaviourData : BehaviourData
    {
        public DamageFlags DamageFlags;
        public DamageInfoFlags DamageInfoFlags;
        public DamageFlags ExcludeDamageFlags;
        public DamageInfoFlags ExcludeDamageInfoFlags;
        public List<DamageType> DamageTypes = new List<DamageType>();
        public int EffectId;
    }

    [Serializable]
    public class OnUseSkillBehaviourData : BehaviourData
    {
        public SkillFlags SkillFlags;
        public int SkillRarityId;
        public int EffectId;
    }

    [Serializable]
    public class DamageBehaviourData : BehaviourData
    {
        public ModifierType ModifierType;
        public List<DamageType> DamageTypes = new List<DamageType>();
    }

    [Serializable]
    public class SummonedDamageBehaviourData : DamageBehaviourData
    {
        public float Amount;
    }

    [Serializable]
    public class BackstabDamageBehaviourData : DamageBehaviourData
    {
        public float Amount;
    }

    [Serializable]
    public class PerSurroundingEnemyDamageBehaviourData : DamageBehaviourData
    {
        public int Range;
        public int MinimumNumberOfEnemies;
        public float AmountPerEnemy;
        public float Min;
        public float Max;
    }

    [Serializable]
    public class HealthFractionDamageBehaviourData : DamageBehaviourData
    {
        public DamageSubject Target;
        public float RequiredHealthFraction;
        public ComparatorMethod Comparator;
        public float Amount;
    }

    [Serializable]
    public class PerMissingHealthPercentDamageBehaviourData : DamageBehaviourData
    {
        public DamageSubject Target;
        public float AmountPerPercent;
        public float Min;
        public float Max;
    }

    [Serializable]
    public class StatusEffectDamageBehaviourData : DamageBehaviourData
    {
        public StatusFlags DamageStatusFlags;
        public float Amount;
    }

    [Serializable]
    public class RangeDamageBehaviourData : DamageBehaviourData
    {
        public float Range;
        public ComparatorMethod Comparator;
        public float Amount;
    }

    [Serializable]
    public class PerRangeDamageBehaviourData : DamageBehaviourData
    {
        public DamageFlags DamageFlags;
        public DamageInfoFlags DamageInfoFlags;
        public float AmountPerCell;
        public float Min;
        public float Max;
    }
}