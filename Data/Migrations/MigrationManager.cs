using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DarkBestiary.Data.Migrations
{
    public class MigrationManager
    {
        public static void Migrate()
        {
            var migrationTypes = Assembly.GetAssembly(typeof(IMigration))
                .GetTypes()
                .Where(type => type.IsClass && type.GetInterfaces().Contains(typeof(IMigration)) && !type.IsAbstract)
                .OrderBy(type => type.Name)
                .ToList();

            foreach (var migrationType in migrationTypes)
            {
                var migration = Container.Instance.Instantiate(migrationType) as IMigration;
                migration?.Migrate();

                Debug.Log($"Migration: {migrationType.Name} successfully migrated.");
            }
        }
    }
}