using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarkBestiary.Data.Repositories
{
    public interface IUnitRepository : IRepository<int, GameObject>
    {
        List<GameObject> FindPlayable();

        List<GameObject> Find(Func<UnitData, bool> predicate);

        GameObject Random(Func<UnitData, bool> predicate);
    }
}