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

        // public enum AgentControlMode {
        //     Passive,
        //     Goto,
        //     Chasing,
        //     Wandering,
        // }

        public enum AgentControlMode {
            Passive,
            Active,
        }

        // public enum AgentChaseMode {
        //     Late,
        //     Predictive,
        //     FurtherPredictive,
        //     Shy,
        // }

        // [SerializeField]
        // private AgentChaseMode chaseMode;

        [SerializeField]
        private AgentControlMode controlMode;

        [SerializeField]
        private float maxSpeed;

        [SerializeField]
        public MapNav2D map;

        private MapNavAgent targetAgent;
        private MapNode targetNode;

        // private MapNode intermediateNode;
        // private bool afterIntermediateNode;

        // 暂存随机数，用于保证路径一致性。
        // private int randNum;

        private void Start() {
            (MapNode nearestNode, MapEdge nearestEdge) = map.GetNearestEdge(this);
            SetNodes(nearestNode, nearestEdge.neighbor);
        }

        private void Update() {

            UpdateCosts();

            // if (controlMode == AgentControlMode.Passive) {
            //     UpdatePassive();
            // } else if (controlMode == AgentControlMode.Chasing && ChaseTarget != null) {

            //     if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
            //         if (chaseMode == AgentChaseMode.Late) {
            //             SetNodes(NextNode, map.NextNode(NextNode, ChaseTarget));
            //         } else if (chaseMode == AgentChaseMode.Predictive) {
            //             SetNodes(NextNode, map.NextNode(NextNode, ChaseTarget.NextNode));
            //         }
            //     }

            //     // 如果可以转向
            //     if (BackwardCostPerUnit != float.PositiveInfinity) {
            //         if (chaseMode == AgentChaseMode.Late && map.NextNode(this, ChaseTarget) != NextNode) {
            //             SetNodes(NextNode, CurrentNode);
            //         } else if (chaseMode == AgentChaseMode.Predictive && map.NextNode(this, ChaseTarget.NextNode) != NextNode) {
            //             SetNodes(NextNode, CurrentNode);
            //         }
            //     }

            //     transform.position = Vector2.MoveTowards(transform.position, NextNode.Position, maxSpeed * Time.deltaTime);

            // } else if (controlMode == AgentControlMode.Wandering) {

            // }

            if (controlMode == AgentControlMode.Passive) {

                // if (Vector2.Distance(transform.position, NextNode.Position) <= map.NodeRadius) {
                //     SetNodes(NextNode, NextNode.GetNearestEdge(this).neighbor);
                // } else if (Vector2.Distance(transform.position, CurrentNode.Position) <= map.NodeRadius) {
                //     SetNodes(CurrentNode, CurrentNode.GetNearestEdge(this).neighbor);
                // } else {
                //     float distance;
                //     MapNode nearestEdgeNode = CurrentNode.GetNearestEdge(this, out distance).neighbor;
                //     if (distance <= map.EdgeWidth / 2) {
                //         SetNodes(CurrentNode, nearestEdgeNode);
                //     } else {
                //         (MapNode nearestNode, MapEdge nearestEdge) = map.GetNearestEdge(this);
                //         SetNodes(nearestNode, nearestEdge.neighbor);
                //     }

                // }
                (MapNode nearestNode, MapEdge nearestEdge) = map.GetNearestEdge(this);
                SetNodes(nearestNode, nearestEdge.neighbor);

            } else if (controlMode == AgentControlMode.Active) {

                if (targetAgent != null) {

                    // 同向同边
                    if (CurrentNode == targetAgent.CurrentNode && NextNode == targetAgent.NextNode) {

                        // 检测转向
                        if (CurrentNodeToAgentCost > targetAgent.CurrentNodeToAgentCost && BackwardCostPerUnit != float.PositiveInfinity) {
                            SetNodes(NextNode, CurrentNode);
                        }

                    // 异向同边
                    } else if (CurrentNode == targetAgent.NextNode && NextNode == targetAgent.CurrentNode) {

                        // 检测转向
                        if (CurrentNodeToAgentCost > targetAgent.AgentToNextNodeCost && BackwardCostPerUnit != float.PositiveInfinity) {
                            SetNodes(NextNode, CurrentNode);
                        }

                    // 异边
                    } else {

                        // 到达 NextNode 后需获得新的 NextNode
                        if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
                            if (NextNode == targetAgent.CurrentNode) {
                                SetNodes(NextNode, targetAgent.NextNode);
                            } else if (NextNode == targetAgent.NextNode && targetAgent.BackwardCostPerUnit != float.PositiveInfinity) {
                                SetNodes(NextNode, targetAgent.CurrentNode);
                            } else {
                                map.TagReachableNodes(NextNode);
                                float cost1 = targetAgent.CurrentNode.CostFlag + targetAgent.CurrentNodeToAgentCost;
                                float cost2 = targetAgent.NextNode.CostFlag + targetAgent.NextNodeToAgentCost;
                                SetNodes(NextNode, map.GetFirstNode(cost2 < cost1 ? targetAgent.NextNode : targetAgent.CurrentNode));
                            }

                        // 如果可转向，需检测转向后是否有更短的路径
                        } else if (BackwardCostPerUnit != float.PositiveInfinity) {
                            map.TagReachableNodes(CurrentNode, AgentToCurrentNodeCost, NextNode, AgentToNextNodeCost);
                            float cost1 = targetAgent.CurrentNode.CostFlag + targetAgent.CurrentNodeToAgentCost;
                            float cost2 = targetAgent.NextNode.CostFlag + targetAgent.NextNodeToAgentCost;
                            if (NextNode != map.GetSourceNode(cost2 < cost1 ? targetAgent.NextNode : targetAgent.CurrentNode)) {
                                SetNodes(NextNode, CurrentNode);
                            }
                        }
                    }

                } else if (targetNode != null) {

                    IsOnTargetNode = false;

                    // 到达 NextNode 后需获得新的 NextNode
                    if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
                        if (NextNode == targetNode) {
                            IsOnTargetNode = true;
                        } else {
                            map.TagReachableNodes(NextNode);
                            SetNodes(NextNode, map.GetFirstNode(targetNode));
                        }

                    // 如果可转向，需检测转向后是否有更短的路径
                    } else if (BackwardCostPerUnit != float.PositiveInfinity) {
                        map.TagReachableNodes(CurrentNode, AgentToCurrentNodeCost, NextNode, AgentToNextNodeCost);
                        if (NextNode != map.GetSourceNode(targetNode)) {
                            SetNodes(NextNode, CurrentNode);
                        }
                    }
                }

                // 步进
                transform.position = Vector2.MoveTowards(transform.position, NextNode.Position, maxSpeed * Time.deltaTime);

            } else {
                throw new System.NotImplementedException("Not implemented agent control mode: " + controlMode.ToString());
            }
        }

        // private void UpdatePassive() {
        //     if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
        //         SetNodes(NextNode, NextNode.GetNearestEdge(this).neighbor);
        //     } else {
        //         SetNodes(CurrentNode, CurrentNode.GetNearestEdge(this).neighbor);
        //     }
        // }

        // private void UpdateChasing() {
        //     if (ChaseTarget == null) {
        //         return;
        //     }

        //     MapNode newIntermediateNode = GetIntermediateNode(ChaseTarget);
        //     if (newIntermediateNode != intermediateNode) {
        //         intermediateNode = newIntermediateNode;
        //         afterIntermediateNode = false;
        //     }

        //     // 到达 NextNode 后需获得新的 NextNode
        //     if (Vector2.Distance(transform.position, NextNode.Position) < 0.01) {
        //         if (NextNode == intermediateNode) {
        //             afterIntermediateNode = true;
        //         }
        //         map.TagReachableNodes(NextNode);
        //         SetNodes(NextNode, map.GetFirstNode(afterIntermediateNode ? intermediateNode : intermediateNode)); // ?

        //     // 如果可转向，需检测转向后是否有更短的路径
        //     } else if (BackwardCostPerUnit != float.PositiveInfinity) {
        //         map.TagReachableNodes(CurrentNode, AgentToCurrentNodeCost, NextNode, AgentToNextNodeCost);
        //         if (NextNode != map.GetSourceNode(intermediateNode)) {
        //             SetNodes(NextNode, CurrentNode);
        //         }
        //     }

        //     // 步进
        //     transform.position = Vector2.MoveTowards(transform.position, NextNode.Position, maxSpeed * Time.deltaTime);
        // }

        // private MapNode GetIntermediateNode(MapNavAgent target) {
        //     switch (chaseMode) {
        //         case AgentChaseMode.Late:
        //             return target.CurrentNode;
        //         case AgentChaseMode.Predictive:
        //             return target.NextNode;
        //         case AgentChaseMode.FurtherPredictive:
        //             MapNode node = target.NextNode;
        //             MapNode prevNode = target.CurrentNode;
        //             while (true) {
        //                 MapNode newNode;
        //                 if (node.Edges.Count == 2 && node.HasNeighbor(prevNode)) {
        //                     newNode = node.Edges[0].neighbor == prevNode ? node.Edges[1].neighbor : node.Edges[0].neighbor;
        //                 } else if (node.Edges.Count == 1 && node.Edges[0].neighbor != prevNode) {
        //                     newNode = node.Edges[0].neighbor;
        //                 } else {
        //                     break;
        //                 }
        //                 prevNode = node;
        //                 node = newNode;
        //             }
        //             return node;
        //         default:
        //             return null;
        //     }
        // }

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

        public float DistanceToAgent(MapNavAgent agent) {
            return Vector2.Distance(transform.position, agent.Position);
        }

        public void SetTarget(MapNavAgent target) {
            targetAgent = target;
            targetNode = null;
        }

        public void SetTarget(MapNode target) {
            targetNode = target;
            targetAgent = null;
        }

        public void GotoRandomNode() {
            SetTarget(map.Nodes[Random.Range(0, map.Nodes.Count)]);
        }

        public void EscapeFromAgent(MapNavAgent agent) {
            float maxScore = 0;
            MapNode targetNode = null;
            foreach (MapNode node in map.Nodes) {
                if (node == CurrentNode || node == NextNode) {
                    continue;
                }
                float score = agent.DistanceToNode(node) - this.DistanceToNode(node);
                if (score > maxScore) {
                    maxScore = score;
                    targetNode = node;
                }
            }
            SetTarget(targetNode);
        }

        public bool IsOnTargetNode { get; private set; }

        // public MapNavAgent ChaseTarget { get; set; }
        // public MapNavAgent TargetAgent { get; set; }
        // public MapNode TargetNode { get; set; }

        // public AgentChaseMode ChaseMode {
        //     get { return chaseMode; }
        //     set { chaseMode = value; }
        // }

        public AgentControlMode ControlMode {
            get { return controlMode; }
            set { controlMode = value; }
        }

        public Vector2 Position {
            get { return transform.position; }
        }

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