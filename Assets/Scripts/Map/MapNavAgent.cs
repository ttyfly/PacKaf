/*
 * Copyright (c) 2021 Lu Kangyuan (ttyfly@126.com)
 *
 * This file is part of PacKaf.
 *
 * PacKaf is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * PacKaf is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with PacKaf.  If not, see <https://www.gnu.org/licenses/>.
 */

using UnityEngine;

namespace PacKaf {
    public class MapNavAgent : MonoBehaviour {

        public enum AgentControlMode {
            Passive,
            Chasing,
            Wandering
        }

        public enum AgentChaseMode {
            Simple,
            Predictive,
            FurtherPredictive,
            Shy
        }

        [SerializeField]
        private AgentChaseMode chaseMode;

        [SerializeField]
        private AgentControlMode controlMode;

        [SerializeField]
        private float maxSpeed;

        private MapNav2D map;

        private void Start() {
            map = GameObject.Find("Map").GetComponent<MapNav2D>();

            MapNode nearestNode = map.GetNearestNode(this);
            SetNodes(nearestNode, nearestNode.GetNearestEdge(this).neighbor);
        }

        private void Update() {

            UpdateCosts();

            if (controlMode == AgentControlMode.Passive) {

                if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
                    CurrentNode = NextNode;
                }

                NextNode = CurrentNode.GetNearestEdge(this).neighbor;

            } else if (controlMode == AgentControlMode.Chasing && ChaseTarget != null) {

                if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
                    if (chaseMode == AgentChaseMode.Simple) {
                        SetNodes(NextNode, map.NextNode(NextNode, ChaseTarget));
                    } else if (chaseMode == AgentChaseMode.Predictive) {
                        SetNodes(NextNode, map.NextNode(NextNode, ChaseTarget.NextNode));
                    }
                }

                // 如果可以转向
                if (BackwardCostPerUnit != float.PositiveInfinity) {
                    if (chaseMode == AgentChaseMode.Simple && map.NextNode(this, ChaseTarget) != NextNode) {
                        SetNodes(NextNode, CurrentNode);
                    } else if (chaseMode == AgentChaseMode.Predictive && map.NextNode(this, ChaseTarget.NextNode) != NextNode) {
                        SetNodes(NextNode, CurrentNode);
                    }
                }

                transform.position = Vector2.MoveTowards(transform.position, NextNode.Position, maxSpeed * Time.deltaTime);

            } else if (controlMode == AgentControlMode.Wandering) {

            }

            // ------

        }

        private void SetNodes(MapNode currentNode, MapNode nextNode) {
            if (!currentNode.HasNeighbor(nextNode)) {
                throw new System.Exception("Next node should be a neighbor of current node.");
            }

            CurrentNode = currentNode;
            NextNode = nextNode;

            ForwardCostPerUnit = currentNode.GetEdgeCostPerUnit(nextNode);
            BackwardCostPerUnit = nextNode.HasNeighbor(currentNode) ? nextNode.GetEdgeCostPerUnit(currentNode) : float.PositiveInfinity;

            UpdateCosts();
        }

        private void UpdateCosts() {
            CurrentNodeToAgentCost = ForwardCostPerUnit * DistanceToNode(CurrentNode);
            NextNodeToAgentCost = BackwardCostPerUnit * DistanceToNode(NextNode);
            AgentToCurrentNodeCost = BackwardCostPerUnit * DistanceToNode(CurrentNode);
            AgentToNextNodeCost = ForwardCostPerUnit * DistanceToNode(NextNode);
        }

        /// <summary>
        /// 此 agent 到某节点的距离。
        /// </summary>
        /// <param name="node">节点</param>
        /// <returns>距离</returns>
        public float DistanceToNode(MapNode node) {
            return Vector2.Distance(transform.position, node.Position);
        }

        /// <summary>
        /// 此 agent 到某条边（线段）的距离。<br/>
        /// 需自行保证 to 是 from 的邻居，否则调用此函数可能是无意义的。
        /// </summary>
        /// <param name="from">边起始节点</param>
        /// <param name="to">边结束节点</param>
        /// <returns>距离</returns>
        public float DistanceToEdge(MapNode from, MapNode to) {
            float x1 = from.Position.x;
            float y1 = from.Position.y;
            float x2 = to.Position.x;
            float y2 = to.Position.y;
            float x = transform.position.x;
            float y = transform.position.y;

            float cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
            if (cross <= 0) return DistanceToNode(from);

            float d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);
            if (cross >= d2) return DistanceToNode(to);

            float r = cross / d2;
            float px = x1 + (x2 - x1) * r;
            float py = y1 + (y2 - y1) * r;
            return Mathf.Sqrt((x - px) * (x - px) + (py - y) * (py - y));
        }

        public MapNavAgent ChaseTarget { get; set; }

        public MapNode CurrentNode { get; private set; }
        public MapNode NextNode { get; private set; }

        public float ForwardCostPerUnit { get; private set; }
        public float BackwardCostPerUnit { get; private set; }

        public float CurrentNodeToAgentCost { get; private set; }
        public float NextNodeToAgentCost { get; private set; }
        public float AgentToCurrentNodeCost { get; private set; }
        public float AgentToNextNodeCost { get; private set; }
    }
}