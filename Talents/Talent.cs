using DarkBestiary.Components;
using DarkBestiary.Data;
using DarkBestiary.Messaging;
using UnityEngine;
using Behaviour = DarkBestiary.Behaviours.Behaviour;

namespace DarkBestiary.Talents
{
    public class Talent
    {
        public static readonly Talent Empty = new Talent();

        public event Payload<Talent> Learned;
        public event Payload<Talent> Unlearned;

        public int Id { get; }
        public int Tier { get; }
        public int Index { get; }
        public I18NString Name { get; }
        public I18NString Description { get; }
        public string Icon { get; }
        public Behaviour Behaviour { get; }
        public TalentCategory Category { get; }
        public bool IsLearned { get; private set; }

        public Talent(TalentData data, Behaviour behaviour, TalentCategory category)
        {
            Id = data.Id;
            Tier = data.Tier;
            Index = data.Index;
            Name = I18N.Instance.Get(data.NameKey);
            Description = I18N.Instance.Get(data.DescriptionKey);
            Icon = data.Icon;
            Behaviour = behaviour;
            Category = category;
        }

        private Talent()
        {
            Id = -1;
        }

        public void Learn(GameObject entity)
        {
            if (Behaviour == null)
            {
                Debug.LogWarning($"Talent {Name} have no behaviour");
            }
            else
            {
                entity.GetComponent<BehavioursComponent>()?.ApplyStack(Behaviour, entity);
            }

            IsLearned = true;
            Learned?.Invoke(this);
        }

        public void Unlearn(GameObject entity)
        {
            if (!IsLearned)
            {
                return;
            }

            if (Behaviour == null)
            {
                Debug.LogWarning($"Talent {Name} have no behaviour");
            }
            else
            {
                entity.GetComponent<BehavioursComponent>()?.RemoveStack(Behaviour.Id);
            }

            IsLearned = false;
            Unlearned?.Invoke(this);
        }
    }
}
