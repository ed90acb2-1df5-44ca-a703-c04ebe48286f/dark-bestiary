using System;
using UnityEngine;

namespace DarkBestiary
{
    public class Resource
    {
        public event Action<Resource> Changed;

        public string Name { get; }
        public ResourceType Type { get; }
        public GameObject Owner { get; }

        public float MaxAmount
        {
            get => m_AmountMax;
            set
            {
                m_AmountMax = value;
                Changed?.Invoke(this);
            }
        }

        public float Amount
        {
            get => m_Amount;
            set
            {
                m_Amount = Mathf.Clamp(value, 0, MaxAmount);

                Changed?.Invoke(this);
            }
        }

        private float m_Amount;
        private float m_AmountMax;

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