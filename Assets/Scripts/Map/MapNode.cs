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
using System.Collections.Generic;

namespace PacKaf {
    [System.Serializable]
    public class MapNode {

        [SerializeField]
        private Vector2 position;

        [SerializeField]
        private List<MapEdge> edges;

        public MapNode(Vector2 position) {
            this.position = position;
            edges = new List<MapEdge>();
        }

        public static float Distance(MapNode node1, MapNode node2) {
            return Vector2.Distance(node1.Position, node2.Position);
        }

        public bool HasNeighbor(MapNode node) {
            foreach (MapEdge edge in edges) {
                if (node == edge.neighbor) {
                    return true;
                }
            }
            return false;
        }

        public float GetEdgeCost(MapNode neighbor) {
            return GetEdgeCostPerUnit(neighbor) * Distance(neighbor, this);
        }

        public float GetEdgeCostPerUnit(MapNode neighbor) {
            foreach (MapEdge edge in edges) {
                if (edge.neighbor == neighbor) {
                    return edge.costPerUnit;
                }
            }
            throw new System.Exception("No such neighbor.");
        }

        public MapEdge GetNearestEdge(MapNavAgent agent, out float minDistance) {
            MapEdge nearestEdge = default(MapEdge);
            minDistance = float.PositiveInfinity;
            foreach (MapEdge edge in edges) {
                float dist = agent.DistanceToEdge(this, edge.neighbor);
                if (dist < minDistance) {
                    nearestEdge = edge;
                    minDistance = dist;
                }
            }
            return nearestEdge;
        }

        public MapEdge GetNearestEdge(MapNavAgent agent) {
            return GetNearestEdge(agent, out _);
        }

        public void Clear() {
            Edges.Clear();
            ClearFlags();
        }

        public void ClearFlags() {
            VisitFlag = false;
            ParentFlag = null;
            CostFlag = float.PositiveInfinity;
        }

        public Vector2 Position {
            get { return position; }
            set { position = value; }
        }

        public List<MapEdge> Edges {
            get { return edges; }
        }

        public bool VisitFlag { get; set; }
        public MapNode ParentFlag { get; set; }
        public float CostFlag { get; set; }
    }
}