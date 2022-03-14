using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary
{
    public class UnitGroup
    {
        private readonly List<GameObject> entities;

        public UnitGroup()
        {
            this.entities = new List<GameObject>();
        }

        public void Add(GameObject entity)
        {
            this.entities.Add(entity);
        }

        public void Add(List<GameObject> entities)
        {
            this.entities.AddRange(entities);
        }

        public void Remove(List<GameObject> entities)
        {
            foreach (var entity in entities)
            {
                Remove(entity);
            }
        }

        public void Remove(GameObject entity)
        {
            this.entities.Remove(entity);
        }

        public List<GameObject> All()
        {
            return this.entities.ToList();
        }

        public void Clear()
        {
            this.entities.Clear();
        }

        public List<GameObject> Alive()
        {
            return this.entities.Where(entity => !entity.IsDummy() && entity.IsAlive()).ToList();
        }

        public List<GameObject> Alive(Func<GameObject, bool> predicate)
        {
            return this.entities.Where(entity => !entity.IsDummy() && entity.IsAlive() && predicate(entity)).ToList();
        }

        public List<GameObject> EnemiesOf(GameObject entity)
        {
            return this.entities.Where(e => !e.IsDummy() && e.IsEnemyOf(entity)).ToList();
        }

        public List<GameObject> Find(Func<GameObject, bool> predicate)
        {
            return this.entities.Where(predicate).ToList();
        }

        public List<GameObject> AliveInTeam(int teamId)
        {
            return this.entities.Where(entity => !entity.IsDummy() && entity.IsAlive() && entity.InTeam(teamId))
                .ToList();
        }
    }
}