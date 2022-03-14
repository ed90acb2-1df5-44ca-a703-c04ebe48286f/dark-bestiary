using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Currencies;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Effects;
using DarkBestiary.Exceptions;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using DarkBestiary.Items;
using DarkBestiary.Managers;
using DarkBestiary.Messaging;
using DarkBestiary.Properties;
using DarkBestiary.Scenarios.Encounters;
using DarkBestiary.Skills.Targeting;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Skills
{
    public class Skill
    {
        public static event Payload<SkillUseEventData> AnySkillUsing;
        public static event Payload<SkillUseEventData> AnySkillUsed;

        public const int DefaultWeaponSkillId = 252;

        public static readonly Skill Empty = new Skill(-1, I18N.Instance.Get("Empty"));

        public event Payload<SkillUseEventData> Using;
        public event Payload<SkillUseEventData> Used;
        public event Payload<Skill> PriceUpdated;
        public event Payload<Skill> CooldownStarted;
        public event Payload<Skill> CooldownUpdated;
        public event Payload<Skill> CooldownFinished;

        public int Id { get; }
        public SkillData Data { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public I18NString Lore { get; }
        public SkillFlags Flags { get; }
        public SkillType Type { get; }
        public ISkillUseStrategy UseStrategy { get; }
        public string Animation { get; }
        public int RangeMin { get; }
        public Shape RangeShape { get; }
        public int AOE { get; }
        public Shape AOEShape { get; }
        public int RequiredLevel { get; }
        public Effect Effect { get; }

        public int DefaultCooldown { get; }
        public bool IsAttacking { get; set; }
        public string Icon { get; set; }
        public List<SkillSet> Sets { get; set; } = new List<SkillSet>();
        public EquipmentSlot EquipmentSlot { get; set; }
        public Item Weapon { get; set; }
        public ItemCategory RequiredItemCategory { get; set; }
        public Behaviour Behaviour { get; set; }
        public SkillCategory Category { get; set; }
        public Rarity Rarity { get; set; }

        public GameObject Caster
        {
            get => this.caster;
            set => this.caster = value;
        }

        public float PriceMultiplier
        {
            get => this.priceMultiplier;
            set
            {
                this.priceMultiplier = value;
                PriceUpdated?.Invoke(this);
            }
        }

        private readonly List<Currency> price;
        private readonly Dictionary<ResourceType, float> baseCost;
        private readonly int baseRangeMax;

        private float priceMultiplier = 1;
        private List<Skill> skills;
        private GameObject caster;

        public int Cooldown
        {
            get
            {
                var cooldown = IsOffhandWeapon() ? 1 : DefaultCooldown;

                if (DefaultCooldown <= 1 || Caster == null)
                {
                    return cooldown;
                }

                var cooldownReduction = (int) Caster.GetComponent<PropertiesComponent>().Get(PropertyType.CooldownReduction).Value();

                return Mathf.Max(cooldown - cooldownReduction, 1);
            }
        }

        public int RemainingCooldown
        {
            get => this.remainingCooldown;
            private set
            {
                this.remainingCooldown = value;
                CooldownUpdated?.Invoke(this);
            }
        }

        private int remainingCooldown;

        public Skill(SkillData data, Effect effect, List<Currency> price)
        {
            Data = data;
            Id = data.Id;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Lore = I18N.Instance.Get(data.LoreKey);
            Flags = data.Flags;
            Type = data.Type;
            UseStrategy = SkillTargetingFactory.Make(data.TargetType);
            Animation = data.Animation;
            RangeMin = data.RangeMin;
            RangeShape = data.RangeShape;
            AOE = data.AOE;
            AOEShape = data.AOEShape;
            DefaultCooldown = data.Cooldown;
            Icon = data.Icon;
            RequiredLevel = data.RequiredLevel;
            RemainingCooldown = 0;
            Effect = effect;

            this.price = price;
            this.baseCost = data.ResourcesCosts;
            this.baseRangeMax = data.RangeMax;
        }

        private Skill(int id, I18NString name)
        {
            Data = new SkillData();
            Id = id;
            Name = name;
        }

        public List<Currency> GetPrice()
        {
            var result = new List<Currency>();

            foreach (var currency in this.price)
            {
                result.Add(currency.Clone().Set((int) (currency.Amount * PriceMultiplier)));
            }

            return result;
        }

        public bool IsOnCooldown()
        {
            return RemainingCooldown > 0;
        }

        public bool IsEmpty()
        {
            return Id == -1;
        }

        public bool IsDefault()
        {
            return Id == DefaultWeaponSkillId;
        }

        public static bool IsTradable(SkillType type, SkillFlags flags)
        {
            return type == SkillType.Common &&
                   !flags.HasFlag(SkillFlags.Vision) &&
                   !flags.HasFlag(SkillFlags.Monster) &&
                   !flags.HasFlag(SkillFlags.Talent) &&
                   !flags.HasFlag(SkillFlags.Item);
        }

        public bool IsMatchingSearchTerm(string term)
        {
            return Name.LikeIgnoreCase($"%{term}%") ||
                   Description.LikeIgnoreCase($"%{term}%") ||
                   Sets.Any(s => s.Name.LikeIgnoreCase($"%{term}%"));
        }

        public bool IsMatchingSearchTerm(string term, SkillCategory category)
        {
            return IsMatchingSearchTerm(term) && (category.Id == -1 || Category == null || Category.Id == category.Id);
        }

        public List<Skill> GetSkills()
        {
            if (this.skills != null)
            {
                return this.skills;
            }

            this.skills = Container.Instance.Resolve<ISkillRepository>().Find(Data.Skills);

            foreach (var skill in this.skills)
            {
                skill.Caster = Caster;
            }

            return this.skills;
        }

        public bool IsTradable()
        {
            return IsTradable(Type, Flags);
        }

        public bool HaveEquipmentRequirements()
        {
            return RequiredItemCategory != null;
        }

        public bool EquipmentRequirementsMet()
        {
            if (!HaveEquipmentRequirements())
            {
                return true;
            }

            return Caster.GetComponent<EquipmentComponent>().Slots
                .Any(slot => RequiredItemCategory.Contains(slot.Item.Type));
        }

        public Missile GetMissile()
        {
            if (Effect is LaunchMissileEffect launchMissileEffect)
            {
                return launchMissileEffect.GetMissile();
            }

            if (Effect is ChainEffect chainEffect)
            {
                return (chainEffect.GetEffect() as LaunchMissileEffect)?.GetMissile();
            }

            if (Effect is EffectSet effectSet)
            {
                return (effectSet.GetEffects().FirstOrDefault() as LaunchMissileEffect)?.GetMissile();
            }

            return null;
        }

        public string GetRangeString()
        {
            string range;

            if (GetMaxRange() < 2)
            {
                range = I18N.Instance.Get("ui_melee");
            }
            else
            {
                range = RangeMin == 0 ? GetMaxRange().ToString() : $"{RangeMin}-{GetMaxRange()}";
            }

            return range;
        }

        public int GetMaxRange()
        {
            if (Caster == null || Flags.HasFlag(SkillFlags.FixedRange) || UseStrategy is NoneSkillUseStrategy)
            {
                return this.baseRangeMax;
            }

            var properties = Caster.GetComponent<PropertiesComponent>();
            var extraRange = 0;

            if (Flags.HasFlag(SkillFlags.RangedWeapon))
            {
                extraRange = (int) properties.Get(PropertyType.RangedWeaponExtraRange).Value();
            }
            else if (Flags.HasFlag(SkillFlags.Magic))
            {
                extraRange = (int) properties.Get(PropertyType.MagicExtraRange).Value();
            }

            return this.baseRangeMax + extraRange;
        }

        public float GetCost(ResourceType resourceType)
        {
            return GetCost().GetValueOrDefault(resourceType, 0);
        }

        public float GetBaseCost(ResourceType resourceType)
        {
            return this.baseCost.GetValueOrDefault(resourceType, 0);
        }

        public Dictionary<ResourceType, float> GetCost()
        {
            if (Caster == null)
            {
                return new Dictionary<ResourceType, float>();
            }

            var behaviours = Caster.GetComponent<BehavioursComponent>();

            var cost = this.baseCost
                .Where(pair => pair.Key == ResourceType.ActionPoint)
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            if (behaviours.IsWeakened)
            {
                foreach (var pair in cost.Clone())
                {
                    cost[pair.Key] *= 2;
                }
            }

            if (behaviours.IsAdrenaline)
            {
                foreach (var pair in cost.Clone())
                {
                    cost[pair.Key] = Mathf.Max(1, cost[pair.Key] / 2);
                }
            }

            if (IsOffhandWeapon())
            {
                foreach (var pair in cost.Clone())
                {
                    cost[pair.Key] = Mathf.Max(1, cost[pair.Key] / 2);
                }
            }

            if (behaviours.IsFreecasting)
            {
                foreach (var pair in cost.Clone())
                {
                    cost[pair.Key] = 0;
                }
            }

            cost.Add(ResourceType.Rage, this.baseCost.GetValueOrDefault(ResourceType.Rage, 0));

            return cost;
        }

        public bool IsOffhandWeapon()
        {
            if (EquipmentSlot == null)
            {
                return false;
            }

            return EquipmentSlot.Type == EquipmentSlotType.OffHand && HasAnyWeaponFlag();
        }

        private bool HasAnyWeaponFlag()
        {
            return Flags.HasFlag(SkillFlags.MeleeWeapon) || Flags.HasFlag(SkillFlags.RangedWeapon) ||
                   Flags.HasFlag(SkillFlags.MagicWeapon);
        }

        public bool IsDisabled()
        {
            if (IsEmpty())
            {
                return false;
            }

            var behaviours = Caster.GetComponent<BehavioursComponent>();

            if (behaviours.IsUncontrollable)
            {
                return true;
            }

            return Type == SkillType.Weapon && behaviours.IsDisarmed ||
                   Type == SkillType.Common && behaviours.IsSilenced ||
                   Flags.HasFlag(SkillFlags.Movement) && behaviours.IsImmobilized;
        }

        public bool HasEnoughResources()
        {
            if (IsEmpty())
            {
                return true;
            }

            return GetInsufficientResource() == null;
        }

        public Resource GetInsufficientResource()
        {
            var resources = Caster.GetComponent<ResourcesComponent>();

            foreach (var cost in GetCost())
            {
                if (!resources.HasEnough(cost.Key, cost.Value))
                {
                    return resources.Get(cost.Key);
                }
            }

            return null;
        }

        public void ReduceCooldown(int amount)
        {
            if (Cooldown == 0)
            {
                return;
            }

            if (!IsOnCooldown())
            {
                return;
            }

            RemainingCooldown -= amount;

            if (RemainingCooldown <= 0)
            {
                ResetCooldown();
            }
        }

        public void ResetCooldown()
        {
            RemainingCooldown = 0;
            CooldownFinished?.Invoke(this);
        }

        public void Use(object target)
        {
            if (Flags.HasFlag(SkillFlags.Passive) ||
                Flags.HasFlag(SkillFlags.EndTurn) && !Encounter.IsCombat)
            {
                return;
            }

            Validate(target);

            if (Flags.HasFlag(SkillFlags.EndTurn))
            {
                CombatEncounter.Active?.EndTurn(Caster);
            }

            RunCooldown();
            ConsumeResources();

            UseInternal(target);
        }

        public void UseInternal(object target)
        {
            var payload = new SkillUseEventData(Caster, target, this);

            Using?.Invoke(payload);
            AnySkillUsing?.Invoke(payload);

            if (Flags.HasFlag(SkillFlags.Delayed))
            {
                SkillQueue.Enqueue(this, target);

                Used?.Invoke(payload);
                AnySkillUsed?.Invoke(payload);
            }
            else
            {
                FaceTargetAndPlayAnimation(target, () =>
                {
                    var effect = Effect.Clone();
                    effect.Skill = this;
                    effect.Finished += OnEffectFinished;
                    effect.Apply(Caster, target);
                });
            }
        }

        public void FaceTargetAndPlayAnimation(object target, Action callback)
        {
            var actor = Caster.GetComponent<ActorComponent>();
            actor.Model.LookAt(target.GetPosition());

            var animation = DetermineAnimation(Animation);

            actor.PlayAnimation(animation);

            Timer.Instance.Wait(actor.Model.GetAnimationLength(animation) * 0.33f, callback.Invoke);
        }

        public bool IsTargetInRange(GameObject target)
        {
            return BoardNavigator.Instance
                .WithinSkillRange(Caster.transform.position, target.transform.position, this)
                .Any(c => c.OccupiedBy == target);
        }

        public void Validate(object target)
        {
            if (!(UseStrategy is NoneSkillUseStrategy) && !BoardNavigator.Instance.IsWithinSkillRange(Caster, target.GetPosition(), this))
            {
                throw new TargetIsTooFarException();
            }

            if (!UseStrategy.IsValidTarget(this, target))
            {
                throw new InvalidSkillTargetException();
            }

            if (IsOnCooldown())
            {
                throw new SkillIsOnCooldownException();
            }

            if (!EquipmentRequirementsMet() || !DualWieldRequirementMet())
            {
                throw new MissingRequiredEquipmentException();
            }

            var insufficientResource = GetInsufficientResource();

            if (insufficientResource != null)
            {
                throw new InsufficientResourceException(insufficientResource);
            }

            if (IsDisabled())
            {
                throw new SkillIsDisabledException();
            }
        }

        public bool RequiresDualWielding()
        {
            return Flags.HasFlag(SkillFlags.DualWield);
        }

        public bool DualWieldRequirementMet()
        {
            return !RequiresDualWielding() || Caster.GetComponent<EquipmentComponent>().IsDualWielding();
        }

        public string DetermineAnimation(string animation)
        {
            return DetermineAnimation(
                animation,
                Caster.GetComponent<EquipmentComponent>(),
                Caster.GetComponent<SpellbookComponent>());
        }

        private string DetermineAnimation(string animation, EquipmentComponent equipment, SpellbookComponent spellbook)
        {
            if (!animation.StartsWith("attack") || equipment == null)
            {
                return animation;
            }

            if (IsDefault())
            {
                return spellbook.Slots[0].Skill == this ? "attack_fist_main" : "attack_fist_off";
            }

            var weapon = EquipmentSlot?.Item;

            if (weapon == null || weapon.IsEmpty)
            {
                weapon = GetMatchingWeapon(equipment);

                if (weapon == null)
                {
                    return animation;
                }
            }

            if (animation == "attack")
            {
                animation = GetAnimationByItemType(weapon);
            }

            if (weapon.IsTwoHandedWeapon)
            {
                return animation;
            }

            return weapon.EquipmentSlot?.Type == EquipmentSlotType.OffHand
                ? $"{animation}_off"
                : $"{animation}_main";
        }

        private Item GetMatchingWeapon(EquipmentComponent equipment)
        {
            var mh = equipment.Slots.First(s => s.Type == EquipmentSlotType.MainHand);
            var oh = equipment.Slots.First(s => s.Type == EquipmentSlotType.OffHand);

            var isMh = !mh.IsEmpty && RequiredItemCategory?.Contains(mh.Item.Type) == true;
            var isOh = !oh.IsEmpty && RequiredItemCategory?.Contains(oh.Item.Type) == true;

            if (!isMh && !isOh)
            {
                return null;
            }

            return isMh ? mh.Item : oh.Item;
        }

        private static string GetAnimationByItemType(Item weapon)
        {
            string animation;

            switch (weapon.Type.Type)
            {
                case ItemTypeType.MagicStaff:
                case ItemTypeType.Scythe:
                case ItemTypeType.CombatStaff:
                case ItemTypeType.TwoHandedAxe:
                case ItemTypeType.TwoHandedSword:
                case ItemTypeType.TwoHandedMace:
                    animation = "attack_great_sword";
                    break;
                case ItemTypeType.Claws:
                case ItemTypeType.Chakram:
                    animation = "attack_claws";
                    break;
                case ItemTypeType.Katar:
                    animation = "attack_katar";
                    break;
                case ItemTypeType.Dagger:
                case ItemTypeType.OneHandedSpear:
                    animation = "attack_dagger";
                    break;
                case ItemTypeType.TwoHandedSpear:
                    animation = "attack_spear";
                    break;
                case ItemTypeType.Knuckles:
                    animation = "attack_fist";
                    break;
                case ItemTypeType.Wand:
                case ItemTypeType.OneHandedAxe:
                case ItemTypeType.OneHandedMace:
                case ItemTypeType.OneHandedSword:
                case ItemTypeType.Sickle:
                    animation = "attack_sword";
                    break;
                case ItemTypeType.Bow:
                    animation = "attack_bow";
                    break;
                case ItemTypeType.Pistol:
                    animation = "attack_pistol";
                    break;
                case ItemTypeType.Crossbow:
                    animation = "attack_crossbow";
                    break;
                case ItemTypeType.Rifle:
                    animation = "attack_rifle";
                    break;
                default:
                    throw new Exception($"Cannot determine animation for item type: {weapon.Type.Type}");
            }

            return animation;
        }

        private void ConsumeResources()
        {
            Caster.GetComponent<ResourcesComponent>().Consume(GetCost());
        }

        public void RunCooldown(int cooldown = 0)
        {
            if (Cooldown == 0)
            {
                return;
            }

            RemainingCooldown = cooldown == 0 ? Cooldown : cooldown;
            CooldownStarted?.Invoke(this);
        }

        private void OnEffectFinished(Effect effect)
        {
            effect.Finished -= OnEffectFinished;

            var data = new SkillUseEventData(effect.Caster, effect.OriginalTarget, this);

            Used?.Invoke(data);
            AnySkillUsed?.Invoke(data);
        }

        public DamageEffect GetFirstDamageEffect()
        {
            return GetFirstDamageEffect(Effect);
        }

        public DamageEffect GetFirstDamageEffect(Effect parent)
        {
            switch (parent)
            {
                case DamageEffect damage:
                    return damage;
                case LaunchMissileEffect launchMissile:
                    return GetFirstDamageEffect(launchMissile.GetFinalEffect());
                default:
                    return null;
            }
        }

        public string GetDamageValueString(string @default = "")
        {
            var displayFormulas = SettingsManager.Instance.DisplayFormulasInTooltips;

            SettingsManager.Instance.SetDisplayFormulasInTooltips(false);

            var damageValueString = Description.Variables.FirstOrDefault(
                                            v => v.Data.EntityType == "Effect" &&
                                                 v.Data.PropertyName == "GetAmountString")?
                                        .GetValue(new StringVariableContext(Caster, this))
                                        .ToString() ?? @default;

            SettingsManager.Instance.SetDisplayFormulasInTooltips(displayFormulas);

            return damageValueString;
        }
    }
}