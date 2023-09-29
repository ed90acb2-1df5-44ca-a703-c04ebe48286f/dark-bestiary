using System;
using System.Collections.Generic;
using System.Linq;
using DarkBestiary.Components;
using DarkBestiary.Data.Repositories;
using DarkBestiary.Pathfinding;
using DarkBestiary.Properties;
using UnityEngine;
using Component = DarkBestiary.Components.Component;
using Object = UnityEngine.Object;

namespace DarkBestiary.Data.Mappers
{
    public class UnitMapper : Mapper<UnitData, GameObject>
    {
        private readonly IAttributeRepository m_AttributeRepository;
        private readonly IPropertyRepository m_PropertyRepository;
        private readonly IBehaviourTreeRepository m_BehaviourTreeRepository;
        private readonly IBehaviourRepository m_BehaviourRepository;
        private readonly ISkillRepository m_SkillRepository;
        private readonly IArchetypeDataRepository m_ArchetypeDataRepository;
        private readonly ILootDataRepository m_LootDataRepository;
        private readonly IItemRepository m_ItemRepository;
        private readonly IPathfinder m_Pathfinder;

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
            m_AttributeRepository = attributeRepository;
            m_PropertyRepository = propertyRepository;
            m_BehaviourTreeRepository = behaviourTreeRepository;
            m_BehaviourRepository = behaviourRepository;
            m_SkillRepository = skillRepository;
            m_ArchetypeDataRepository = archetypeDataRepository;
            m_LootDataRepository = lootDataRepository;
            m_ItemRepository = itemRepository;
            m_Pathfinder = pathfinder;
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
                entity.AddComponent<PropertiesComponent>().Construct(m_PropertyRepository.FindAll()),
                entity.AddComponent<AttributesComponent>().Construct(m_AttributeRepository.FindAll()),
                entity.AddComponent<MovementComponent>().Construct(m_Pathfinder),
                entity.AddComponent<ActorComponent>().Construct(CreateModel(entity.transform, data.Model)),
                entity.AddComponent<SpellbookComponent>().Construct(),
                entity.AddComponent<DefenseComponent>(),
                entity.AddComponent<OffenseComponent>(),
                entity.AddComponent<ResourcesComponent>(),
                entity.AddComponent<BehavioursComponent>(),
                entity.AddComponent<HealthComponent>(),
                entity.AddComponent<AttackOfOpportunityComponent>(),
            };

            var archetypeData = m_ArchetypeDataRepository.Find(data.ArchetypeId);

            if (archetypeData != null)
            {
                components.Add(entity.AddComponent<ArchetypeComponent>().Construct(archetypeData));
            }

            if (data.Flags.HasFlag(UnitFlags.BlocksMovement))
            {
                components.Add(entity.AddComponent<MovementBlockerComponent>());
            }

            var maximumActionPoints = entity.GetComponent<PropertiesComponent>().Get(PropertyType.MaximumActionPoints);

            if (data.Flags.HasFlag(UnitFlags.Playable))
            {
                maximumActionPoints.Change(10);
            }
            else
            {
                maximumActionPoints.Change(6);
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

            foreach (var item in data.Equipment.Select(m_ItemRepository.Find))
            {
                item.ChangeOwner(entity);
                equipment.Equip(item);
            }
        }

        private void GiveLoot(UnitData data, GameObject entity)
        {
            entity.AddComponent<LootComponent>().Construct(data.Loot);
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
            var behaviourTree = m_BehaviourTreeRepository.Find(data.BehaviourTreeId);

            if (behaviourTree != null)
            {
                entity.AddComponent<AiComponent>().Construct(behaviourTree).Initialize();
            }
        }

        private void GiveSkills(UnitData data, GameObject entity)
        {
            var skillsComponent = entity.GetComponent<SpellbookComponent>();

            foreach (var skill in m_SkillRepository.Find(data.Skills))
            {
                skillsComponent.Learn(skill);
            }
        }

        private void GiveBehaviours(UnitData data, GameObject entity)
        {
            // Note: Wait for owner assignment

            Timer.Instance.WaitForFixedUpdate(() =>
            {
                var behavioursComponent = entity.GetComponent<BehavioursComponent>();

                foreach (var behaviour in m_BehaviourRepository.Find(data.Behaviours))
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