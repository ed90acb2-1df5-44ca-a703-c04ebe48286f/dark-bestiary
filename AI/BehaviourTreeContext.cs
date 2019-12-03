using System;
using System.Collections.Generic;
using DarkBestiary.Scenarios.Encounters;
using UnityEngine;

namespace DarkBestiary.AI
{
    public class BehaviourTreeContext
    {
        public GameObject Entity { get; set; }

        public GameObject TargetEntity
        {
            get => this.targetEntity;
            set
            {
                PreviousTargetEntity = this.targetEntity;
                this.targetEntity = value;
            }
        }

        public GameObject PreviousTargetEntity { get; private set; }

        public Vector3? TargetPoint { get; set; }
        public CombatEncounter Combat { get; set; }
        public List<BehaviourTreeNode> OpenedNodes { get; } = new List<BehaviourTreeNode>();
        public Dictionary<BehaviourTreeNode, int> RunningChildIndex { get; } = new Dictionary<BehaviourTreeNode, int>();

        private GameObject targetEntity;

        public Vector3 RequireTargetPoint()
        {
            if (TargetPoint == null)
            {
                throw new Exception(GetType().Name + ": target point not set");
            }

            return (Vector3) TargetPoint;
        }

        public GameObject RequireTargetEntity()
        {
            if (TargetEntity == null)
            {
                throw new Exception(GetType() + ": target entity not set");
            }

            return TargetEntity;
        }
    }
}