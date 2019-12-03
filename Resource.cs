using DarkBestiary.Messaging;
using UnityEngine;

namespace DarkBestiary
{
    public class Resource
    {
        public event Payload<Resource> Changed;

        public string Name { get; }
        public ResourceType Type { get; }
        public GameObject Owner { get; }

        public float MaxAmount
        {
            get => this.amountMax;
            set
            {
                this.amountMax = value;
                Changed?.Invoke(this);
            }
        }

        public float Amount
        {
            get => this.amount;
            set
            {
                this.amount = Mathf.Clamp(value, 0, MaxAmount);

                Changed?.Invoke(this);
            }
        }

        private float amount;
        private float amountMax;

        public Resource(GameObject owner, string name, ResourceType type, float amount, float maxAmount)
        {
            Owner = owner;
            Name = name;
            Type = type;
            Amount = amount;
            MaxAmount = maxAmount;
        }
    }
}