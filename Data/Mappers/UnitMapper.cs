using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Items;
using DarkBestiary.Pathfinding;
using UnityEngine;
using Component = DarkBestiary.Components.Component;
using Object = UnityEngine.Object;

namespace DarkBestiary.Data.Mappers
{
    public class UnitMapper : Mapper<UnitData, GameObject>
    {
        private readonly IAttributeRepository attributeRepository;
        private readonly IPropertyRepository propertyRepository;
        private readonly IBehaviourTreeRepository behaviourTreeRepository;
        private readonly IBehaviourRepository behaviourRepository;
        private readonly ISkillRepository skillRepository;
        private readonly IArchetypeDataRepository archetypeDataRepository;
        private readonly ILootDataRepository lootDataRepository;
        private readonly IItemRepository itemRepository;
        private readonly IPathfinder pathfinder;

        public UnitMapper(
            IAttributeRepository attributeRepository,
            IPropertyRepository propertyRepository,
            IBehaviourTreeRepository behaviourTreeRepository,
            IBehaviourRepository behaviourRepository,
            ISkillRepository skillRepository,
            IArchetypeDataRepository archetypeDataRepository,
            ILootDataRepository lootDataRepository,
            IItemRepository itemRepository,
            IPathfinder pathfinder)
        {
            this.attributeRepository = attributeRepository;
            this.propertyRepository = propertyRepository;
            this.behaviourTreeRepository = behaviourTreeRepository;
            this.behaviourRepository = behaviourRepository;
            this.skillRepository = skillRepository;
            this.archetypeDataRepository = archetypeDataRepository;
            this.lootDataRepository = lootDataRepository;
            this.itemRepository = itemRepository;
            this.pathfinder = pathfinder;
        }

        public override UnitData ToData(GameObject data)
        {
            throw new NotImplementedException();
        }

        public override GameObject ToEntity(UnitData data)
        {
            var entity = new GameObject(data.NameKey);

            var components = new List<Component>
            {
                entity.AddComponent<ThreatComponent>(),
                entity.AddComponent<UnitComponent>().Construct(data),
                entity.AddComponent<PropertiesComponent>().Construct(this.propertyRepository.FindAll()),
                entity.AddComponent<AttributesComponent>().Construct(this.attributeRepository.FindAll()),
                entity.AddComponent<MovementComponent>().Construct(this.pathfinder),
                entity.AddComponent<ActorComponent>().Construct(CreateModel(entity.transform, data.Model)),
                entity.AddComponent<SpellbookComponent>().Construct(),
                entity.AddComponent<DefenseComponent>(),
                entity.AddComponent<OffenseComponent>(),
                entity.AddComponent<ResourcesComponent>(),
                entity.AddComponent<BehavioursComponent>(),
                entity.AddComponent<HealthComponent>(),
                entity.AddComponent<AttackOfOpportunityComponent>(),
            };

            var archetypeData = this.archetypeDataRepository.Find(data.ArchetypeId);

            if (archetypeData != null)
            {
                components.Add(entity.AddComponent<ArchetypeComponent>().Construct(archetypeData));
            }

            if (data.Flags.HasFlag(UnitFlags.BlocksMovement))
            {
                components.Add(entity.AddComponent<MovementBlockerComponent>());
            }

            components.ForEach(component => component.Initialize());

            GiveRigidbody(entity);
            GiveAi(data, entity);
            GiveSkills(data, entity);
            GiveBehaviours(data, entity);
            GiveLoot(data, entity);
            GiveEquipment(data, entity);

            entity.GetComponent<HealthComponent>().Restore();

            return entity;
        }

        private void GiveEquipment(UnitData data, GameObject entity)
        {
            if (data.Equipment.Count == 0)
            {
                return;
            }

            var equipment = entity.AddComponent<EquipmentComponent>();
            equipment.Construct();
            equipment.Initialize();

            foreach (var item in data.Equipment.Select(this.itemRepository.Find))
            {
                item.ChangeOwner(entity);
                equipment.Equip(item);
            }
        }

        private void GiveLoot(UnitData data, GameObject entity)
        {
            var loot = new List<Loot>();

            foreach (var tableId in data.Loot)
            {
                loot.Add(Container.Instance.Instantiate<Loot>(new object[] {this.lootDataRepository.Find(tableId)}));
            }

            entity.AddComponent<LootComponent>().Construct(loot);
        }

        private static void GiveRigidbody(GameObject entity)
        {
            entity.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;

            var collider = entity.AddComponent<CircleCollider2D>();
            entity.layer = 2;
            collider.radius = 0.01f;
            collider.isTrigger = true;
        }

        private void GiveAi(UnitData data, GameObject entity)
        {
            var behaviourTree = this.behaviourTreeRepository.Find(data.BehaviourTreeId);

            if (behaviourTree != null)
            {
                entity.AddComponent<AiComponent>().Construct(behaviourTree).Initialize();
            }
        }

        private void GiveSkills(UnitData data, GameObject entity)
        {
            var skillsComponent = entity.GetComponent<SpellbookComponent>();

            foreach (var skill in this.skillRepository.Find(data.Skills))
            {
                skillsComponent.PlaceOnActionBar(skill);
            }
        }

        private void GiveBehaviours(UnitData data, GameObject entity)
        {
            // Note: Wait for owner assignment

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                var behavioursComponent = entity.GetComponent<BehavioursComponent>();

                foreach (var behaviour in this.behaviourRepository.Find(data.Behaviours))
                {
                    behavioursComponent.ApplyAllStacks(behaviour, entity);
                }
            });
        }

        private Model CreateModel(Transform parent, string path)
        {
            var prefab = Resources.Load<Model>(path);

            if (prefab == null)
            {
                Debug.LogError("Model not found at path: " + path);
                return CreateDefaultModel(parent);
            }

            return Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        }

        private Model CreateDefaultModel(Transform parent)
        {
            return Object.Instantiate(
                Resources.Load<Model>("Prefabs/Empty"), Vector3.zero, Quaternion.identity, parent
            );
        }
    }
}