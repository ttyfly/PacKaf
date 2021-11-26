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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace PacKaf {
    [DisallowMultipleComponent]
    public class MapNav2D : MonoBehaviour, ISerializationCallbackReceiver {

        [SerializeField]
        private List<MapNode> nodes;

        private PriorityQueue<MapNode> nodePQ;
        private MapNode sourceNode1 = null, sourceNode2 = null;

        private void Awake() {
            nodePQ = new PriorityQueue<MapNode>(false);
        }

        /// <summary>
        /// 序列化前回调。
        /// </summary>
        public void OnBeforeSerialize() {
            foreach (MapNode node in nodes) {
                for (int i = 0; i < node.Edges.Count; i++) {
                    MapEdge edge = node.Edges[i];
                    edge.neighborIndex = nodes.IndexOf(edge.neighbor);
                    node.Edges[i] = edge;
                }
            }
        }

        /// <summary>
        /// 反序列化后回调。
        /// </summary>
        public void OnAfterDeserialize() {
            foreach (MapNode node in nodes) {
                node.ClearFlags();
                for (int i = 0; i < node.Edges.Count; i++) {
                    MapEdge edge = node.Edges[i];
                    edge.neighbor = nodes[edge.neighborIndex];
                    node.Edges[i] = edge;
                }
            }
        }

        /// <summary>
        /// 标记节点可达性。<br/>
        /// 使用堆优化的 dijkstra 算法。源节点可达的所有 MapNode 对象的 flags 均会更新。
        /// </summary>
        /// <param name="sourceNode">源节点</param>
        /// <param name="initialCost">抵达源节点的花费</param>
        public void TagReachableNodes(MapNode sourceNode1, float initialCost1 = 0, MapNode sourceNode2 = null, float initialCost2 = 0) {

            if (sourceNode1 == null) {
                throw new ArgumentNullException(nameof(sourceNode1));
            }

            if (initialCost1 < 0) {
                throw new ArgumentOutOfRangeException(nameof(initialCost1));
            }

            if (initialCost2 < 0) {
                throw new ArgumentOutOfRangeException(nameof(initialCost2));
            }

            if (sourceNode1 == sourceNode2) {
                Debug.LogWarning("Source node 2 equals to source node 1.");
                sourceNode2 = null;
                initialCost1 = Mathf.Min(initialCost1, initialCost2);
            }

            this.sourceNode1 = sourceNode1;
            this.sourceNode2 = sourceNode2;

            ClearNodeFlags();

            nodePQ.Enqueue(sourceNode1, (int)(initialCost1 * 100));
            if (sourceNode2 != null) {
                nodePQ.Enqueue(sourceNode2, (int)(initialCost2 * 100));
            }

            while (!nodePQ.Empty) {
                int priority;
                MapNode node = nodePQ.Dequeue(out priority);

                if (node.VisitFlag) {
                    continue;
                }

                node.VisitFlag = true;

                foreach (MapEdge edge in node.Edges) {
                    if (!edge.neighbor.VisitFlag) {
                        float newCost = node.GetEdgeCost(edge.neighbor) + priority / 100;
                        if (newCost < edge.neighbor.CostFlag) {
                            edge.neighbor.ParentFlag = node;
                            edge.neighbor.CostFlag = newCost;
                            nodePQ.Enqueue(edge.neighbor, (int)(newCost * 100));
                        }
                    }
                }
            }

            nodePQ.Clear();
        }

        public MapNode GetSourceNode(MapNode targetNode) {

            if (targetNode == null) {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (sourceNode1 == null) {
                throw new InvalidOperationException("Source node is null. You should call TagReachableNodes() before getSourceNode().");
            }

            while (targetNode != sourceNode1 && targetNode != sourceNode2) {
                targetNode = targetNode.ParentFlag;
                if (targetNode == null) {
                    throw new InvalidOperationException("Cannot get source node because target node is not reachable.");
                }
            }

            return targetNode;
        }

        public MapNode GetFirstNode(MapNode targetNode) {

            if (targetNode == null) {
                throw new ArgumentNullException(nameof(targetNode));
            }

            while (targetNode.ParentFlag != null) {
                if (targetNode.ParentFlag == sourceNode1 || targetNode.ParentFlag == sourceNode2) {
                    return targetNode;
                }
                targetNode = targetNode.ParentFlag;
            }

            throw new InvalidOperationException("Cannot get first node because target node is not reachable.");
        }

        /// <summary>
        /// 计算下一跳的节点。<br/>
        /// 使用堆优化的 dijkstra 算法。
        /// </summary>
        /// <param name="sourceNode">源节点</param>
        /// <param name="targetNode">目标节点</param>
        /// <returns>下一跳的节点</returns>
        public MapNode NextNode(MapNode sourceNode, MapNode targetNode) {

            if (sourceNode == null) {
                throw new ArgumentNullException(nameof(sourceNode));
            }

            if (targetNode == null) {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (sourceNode == targetNode) {
                return targetNode;
            }

            PriorityQueue<MapNode> nodePQ = new PriorityQueue<MapNode>(false);
            nodePQ.Enqueue(sourceNode, 0);

            while (!nodePQ.Empty) {
                int priority;
                MapNode node = nodePQ.Dequeue(out priority);

                if (node.VisitFlag) {
                    continue;
                }
                node.VisitFlag = true;

                foreach (MapEdge edge in node.Edges) {
                    if (!edge.neighbor.VisitFlag) {
                        float newCost = node.GetEdgeCost(edge.neighbor) + priority / 100;
                        if (newCost < edge.neighbor.CostFlag) {
                            edge.neighbor.ParentFlag = node;
                            edge.neighbor.CostFlag = newCost;
                            nodePQ.Enqueue(edge.neighbor, (int)(newCost * 100));
                        }
                    }
                }
            }

            while (targetNode.ParentFlag != sourceNode) {
                targetNode = targetNode.ParentFlag;
                if (targetNode == null) {
                    throw new Exception("Cannot get next node because target node is not reachable.");
                }
            }

            ClearNodeFlags();

            return targetNode;
        }

        /// <summary>
        /// 计算下一跳的节点。<br/>
        /// 使用堆优化的 dijkstra 算法。
        /// </summary>
        /// <param name="sourceNode">源节点</param>
        /// <param name="targetAgent">目标 agent</param>
        /// <returns>下一跳的节点</returns>
        public MapNode NextNode(MapNode sourceNode, MapNavAgent targetAgent) {

            if (targetAgent == null) {
                throw new ArgumentNullException(nameof(targetAgent));
            }

            if (sourceNode == null) {
                throw new ArgumentNullException(nameof(sourceNode));
            }

            // 在一条边上
            if (sourceNode == targetAgent.CurrentNode) {
                return targetAgent.NextNode;
            }
            if (sourceNode == targetAgent.NextNode && targetAgent.BackwardCostPerUnit != float.PositiveInfinity) {
                return targetAgent.CurrentNode;
            }

            PriorityQueue<MapNode> nodePQ = new PriorityQueue<MapNode>(false);
            nodePQ.Enqueue(sourceNode, 0);

            while (!nodePQ.Empty) {
                int priority;
                MapNode node = nodePQ.Dequeue(out priority);

                if (node.VisitFlag) {
                    continue;
                }
                node.VisitFlag = true;

                foreach (MapEdge edge in node.Edges) {
                    if (!edge.neighbor.VisitFlag) {
                        float newCost = node.GetEdgeCost(edge.neighbor) + priority / 100;
                        if (newCost < edge.neighbor.CostFlag) {
                            edge.neighbor.ParentFlag = node;
                            edge.neighbor.CostFlag = newCost;
                            nodePQ.Enqueue(edge.neighbor, (int)(newCost * 100));
                        }
                    }
                }
            }

            float nextNodeCost = targetAgent.NextNode.CostFlag + targetAgent.NextNodeToAgentCost;
            float currentNodeCost = targetAgent.CurrentNode.CostFlag + targetAgent.CurrentNodeToAgentCost;
            MapNode targetNode = nextNodeCost < currentNodeCost ? targetAgent.NextNode : targetAgent.CurrentNode;

            while (targetNode.ParentFlag != sourceNode) {
                targetNode = targetNode.ParentFlag;
                if (targetNode == null) {
                    throw new Exception("Cannot get next node because target node is not reachable.");
                }
            }

            ClearNodeFlags();

            return targetNode;
        }

        /// <summary>
        /// 计算下一跳的节点。<br/>
        /// 使用堆优化的 dijkstra 算法。
        /// </summary>
        /// <param name="sourceAgent">源 agent</param>
        /// <param name="targetNode">目标节点</param>
        /// <returns>下一跳的节点</returns>
        public MapNode NextNode(MapNavAgent sourceAgent, MapNode targetNode) {

            if (targetNode == null) {
                throw new ArgumentNullException(nameof(targetNode));
            }

            if (sourceAgent == null) {
                throw new ArgumentNullException(nameof(sourceAgent));
            }

            PriorityQueue<MapNode> nodePQ = new PriorityQueue<MapNode>(false);
            nodePQ.Enqueue(sourceAgent.CurrentNode, (int)(sourceAgent.AgentToCurrentNodeCost * 100));
            nodePQ.Enqueue(sourceAgent.NextNode, (int)(sourceAgent.AgentToNextNodeCost * 100));

            while (!nodePQ.Empty) {
                int priority;
                MapNode node = nodePQ.Dequeue(out priority);

                if (node.VisitFlag) {
                    continue;
                }
                node.VisitFlag = true;

                foreach (MapEdge edge in node.Edges) {
                    if (!edge.neighbor.VisitFlag) {
                        float newCost = node.GetEdgeCost(edge.neighbor) + priority / 100;
                        if (newCost < edge.neighbor.CostFlag) {
                            edge.neighbor.ParentFlag = node;
                            edge.neighbor.CostFlag = newCost;
                            nodePQ.Enqueue(edge.neighbor, (int)(newCost * 100));
                        }
                    }
                }
            }

            while (targetNode != sourceAgent.CurrentNode && targetNode != sourceAgent.NextNode) {
                targetNode = targetNode.ParentFlag;
                if (targetNode == null) {
                    throw new Exception("Cannot get next node because target node is not reachable.");
                }
            }

            ClearNodeFlags();

            return targetNode;
        }

        /// <summary>
        /// 计算下一跳的节点。<br/>
        /// 使用堆优化的 dijkstra 算法。
        /// </summary>
        /// <param name="sourceAgent">源 agent</param>
        /// <param name="targetAgent">目标 agent</param>
        /// <returns>下一跳的节点</returns>
        public MapNode NextNode(MapNavAgent sourceAgent, MapNavAgent targetAgent) {

            if (targetAgent == null) {
                throw new ArgumentNullException(nameof(targetAgent));
            }

            if (sourceAgent == null) {
                throw new ArgumentNullException(nameof(sourceAgent));
            }

            if (sourceAgent == targetAgent) {
                throw new InvalidOperationException("An agent cannot chase itself (unless it's a dog).");
            }

            // 在同一条边上
            if (sourceAgent.CurrentNode == targetAgent.CurrentNode && sourceAgent.NextNode == targetAgent.NextNode) {
                if (sourceAgent.CurrentNodeToAgentCost < targetAgent.CurrentNodeToAgentCost) {
                    return sourceAgent.NextNode;
                } else if (sourceAgent.BackwardCostPerUnit != float.PositiveInfinity) {
                    return sourceAgent.CurrentNode;
                }
            }
            if (sourceAgent.CurrentNode == targetAgent.NextNode && sourceAgent.NextNode == targetAgent.CurrentNode) {
                if (sourceAgent.CurrentNodeToAgentCost < targetAgent.AgentToNextNodeCost) {
                    return sourceAgent.NextNode;
                } else if (sourceAgent.BackwardCostPerUnit != float.PositiveInfinity) {
                    return sourceAgent.CurrentNode;
                }
            }

            PriorityQueue<MapNode> nodePQ = new PriorityQueue<MapNode>(false);
            nodePQ.Enqueue(sourceAgent.CurrentNode, (int)(sourceAgent.AgentToCurrentNodeCost * 100));
            nodePQ.Enqueue(sourceAgent.NextNode, (int)(sourceAgent.AgentToNextNodeCost * 100));

            while (!nodePQ.Empty) {
                int priority;
                MapNode node = nodePQ.Dequeue(out priority);

                if (node.VisitFlag) {
                    continue;
                }
                node.VisitFlag = true;

                foreach (MapEdge edge in node.Edges) {
                    if (!edge.neighbor.VisitFlag) {
                        float newCost = node.GetEdgeCost(edge.neighbor) + priority / 100;
                        if (newCost < edge.neighbor.CostFlag) {
                            edge.neighbor.ParentFlag = node;
                            edge.neighbor.CostFlag = newCost;
                            nodePQ.Enqueue(edge.neighbor, (int)(newCost * 100));
                        }
                    }
                }
            }

            float nextNodeCost = targetAgent.NextNode.CostFlag + targetAgent.NextNodeToAgentCost;
            float currentNodeCost = targetAgent.CurrentNode.CostFlag + targetAgent.CurrentNodeToAgentCost;
            MapNode targetNode = nextNodeCost < currentNodeCost ? targetAgent.NextNode : targetAgent.CurrentNode;

            while (targetNode != sourceAgent.CurrentNode && targetNode != sourceAgent.NextNode) {
                targetNode = targetNode.ParentFlag;
                if (targetNode == null) {
                    throw new Exception("Cannot get next node because target agent is not reachable.");
                }
            }

            ClearNodeFlags();

            return targetNode;
        }

        public MapNode GetNearestNode(MapNavAgent agent) {
            MapNode nearestNode = null;
            float minDist = float.PositiveInfinity;
            foreach (MapNode node in nodes) {
                float dist = agent.DistanceToNode(node);
                if (dist < minDist) {
                    nearestNode = node;
                    minDist = dist;
                }
            }
            return nearestNode;
        }

        public void ClearNodeFlags() {
            foreach (MapNode node in nodes) {
                node.ClearFlags();
            }
        }

        public List<MapNode> Nodes {
            get { return nodes; }
        }
    }
}