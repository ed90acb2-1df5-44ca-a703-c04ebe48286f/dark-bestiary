using System;
using System.Collections.Generic;
using DarkBestiary.Attributes;
using DarkBestiary.Components;
using DarkBestiary.Properties;
using DarkBestiary.Skills;
using UnityEngine;
using Utils;

namespace DarkBestiary
{
    public static class Formula
    {
        private static readonly Dictionary<string, Expression> Expressions = new Dictionary<string, Expression>();

        public static string ToUserFormat(string formula)
        {
            return formula;
        }

        public static float Evaluate(string formula, GameObject caster, GameObject target, Skill skill = null)
        {
            var expression = ParseAndCacheExpression(formula);

            foreach (var parameter in Parameters)
            {
                if (!expression.Parameters.ContainsKey(parameter.Key))
                {
                    continue;
                }

                expression.Parameters[parameter.Key].Value = parameter.Value(caster, target, skill);
            }

            return (float) expression.Value;
        }

        private static Expression ParseAndCacheExpression(string formula)
        {
            if (Expressions.ContainsKey(formula))
            {
                return Expressions[formula];
            }

            try
            {
                Expressions.Add(formula, new ExpressionParser().EvaluateExpression(formula));
            }
            catch (ExpressionParser.ParseException exception)
            {
                Debug.LogError($"Error parsing formula: \"{formula}\" {exception.Message}");
            }

            return Expressions[formula];
        }

        private static readonly Dictionary<string, Func<GameObject, GameObject, Skill, float>> Parameters =
            new Dictionary<string, Func<GameObject, GameObject, Skill, float>>
            {
                {"AP_SCALE_2", (caster, target, skill) => 0.5f},
                {"AP_SCALE_4", (caster, target, skill) => 1.25f},
                {"AP_SCALE_6", (caster, target, skill) => 2.5f},
                {
                    "TARGET_HP_MAX",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<HealthComponent>().HealthMax
                        : target.GetComponent<HealthComponent>().HealthMax
                },
                {
                    "TARGET_HP_CURRENT",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<HealthComponent>().Health
                        : target.GetComponent<HealthComponent>().Health
                },
                {
                    "TARGET_HP_MISSING",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<HealthComponent>().MissingHealth
                        : target.GetComponent<HealthComponent>().MissingHealth
                },
                {
                    "TARGET_HP_FRACTION",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<HealthComponent>().HealthFraction
                        : target.GetComponent<HealthComponent>().HealthFraction
                },
                {
                    "TARGET_OVERHEAL",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<HealthComponent>().LastOverheal
                        : target.GetComponent<HealthComponent>().LastOverheal
                },
                {
                    "TARGET_LEVEL",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<ExperienceComponent>().Experience.Level
                        : target.GetComponent<ExperienceComponent>().Experience.Level
                },
                {
                    "TARGET_UNIT_LEVEL",
                    (caster, target, skill) => target == null
                        ? caster.GetComponent<UnitComponent>().Level
                        : target.GetComponent<UnitComponent>().Level
                },
                {
                    "LAST_AP_SPENT",
                    (caster, target, skill) => caster.GetComponent<SpellbookComponent>().LastUsedSkill
                        .GetCost(ResourceType.ActionPoint)
                },
                {
                    "LEVEL",
                    (caster, target, skill) => caster.GetComponent<ExperienceComponent>().Experience.Level
                },
                {
                    "OVERHEAL",
                    (caster, target, skill) => caster.GetComponent<HealthComponent>().LastOverheal
                },
                {
                    "SKILL_AP_COST",
                    (caster, target, skill) => skill?.GetBaseCost(ResourceType.ActionPoint) ?? 0
                },
                {
                    "SKILL_COOLDOWN",
                    (caster, target, skill) => skill?.Cooldown ?? 0
                },
                {
                    "SHIELD",
                    (caster, target, skill) => caster.GetComponent<HealthComponent>().Shield
                },
                {
                    "MAX_ATTRIBUTE",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().GetMaxAttribute()
                },
                {
                    "MIGHT",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().Get(AttributeType.Might).Value()
                },
                {
                    "CONSTITUTION",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().Get(AttributeType.Constitution).Value()
                },
                {
                    "DEFENSE",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().Get(AttributeType.Defense).Value()
                },
                {
                    "RESISTANCE",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().Get(AttributeType.Resistance).Value()
                },
                {
                    "AVERAGE_ATTRIBUTE",
                    (caster, target, skill) => caster.GetComponent<AttributesComponent>().GetAverageAttribute()
                },
                {
                    "HEALING_INCREASE",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.HealingIncrease).Value()
                },
                {
                    "ALCHEMY",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.Alchemy).Value()
                },
                {
                    "AP",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.AttackPower).Value()
                },
                {
                    "SP",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.SpellPower).Value()
                },
                {
                    "BLOCK",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.BlockChance).Value()
                },
                {
                    "THORNS",
                    (caster, target, skill) => caster.GetComponent<PropertiesComponent>().Get(PropertyType.Thorns).Value()
                },
                {
                    "HP_MAX",
                    (caster, target, skill) => caster.GetComponent<HealthComponent>().HealthMax
                },
                {
                    "HP",
                    (caster, target, skill) => caster.GetComponent<HealthComponent>().Health
                },
            };
    }
}