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
using UnityEditor;

namespace PacKaf.Editor {
    [CustomEditor(typeof(MapNav2D))]
    public class MapNav2DEditor : UnityEditor.Editor {

        private MapNode selectedNode = null;
        private int editState;

        private void OnSceneGUI() {
            MapNav2D map = (MapNav2D)target;

            MapNode clickedNode = null;

            if (map.Nodes != null && map.Nodes.Count > 0) {
                for (int i = 0; i < map.Nodes.Count; i++) {

                    MapNode node = map.Nodes[i];

                    node.Edges.RemoveAll((MapEdge edge) => edge.neighborIndex == -1);

                    foreach (MapEdge edge in node.Edges) {
                        Handles.color = Color.green;
                        Handles.DrawLine(node.Position, edge.neighbor.Position, 2);
                        Handles.DrawSolidArc(edge.neighbor.Position, Vector3.back, node.Position - edge.neighbor.Position, 10, map.NodeRadius);
                        Handles.DrawSolidArc(edge.neighbor.Position, Vector3.back, node.Position - edge.neighbor.Position, -10, map.NodeRadius);

                        Vector2 rel = (edge.neighbor.Position - node.Position).normalized * map.EdgeWidth / 2;
                        rel.Set(rel.y, -rel.x);
                        Handles.color = Color.red;
                        Handles.DrawLine(node.Position + rel, edge.neighbor.Position + rel, 2);
                        Handles.DrawLine(node.Position - rel, edge.neighbor.Position - rel, 2);
                    }

                    EditorGUI.BeginChangeCheck();

                    Handles.color = Color.red;
                    int id = EditorGUIUtility.GetControlID(FocusType.Passive);
                    Vector2 newPosition = Handles.FreeMoveHandle(id, node.Position, Quaternion.identity, map.NodeRadius, Vector3.one * 0.5f, Handles.CircleHandleCap);

                    if (id == EditorGUIUtility.hotControl) {
                        clickedNode = map.Nodes[i];
                        Handles.color = Color.yellow;
                    }

                    if (node == selectedNode) {
                        Handles.color = Color.yellow;
                    }

                    Handles.DrawWireDisc(newPosition, Vector3.back, map.NodeRadius, 2);

                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(map, "Map Node Position Change");
                        node.Position = newPosition;
                    }
                }

                for (int i = 0; i < map.Nodes.Count; i++) {
                    Handles.Label(map.Nodes[i].Position, i.ToString());
                }
            }

            Handles.BeginGUI();
            GUILayout.BeginVertical("Map Node Editor", "window", GUILayout.Width(200), GUILayout.Height(300));

            if (GUILayout.Button("Add Node")) {
                Undo.RecordObject(map, "Add Map Node");
                selectedNode = new MapNode(map.Nodes[map.Nodes.Count - 1].Position);
                map.Nodes.Add(selectedNode);
            }

            editState = GUILayout.SelectionGrid(editState, new[] {"Select Node", "Delete Node", "Add Neighbor", "Delete Neighbor"}, 2);

            if (editState == 0 && clickedNode != null) {
                selectedNode = clickedNode;

            } else if (editState == 1 && clickedNode != null) {

                Undo.RecordObject(map, "Delete Map Node");

                foreach (MapNode node in map.Nodes) {
                    node.Edges.RemoveAll((MapEdge edge) => edge.neighbor == clickedNode);
                }
                map.Nodes.Remove(clickedNode);

                editState = 0;
                selectedNode = null;
            }

            if (selectedNode != null) {
                bool addNeighbor = editState == 2 && clickedNode != null && clickedNode != selectedNode;
                if (addNeighbor && !selectedNode.HasNeighbor(clickedNode)) {
                    Undo.RecordObject(map, "Add Map Edge");
                    selectedNode.Edges.Add(new MapEdge() {
                        neighborIndex = GetNodeIndex(clickedNode),
                        neighbor = clickedNode,
                        costPerUnit = 1,
                    });
                }

                bool delNeighbor = editState == 3 && clickedNode != null;
                if (delNeighbor) {
                    Undo.RecordObject(map, "Delete Map Edge");
                    selectedNode.Edges.RemoveAll((MapEdge edge) => edge.neighbor == clickedNode);
                }

                for (int i = 0; i < selectedNode.Edges.Count; i++) {
                    MapEdge edge = selectedNode.Edges[i];
                    EditorGUILayout.Separator();
                    int neighborIndex = EditorGUILayout.IntField("Neighbor Index", edge.neighborIndex);
                    float costPerUnit = EditorGUILayout.FloatField("Cost Per Unit", edge.costPerUnit);
                    selectedNode.Edges[i] = new MapEdge() {
                        neighborIndex = neighborIndex,
                        neighbor = map.Nodes[neighborIndex],
                        costPerUnit = costPerUnit,
                    };
                }
            }
            GUILayout.EndVertical();
            Handles.EndGUI();
        }

        private int GetNodeIndex(MapNode node) {
            MapNav2D map = (MapNav2D)target;

            for (int i = 0; i < map.Nodes.Count; i++) {
                if (map.Nodes[i] == node) {
                    return i;
                }
            }

            return -1;
        }
    }
}