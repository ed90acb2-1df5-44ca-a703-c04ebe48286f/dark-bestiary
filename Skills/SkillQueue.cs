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
        private static readonly Dictionary<GameObject, Queue<SkillQueueInfo>> Queue =
            new Dictionary<GameObject, Queue<SkillQueueInfo>>();

        private static readonly Dictionary<GameObject, Action> QueueCallbacks =
            new Dictionary<GameObject, Action>();

        public static void Enqueue(Skill skill, object target)
        {
            MarkDangerousCell(skill, target);

            if (!Queue.ContainsKey(skill.Caster))
            {
                Queue.Add(skill.Caster, new Queue<SkillQueueInfo>());
            }

            Queue[skill.Caster].Enqueue(new SkillQueueInfo(skill, target));
        }

        public static void Clear(GameObject entity)
        {
            if (Queue.ContainsKey(entity))
            {
                Queue[entity].Clear();
            }
        }

        public static void Run(GameObject entity, Action callback)
        {
            if (!Queue.ContainsKey(entity) || Queue[entity].Count == 0)
            {
                callback.Invoke();
                return;
            }

            QueueCallbacks[entity] = callback;

            while (Queue[entity].Count > 0)
            {
                var info = Queue[entity].Dequeue();

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
            foreach (var cell in BoardNavigator.Instance.WithinShape(skill.Caster.transform.position, target.GetPosition(), skill.AOEShape, skill.AOE).Where(c => c.IsWalkable))
            {
                cell.Dangerous();
            }
        }

        private static void OnEffectFinished(Effect effect)
        {
            if (Queue[effect.Caster].Count > 0)
            {
                return;
            }

            QueueCallbacks[effect.Caster].Invoke();
        }
    }
}