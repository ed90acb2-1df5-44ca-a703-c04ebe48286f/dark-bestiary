using System;
using System.Collections.Generic;
using DarkBestiary.Behaviours;
using DarkBestiary.Components;
using DarkBestiary.Extensions;
using DarkBestiary.Messaging;
using DarkBestiary.Skills;
using DarkBestiary.UI.Views;
using DarkBestiary.Utility;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.UI.Controllers
{
    public class CombatLogViewController : ViewController<ICombatLogView>
    {
        public const int BufferSize = 1000;

        private const string ColorBlue = "<color=#40c7eb>";
        private const string ColorYellow = "<color=#fff569>";
        private const string ColorGreen = "<color=#00ff96>";
        private const string ColorRed = "<color=#ff2600>";
        private const string EndColor = "</color>";

        private static readonly LinkedList<string> Buffer = new LinkedList<string>();

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

            foreach (var entry in Buffer)
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
            if (behaviour.IsHidden || !behaviour.Target.IsVisible() || Game.Instance.State.IsTown)
            {
                return;
            }

            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                        I18N.Instance.Get("ui_combat_log_buff_applied")
                            .ToString(new object[]
                            {
                                ColorYellow + behaviour.Caster.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                                ColorBlue + behaviour.Name + EndColor,
                                ColorYellow + behaviour.Target.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
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
                                ColorBlue + behaviour.Name + EndColor,
                                ColorRed + behaviour.Target.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                            }));
        }

        private void OnEntityHealed(EntityHealedEventData data)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                        I18N.Instance.Get("ui_combat_log_healing")
                            .ToString(new object[]
                        {
                            ColorYellow + data.Healer.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                            ColorYellow + data.Target.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                            ColorGreen + (int) data.Healing + EndColor,
                        }));
        }

        private void OnEntityDamaged(EntityDamagedEventData data)
        {
            var row = $"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                      I18N.Instance.Get("ui_combat_log_damage")
                          .ToString(new object[]
                          {
                              ColorYellow + data.Attacker.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                              ColorYellow + data.Victim.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                              ColorRed + (int) data.Damage + EndColor,
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
                               ColorYellow + data.Victim.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                           }));
            }
            else
            {
                AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                       I18N.Instance.Get("ui_combat_log_kill")
                           .ToString(new object[]
                           {
                               ColorYellow + data.Killer.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                               ColorYellow + data.Victim.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                           }));
            }
        }

        private void OnEntityUsedSkillOnEntity(GameObject caster, GameObject target, Skill skill)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                        I18N.Instance.Get("ui_combat_log_skill_used_on_entity")
                            .ToString(new object[]
                        {
                            ColorYellow + caster.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                            ColorBlue + skill.Name + EndColor,
                            ColorYellow + target.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor
                        }));
        }

        private void OnEntityUsedSkillOnPoint(GameObject caster, Vector3 target, Skill skill)
        {
            AddRow($"<color=#888888>[{DateTime.Now.ToLongTimeString()}]</color> " +
                        I18N.Instance.Get("ui_combat_log_skill_used_on_point")
                            .ToString(new object[]
                        {
                            ColorYellow + caster.GetComponent<UnitComponent>().GetNameOrLabel() + EndColor,
                            ColorBlue + skill.Name + EndColor,
                            target.x,
                            target.y
                        }));
        }

        private void AddRow(string row)
        {
            Buffer.AddLast(row);

            if (Buffer.Count > BufferSize)
            {
                Buffer.RemoveFirst();
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