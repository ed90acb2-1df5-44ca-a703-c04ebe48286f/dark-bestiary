using System;
using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class UnitComponent : Component
    {
        public static event Action<UnitComponent> AnyUnitInitialized;
        public static event Action<UnitComponent> AnyUnitTerminated;
        public static event Action<UnitComponent> AnyUnitOwnerChanged;

        public event Action<UnitComponent> OwnerChanged;
        public event Action<UnitComponent> TeamChanged;
        public event Action<UnitComponent> LevelChanged;

        public int Id { get; set; }
        public string Label { get; set; }
        public I18NString Name { get; set; }
        public EnvironmentData Environment { get; private set; }
        public I18NString Description { get; private set; }
        public UnitFlags Flags { get; set; }
        public ArmorSound ArmorSound { get; private set; }
        public string Corpse { get; private set; }

        public bool IsPlayer => Owner == Owner.Player;
        public bool IsNeutral => Owner == Owner.Neutral;
        public bool IsHostile => Owner == Owner.Hostile;

        public bool IsPlayable => Flags.HasFlag(UnitFlags.Playable);
        public bool IsDummy => Flags.HasFlag(UnitFlags.Dummy);
        public bool IsCorpseless => Flags.HasFlag(UnitFlags.Corpseless);
        public bool IsStructure => Flags.HasFlag(UnitFlags.Structure);
        public bool IsPlant => Flags.HasFlag(UnitFlags.Plant);
        public bool IsWooden => Flags.HasFlag(UnitFlags.Wooden);
        public bool IsStone => Flags.HasFlag(UnitFlags.Stone);
        public bool IsBoss => Flags.HasFlag(UnitFlags.Boss);
        public bool IsUndead => Flags.HasFlag(UnitFlags.Undead);
        public bool IsBlocksMovement => Flags.HasFlag(UnitFlags.BlocksMovement);
        public bool IsBlocksVision => Flags.HasFlag(UnitFlags.BlocksVision);
        public bool IsAirborne => Flags.HasFlag(UnitFlags.Airborne);
        public bool IsMovingViaScript => Flags.HasFlag(UnitFlags.MovingViaScript);
        public bool IsImmovable => Flags.HasFlag(UnitFlags.Immovable);

        public int Level
        {
            get => m_Level;
            set
            {
                if (value == m_Level)
                {
                    return;
                }

                m_Level = Mathf.Clamp(value, 1, int.MaxValue);
                LevelChanged?.Invoke(this);
            }
        }

        public int TeamId
        {
            get => m_TeamId;
            set
            {
                if (value == m_TeamId)
                {
                    return;
                }

                m_PreviousTeamId = TeamId;
                m_TeamId = value;
                TeamChanged?.Invoke(this);
            }
        }

        public Owner Owner
        {
            get => m_Owner;
            set
            {
                if (value == m_Owner)
                {
                    return;
                }

                m_PreviousOwner = Owner;
                m_Owner = value;

                OwnerChanged?.Invoke(this);
                AnyUnitOwnerChanged?.Invoke(this);
            }
        }

        public int ChallengeRating { get; private set; }

        private Owner m_Owner = Owner.Neutral;
        private int m_Level = 1;
        private int m_TeamId;

        private Owner m_PreviousOwner = Owner.Neutral;
        private int m_PreviousTeamId;

        public UnitComponent Construct(UnitData data)
        {
            Id = data.Id;
            Label = data.Label;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Flags = data.Flags;
            Corpse = data.Corpse;
            ChallengeRating = data.ChallengeRating;
            ArmorSound = data.ArmorSound;

            gameObject.name = $"{Name} #{GetInstanceID()}";

            return this;
        }

        public static int CalculateKillExperience(int level, int challengeRating)
        {
            // +2 to level up after Predatory Flora scenario
            return (int) (level * (3 + challengeRating * 0.5f) * CalculateKillExperienceMultiplier(level)) + 2;
        }

        private static float CalculateKillExperienceMultiplier(int level)
        {
            return 1;

            var characterLevel = level;

            if (Game.Instance.Character != null)
            {
                characterLevel = Game.Instance.Character.Entity.GetComponent<ExperienceComponent>().Experience.Level;
            }

            if (characterLevel - level >= 10)
            {
                return 0;
            }

            if (characterLevel - level >= 8)
            {
                return 0.25f;
            }

            if (characterLevel - level >= 6)
            {
                return 0.5f;
            }

            if (characterLevel - level >= 4)
            {
                return 0.75f;
            }

            return 1;
        }

        public string GetNameOrLabel()
        {
            return string.IsNullOrEmpty(Name) ? Label : Name;
        }

        public int GetKillExperience()
        {
            return CalculateKillExperience(Level, ChallengeRating);
        }

        public void ChangeOwner(UnitComponent owner)
        {
            Owner = owner.Owner;
            TeamId = owner.TeamId;
        }

        public void ChangeOwner(Owner owner, int teamId)
        {
            Owner = owner;
            TeamId = teamId;
        }

        public void RestorePreviousOwner()
        {
            Owner = m_PreviousOwner;
            TeamId = m_PreviousTeamId;
        }

        protected override void OnInitialize()
        {
            AnyUnitInitialized?.Invoke(this);
        }

        protected override void OnTerminate()
        {
            AnyUnitTerminated?.Invoke(this);
        }
    }
}