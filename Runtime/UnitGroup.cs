using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Extensions;
using UnityEngine;

namespace DarkBestiary
{
    public class UnitGroup
    {
        private readonly List<GameObject> m_Entities;

        public UnitGroup()
        {
            m_Entities = new List<GameObject>();
        }

        public void Add(GameObject entity)
        {
            m_Entities.Add(entity);
        }

        public void Add(List<GameObject> entities)
        {
            m_Entities.AddRange(entities);
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
            m_Entities.Remove(entity);
        }

        public List<GameObject> All()
        {
            return m_Entities.ToList();
        }

        public void Clear()
        {
            m_Entities.Clear();
        }

        public List<GameObject> Alive()
        {
            return m_Entities.Where(entity => !entity.IsDummy() && entity.IsAlive()).ToList();
        }

        public List<GameObject> Alive(Func<GameObject, bool> predicate)
        {
            return m_Entities.Where(entity => !entity.IsDummy() && entity.IsAlive() && predicate(entity)).ToList();
        }

        public List<GameObject> EnemiesOf(GameObject entity)
        {
            return m_Entities.Where(e => !e.IsDummy() && e.IsEnemyOf(entity)).ToList();
        }

        public List<GameObject> Find(Func<GameObject, bool> predicate)
        {
            return m_Entities.Where(predicate).ToList();
        }

        public List<GameObject> AliveInTeam(int teamId)
        {
            return m_Entities.Where(entity => !entity.IsDummy() && entity.IsAlive() && entity.InTeam(teamId))
                .ToList();
        }
    }
}