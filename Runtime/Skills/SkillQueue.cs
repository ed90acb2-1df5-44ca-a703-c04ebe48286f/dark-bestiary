using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Effects;
using DarkBestiary.Extensions;
using DarkBestiary.GameBoard;
using UnityEngine;

namespace DarkBestiary.Skills
{
    public static class SkillQueue
    {
        private static readonly Dictionary<GameObject, Queue<SkillQueueInfo>> s_Queue = new();

        private static readonly Dictionary<GameObject, Action> s_QueueCallbacks = new();

        public static void Enqueue(Skill skill, object target)
        {
            MarkDangerousCell(skill, target);

            if (!s_Queue.ContainsKey(skill.Caster))
            {
                s_Queue.Add(skill.Caster, new Queue<SkillQueueInfo>());
            }

            s_Queue[skill.Caster].Enqueue(new SkillQueueInfo(skill, target));
        }

        public static void Clear(GameObject entity)
        {
            if (s_Queue.ContainsKey(entity))
            {
                s_Queue[entity].Clear();
            }
        }

        public static void Run(GameObject entity, Action callback)
        {
            if (!s_Queue.ContainsKey(entity) || s_Queue[entity].Count == 0)
            {
                callback.Invoke();
                return;
            }

            s_QueueCallbacks[entity] = callback;

            while (s_Queue[entity].Count > 0)
            {
                var info = s_Queue[entity].Dequeue();

                info.Skill.FaceTargetAndPlayAnimation(info.Target, () =>
                {
                    var clone = info.Skill.Effect.Clone();
                    clone.Skill = info.Skill;
                    clone.Finished += OnEffectFinished;
                    clone.Apply(info.Skill.Caster, info.Target);
                });
            }
        }

        private static void MarkDangerousCell(Skill skill, object target)
        {
            foreach (var cell in BoardNavigator.Instance.WithinShape(skill.Caster.transform.position, target.GetPosition(), skill.AoeShape, skill.Aoe).Where(c => c.IsWalkable))
            {
                cell.Dangerous();
            }
        }

        private static void OnEffectFinished(Effect effect)
        {
            if (s_Queue[effect.Caster].Count > 0)
            {
                return;
            }

            s_QueueCallbacks[effect.Caster].Invoke();
        }
    }
}