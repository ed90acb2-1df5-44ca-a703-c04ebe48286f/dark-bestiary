using DarkBestiary.Data;
using UnityEngine;

namespace DarkBestiary
{
    public class Character
    {
        public int Id { get; }
        public string Name { get; set; }
        public GameObject Entity { get; set; }
        public CharacterData Data { get; }

        public Character(CharacterData data, GameObject entity)
        {
            Id = data.Id;
            Name = data.Name;
            Data = data;
            Entity = entity;
        }
    }
}