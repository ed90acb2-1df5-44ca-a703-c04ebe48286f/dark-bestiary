using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Events;
using DarkBestiary.Extensions;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;
using DarkBestiary.Utility;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class CombatLogViewController : ViewController<ICombatLogView>
    {
        public const int c_BufferSize = 1000;

        private const string c_ColorBlue = "<color=#40c7eb>";
        private const string c_ColorYellow = "<color=#fff569>";
        private const string c_ColorGreen = "<color=#00ff96>";
        private const string c_ColorRed = "<color=#ff2600>";
        private const string c_EndColor = "</color>";

        private static readonly LinkedList<string> s_Buffer = new();

        public CombatLogViewController(ICombatLogView view) : base(view)
        {
        }

        protected override void OnInitialize()
        {
            View.CloseButtonClicked += OnCloseButtonClicked;

            Skill.AnySkillUsing += OnSkillUsing;
            HealthComponent.AnyEntityDied += OnEntityDied;
            BehavioursComponent.AnyBehaviourApplied += OnBehaviourApplied;
            BehavioursComponent.AnyBehaviourRemoved += OnBehaviourRemoved;
            HealthComponent.AnyEntityDamaged += OnEntityDamaged;
            HealthComponent.AnyEntityHealed += OnEntityHealed;

            foreach (var entry in s_Buffer)
            {
                View.Add(entry);
            }
        }

        protected override void OnTerminate()
        {
            View.CloseButtonClicked -= OnCloseButtonClicked;

            Skill.AnySkillUsing -= OnSkillUsing;
            HealthComponent.AnyEntityDied -= OnEntityDied;
            BehavioursComponent.AnyBehaviourApplied -= OnBehaviourApplied;
            BehavioursComponent.AnyBehaviourRemoved -= OnBehaviourRemoved;
            HealthComponent.AnyEntityDamaged -= OnEntityDamaged;
            HealthComponent.AnyEntityHealed -= OnEntityHealed;
        }

        private void OnCloseButtonClicked()
        {
            View.Hide();
        }

        private void OnBehaviourApplied(Behaviour behaviour)
        {
            if (behaviour.IsHidden || !behaviour.Target.IsVisible() || Game.Instance.IsTown)
            {
                return;
            }

            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                   I18N.Instance.Get("ui_combat_log_buff_applied")
                       .ToString(new object[]
                       {
                           c_ColorYellow + behaviour.Caster.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           c_ColorBlue + behaviour.Name + c_EndColor,
                           c_ColorYellow + behaviour.Target.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                       }));
        }

        private void OnBehaviourRemoved(Behaviour behaviour)
        {
            if (!(behaviour is BuffBehaviour))
            {
                return;
            }

            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                   I18N.Instance.Get("ui_combat_log_buff_removed")
                       .ToString(new object[]
                       {
                           c_ColorBlue + behaviour.Name + c_EndColor,
                           c_ColorRed + behaviour.Target.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                       }));
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                   I18N.Instance.Get("ui_combat_log_healing")
                       .ToString(new object[]
                       {
                           c_ColorYellow + data.Source.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           c_ColorYellow + data.Target.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           c_ColorGreen + (int) data.Healing + c_EndColor,
                       }));
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            var row = $"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                      I18N.Instance.Get("ui_combat_log_damage")
                          .ToString(new object[]
                          {
                              c_ColorYellow + data.Source.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                              c_ColorYellow + data.Target.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                              c_ColorRed + (int) data.Damage + c_EndColor,
                              EnumTranslator.Translate(data.Damage.Type)
                          });

            var flags = new List<string>();

            if (data.Damage.IsCritical())
            {
                flags.Add(I18N.Instance.Get("ui_critical"));
            }

            if (data.Damage.IsBlocked())
            {
                flags.Add(I18N.Instance.Get("ui_block"));
            }

            if (data.Damage.IsDodged())
            {
                flags.Add(I18N.Instance.Get("ui_dodge"));
            }

            if (flags.Count > 0)
            {
                row += $" ({string.Join(", ", flags)})";
            }

            AddRow(row);
        }

        private void OnEntityDied(EntityDiedEventData data)
        {
            if (data.Killer == data.Victim)
            {
                AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                       I18N.Instance.Get("ui_combat_log_death")
                           .ToString(new object[]
                           {
                               c_ColorYellow + data.Victim.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           }));
            }
            else
            {
                AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                       I18N.Instance.Get("ui_combat_log_kill")
                           .ToString(new object[]
                           {
                               c_ColorYellow + data.Killer.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                               c_ColorYellow + data.Victim.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           }));
            }
        }

        private void OnEntityUsedSkillOnEntity(GameObject caster, GameObject target, Skill skill)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                   I18N.Instance.Get("ui_combat_log_skill_used_on_entity")
                       .ToString(new object[]
                       {
                           c_ColorYellow + caster.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           c_ColorBlue + skill.Name + c_EndColor,
                           c_ColorYellow + target.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor
                       }));
        }

        private void OnEntityUsedSkillOnPoint(GameObject caster, Vector3 target, Skill skill)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                   I18N.Instance.Get("ui_combat_log_skill_used_on_point")
                       .ToString(new object[]
                       {
                           c_ColorYellow + caster.GetComponent<UnitComponent>().GetNameOrLabel() + c_EndColor,
                           c_ColorBlue + skill.Name + c_EndColor,
                           target.x,
                           target.y
                       }));
        }

        private void AddRow(string row)
        {
            s_Buffer.AddLast(row);

            if (s_Buffer.Count > c_BufferSize)
            {
                s_Buffer.RemoveFirst();
            }

            View.Add(row);
        }

        private void OnSkillUsing(SkillUseEventData data)
        {
            var targetEntity = data.Target as GameObject;

            if (targetEntity != null)
            {
                OnEntityUsedSkillOnEntity(data.Caster, targetEntity, data.Skill);
                return;
            }

            OnEntityUsedSkillOnPoint(data.Caster, (Vector3) data.Target, data.Skill);
        }
    }
}