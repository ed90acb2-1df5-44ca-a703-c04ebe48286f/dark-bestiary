using DarkBestiary.Data;
using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class UnitComponent : Component
    {
        public const int MaxLevel = 200;

        public static event Payload<UnitComponent> AnyUnitTerminated;

        public event Payload<UnitComponent> OwnerChanged;
        public event Payload<UnitComponent> TeamChanged;
        public event Payload<UnitComponent> LevelChanged;

        public int Id { get; set; }
        public I18NString Name { get; set; }
        public EnvironmentData Environment { get; private set; }
        public I18NString Description { get; private set; }
        public UnitFlags Flags { get; set; }
        public ArmorSound ArmorSound { get; private set; }
        public string Corpse { get; private set; }

        public bool IsPlayer => Owner == Owner.Player;
        public bool IsNeutral => Owner == Owner.Neutral;
        public bool IsHostile => Owner == Owner.Hostile;

        public bool IsDummy => Flags.HasFlag(UnitFlags.Dummy);
        public bool IsCorpseless => Flags.HasFlag(UnitFlags.Corpseless);
        public bool IsStructure => Flags.HasFlag(UnitFlags.Structure);
        public bool IsBoss => Flags.HasFlag(UnitFlags.Boss);
        public bool IsBlocksMovement => Flags.HasFlag(UnitFlags.BlocksMovement);
        public bool IsBlocksVision => Flags.HasFlag(UnitFlags.BlocksVision);
        public bool IsAirborne => Flags.HasFlag(UnitFlags.Airborne);
        public bool IsMovingViaScript => Flags.HasFlag(UnitFlags.MovingViaScript);
        public bool IsImmovable => Flags.HasFlag(UnitFlags.Immovable);

        public int Level
        {
            get => this.level;
            set
            {
                if (value == this.level)
                {
                    return;
                }

                this.level = Mathf.Clamp(value, 1, MaxLevel);
                LevelChanged?.Invoke(this);
            }
        }

        public int TeamId
        {
            get => this.teamId;
            set
            {
                if (value == this.teamId)
                {
                    return;
                }

                this.previousTeamId = TeamId;
                this.teamId = value;
                TeamChanged?.Invoke(this);
            }
        }

        public Owner Owner
        {
            get => this.owner;
            set
            {
                if (value == this.owner)
                {
                    return;
                }

                this.previousOwner = Owner;
                this.owner = value;
                OwnerChanged?.Invoke(this);
            }
        }

        public int ChallengeRating { get; private set; }

        private Owner owner = Owner.Neutral;
        private int level = 1;
        private int teamId;

        private Owner previousOwner = Owner.Neutral;
        private int previousTeamId;

        public UnitComponent Construct(UnitData data)
        {
            Id = data.Id;
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
            return level * 3;
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

        public void RestorePreviousOwner()
        {
            Owner = this.previousOwner;
            TeamId = this.previousTeamId;
        }

        protected override void OnTerminate()
        {
            AnyUnitTerminated?.Invoke(this);
        }
    }
}