using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Currencies;
using DarkBestiary.Effects;
using DarkBestiary.Items;

namespace DarkBestiary.Data
{
    [Serializable]
    public class EffectData : Identity<int>
    {
        public string Name;
        public string Type;
        public string Animation;
        public string Sound;
        public float Chance;
        public int StackCountMax;
        public bool StackChance;
        public bool CasterIsOwner;
        public EffectTargetType Target;
        public List<ValidatorWithPurposeData> Validators = new();
        public List<AttachmentInfo> Attachments = new();
    }

    [Serializable]
    public class EmptyEffectData : EffectData
    {
    }

    [Serializable]
    public class ChangeOwnerEffectData : EffectData
    {
        public bool IsNeutral;
    }

    [Serializable]
    public class CreateCorpseEffectData : EffectData
    {
        public string Prefab;
    }

    [Serializable]
    public class AttackEffectData : EffectData
    {
        public float DamageMultiplier;
        public bool TriggerCooldown;
        public bool IsOffHand;
    }

    [Serializable]
    public class GiveItemEffectData : EffectData
    {
        public int ItemId;
        public int Count;
    }

    [Serializable]
    public class HookEffectData : EffectData
    {
        public string Hook;
        public string Beam;
    }

    [Serializable]
    public class SearchDummiesEffectData : EffectData
    {
        public int EffectId;
        public int Range;
        public int Limit;
    }

    [Serializable]
    public class IfElseEffectData : EffectData
    {
        public int IfEffectId;
        public int ElseEffectId;
    }

    [Serializable]
    public class RepeatEffectData : EffectData
    {
        public int Times;
        public int EffectId;
    }

    [Serializable]
    public class MirrorImageEffectData : EffectData
    {
        public int BehaviourId;
        public int Duration;
        public int Count;
        public string Prefab;
    }

    [Serializable]
    public class HealFromTargetHealthEffectData : EffectData
    {
        public float Fraction;
    }

    [Serializable]
    public class ReduceCooldownsEffectData : EffectData
    {
        public int Amount;
    }

    [Serializable]
    public class RunCooldownEffectData : EffectData
    {
        public int SkillId;
    }

    [Serializable]
    public class SuckInEffectData : EffectData
    {
        public int Radius;
        public float Duration;
        public string Animation;
    }

    [Serializable]
    public class ChainEffectData : EffectData
    {
        public int Times;
        public float Period;
        public int Radius;
        public int EffectId;
        public int FinalEffectId;
    }

    [Serializable]
    public class SearchPerimeterEffectData : EffectData
    {
        public Side Sides = Side.None;
        public bool PickRandomSide;
        public int Limit;
        public int EffectId;
    }

    [Serializable]
    public class SearchRandomPointsData : EffectData
    {
        public int RangeMin;
        public int RangeMax;
        public int Limit;
        public int EffectId;
        public bool IncludeOccupied;
    }

    [Serializable]
    public class SetHealthFractionEffectData : EffectData
    {
        public float Fraction;
    }

    [Serializable]
    public class FormulaBasedEffectData : EffectData
    {
        public string Formula;
    }

    [Serializable]
    public class DamageEffectData : FormulaBasedEffectData
    {
        public int Base;
        public float Variance;
        public float Vampirism;
        public int OnCritEffectId;
        public WeaponSound WeaponSound;
        public DamageType DamageType;
        public DamageFlags DamageFlags;
        public DamageInfoFlags DamageInfoFlags;
        public List<int> Effects = new();
    }

    [Serializable]
    public class PerBehaviourStackDamageEffectData : DamageEffectData
    {
        public int BehaviourId;
        public StatusFlags StatusFlags;
        public DamageSubject Subject;
    }

    [Serializable]
    public class RemoveBehaviourEffectData : EffectData
    {
        public int BehaviourId;
    }

    [Serializable]
    public class RemoveBehaviourStackEffectData : EffectData
    {
        public int StackCount;
        public int BehaviourId;
    }

    [Serializable]
    public class HealEffectData : FormulaBasedEffectData
    {
        public int Base;
        public HealFlags Flags;
    }

    [Serializable]
    public class ShieldEffectData : FormulaBasedEffectData
    {
        public int Base;
        public int BehaviourId;
    }

    [Serializable]
    public class RewardEffectData : EffectData
    {
        public int RewardId;
    }

    [Serializable]
    public class DispelEffectData : EffectData
    {
        public BehaviourFlags BehaviourFlags;
        public StatusFlags BehaviourStatusFlags;
    }

    [Serializable]
    public class AddCurrencyEffectData : EffectData
    {
        public CurrencyType CurrencyType;
        public string CurrencyFormula;
    }

    [Serializable]
    public class DragEffectData : EffectData
    {
        public string StartAnimation;
        public string EndAnimation;
        public MoverData Mover;
        public int CollideWithEnvironmentEffectId;
        public int CollideWithEntityEffectId;
        public int FinalEffectId;
        public bool StopOnCollision;
        public bool IsAirborne;
    }

    [Serializable]
    public class RestoreResourceEffectData : EffectData
    {
        public ResourceType ResourceType;
        public float ResourceAmount;
    }

    [Serializable]
    public class RunAwayEffectData : EffectData
    {
        public string Animation;
        public float Speed;
        public int Distance;
        public bool RandomDirection;
        public bool ToCaster;
    }

    [Serializable]
    public class MoveEffectData : EffectData
    {
        public string Animation;
        public float Speed;
        public int Distance;
    }

    [Serializable]
    public class KnockbackEffectData : EffectData
    {
        public int Distance;
        public MoverData Mover;
        public int CollideWithEnvironmentEffectId;
        public int CollideWithEntityEffectId;
        public int EnterCellEffectId;
        public int ExitCellEffectId;
        public int FinalEffectId;
    }

    [Serializable]
    public class LaunchMissileEffectData : EffectData
    {
        public int MissileId;
        public AttachmentPoint CasterAttachmentPoint;
        public AttachmentPoint TargetAttachmentPoint;
        public MoverData Mover;
        public int CollideWithEnvironmentEffectId;
        public int CollideWithEntitiesEffectId;
        public int EnterCellEffectId;
        public int ExitCellEffectId;
        public int FinalEffectId;
        public bool FinishImmediately;
        public bool IsPiercing;
        public float CasterDirectionalOffsetX;
        public float CasterDirectionalOffsetY;
        public float TargetDirectionalOffsetX;
        public float TargetDirectionalOffsetY;
        public float CasterOffsetX;
        public float CasterOffsetY;
        public float TargetOffsetX;
        public float TargetOffsetY;
        public float MissileFlyHeight;
    }

    [Serializable]
    public class LaunchAoeMissileEffectData : EffectData
    {
        public int MissileId;
        public AttachmentPoint CasterAttachmentPoint;
        public AttachmentPoint TargetAttachmentPoint;
        public MoverData Mover;
        public int CollideWithEnvironmentEffectId;
        public int CollideWithEntitiesEffectId;
        public int EnterCellEffectId;
        public int ExitCellEffectId;
        public int FinalEffectId;
        public int Radius;
        public int EffectId;
        public float DirectionalOffsetX;
        public float DirectionalOffsetY;
        public float CasterOffsetX;
        public float CasterOffsetY;
        public float TargetOffsetX;
        public float TargetOffsetY;
        public float MissileFlyHeight;
    }

    [Serializable]
    public class SearchAreaEffectData : EffectData
    {
        public Shape Shape;
        public int Radius;
        public int Limit;
        public int EffectId;
        public bool ExcludeOrigin;
        public bool ExcludeTarget;
        public bool CheckLineOfSight;
        public bool Unoccupied;
    }

    [Serializable]
    public class SearchLineEffectData : EffectData
    {
        public int Length;
        public int EffectId;
        public bool ExcludeOrigin;
        public bool CheckLineOfSight;
    }

    [Serializable]
    public class SearchBehindEffectData : EffectData
    {
        public int EffectId;
    }

    [Serializable]
    public class ApplyBehaviourEffectData : EffectData
    {
        public int BehaviourId;
        public int Stacks;
    }

    [Serializable]
    public class DestroyEquippedItemEffectData : EffectData
    {
        public int ItemId;
    }

    [Serializable]
    public class CreateBeamEffectData : EffectData
    {
        public string Path;
    }

    [Serializable]
    public class CreateSoundEffectData : EffectData
    {
        public string Path;
    }

    [Serializable]
    public class CreateUnitEffectData : EffectData
    {
        public int UnitId;
        public int Duration;
        public float HealthFraction;
        public Owner Owner;
        public bool KillOnCasterDeath;
        public bool KillOnEpisodeComplete;
    }

    [Serializable]
    public class WaitEffectData : EffectData
    {
        public float Seconds;
    }

    [Serializable]
    public class RandomWaitEffectData : EffectData
    {
        public float Min;
        public float Max;
    }

    [Serializable]
    public class EffectSetData : EffectData
    {
        public List<int> Effects = new();
    }

    [Serializable]
    public class RandomEffectData : EffectData
    {
        public List<int> Effects = new();
    }
}