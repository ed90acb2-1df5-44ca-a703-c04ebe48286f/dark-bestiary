using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DarkBestiary.AI;

namespace DarkBestiary.Data.Mappers
{
    public class BehaviourTreeMapper : Mapper<BehaviourTreeData, BehaviourTree>
    {
        private static readonly Dictionary<string, Type> Mapping = new Dictionary<string, Type>();

        static BehaviourTreeMapper()
        {
            Assembly.GetAssembly(typeof(BehaviourTreeNode))
                .GetTypes()
                .Where(type => type.IsClass && type.IsSubclassOf(typeof(BehaviourTreeNode)) && !type.IsAbstract)
                .ToList()
                .ForEach(type => Mapping.Add(type.Name, type));
        }

        public override BehaviourTreeData ToData(BehaviourTree target)
        {
            throw new NotImplementedException();
        }

        public override BehaviourTree ToEntity(BehaviourTreeData data)
        {
            var builder = new BehaviourTreeBuilder();
            BuildRecursive(builder, data);

            var tree = builder.Build();
            tree.Id = data.Id;

            return tree;
        }

        private static BehaviourTreeNode CreateNodeByType(Type type, BehaviourTreePropertiesData properties)
        {
            if (type.IsSubclassOf(typeof(BehaviourTreeLogicNode)))
            {
                return Container.Instance.Instantiate(type, new object[] {properties}) as BehaviourTreeNode;
            }

            return Container.Instance.Instantiate(type) as BehaviourTreeNode;
        }

        private static void BuildRecursive(BehaviourTreeBuilder builder, BehaviourTreeData data)
        {
            if (!Mapping.ContainsKey(data.Type))
            {
                throw new Exception($"Unknown node type {data.Type}");
            }

            var node = CreateNodeByType(Mapping[data.Type], data.Properties);
            var parent = node as IBehaviourTreeNodeParent;

            if (parent != null)
            {
                builder.AddParent(parent);
            }
            else
            {
                builder.AddChild(node);
            }

            foreach (var childData in data.Children)
            {
                BuildRecursive(builder, childData);
            }

            if (parent != null)
            {
                builder.End();
            }
        }
    }
}