using System;
using DarkBestiary.Dialogues;
using UnityEngine;

namespace DarkBestiary.Components
{
    public class NarratorComponent : MonoBehaviour
    {
        public Narrator Narrator => (Narrator) Enum.Parse(typeof(Narrator), this.narrator);

        [SerializeField] private string narrator;
    }
}