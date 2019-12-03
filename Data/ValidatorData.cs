using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Validators;

namespace DarkBestiary.Data
{
    [Serializable]
    public class ValidatorData : Identity<int>
    {
        public string Type;
    }

    [Serializable]
    public class EmptyValidatorData : ValidatorData
    {
    }

    [Serializable]
    public class ScenarioValidatorData : ValidatorData
    {
        public int ScenarioId;
    }

    [Serializable]
    public class StatusFlagsValidatorData : ValidatorData
    {
        public StatusFlags Flags;
    }

    [Serializable]
    public class UnitFlagsValidatorData : ValidatorData
    {
        public UnitFlags Flags;
    }

    [Serializable]
    public class UnitValidatorData : ValidatorData
    {
        public int UnitId;
    }

    [Serializable]
    public class ValueValidatorData : ValidatorData
    {
        public ComparatorMethod Comparator;
        public int Value;
    }

    [Serializable]
    public class UnitCountValidatorData : ValidatorData
    {
        public ComparatorMethod Comparator;
        public int Value;
        public int UnitId;
    }

    [Serializable]
    public class InRangeValidatorData : ValidatorData
    {
        public float Min;
        public float Max;
    }

    [Serializable]
    public class CombineValidatorsData : ValidatorData
    {
        public List<int> Validators = new List<int>();
    }

    [Serializable]
    public class BehaviourCountValidatorData : ValidatorData
    {
        public ComparatorMethod Comparator;
        public int Value;
        public int BehaviourId;
    }

    [Serializable]
    public class TargetHealthFractionValidatorData : ValidatorData
    {
        public ComparatorMethod Comparator;
        public float Fraction;
    }
}