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
        private void OnSceneGUI() {
            MapNav2D map = (MapNav2D)target;

            Handles.color = Color.red;
            if (map.Nodes != null && map.Nodes.Count > 0) {
                for (int i = 0; i < map.Nodes.Count; i++) {
                    MapNode node = map.Nodes[i];
                    foreach (MapEdge edge in node.Edges) {
                        Handles.DrawLine(node.Position, edge.neighbor.Position);
                        Handles.DrawSolidArc(
                            edge.neighbor.Position,
                            Vector3.back,
                            node.Position - edge.neighbor.Position,
                            10,
                            HandleUtility.GetHandleSize(edge.neighbor.Position) * 0.2f
                        );
                        Handles.DrawSolidArc(
                            edge.neighbor.Position,
                            Vector3.back,
                            node.Position - edge.neighbor.Position,
                            -10,
                            HandleUtility.GetHandleSize(edge.neighbor.Position) * 0.2f
                        );
                    }

                    Handles.Label(node.Position, i.ToString());

                    EditorGUI.BeginChangeCheck();

                    Vector2 newPosition = Handles.FreeMoveHandle(
                        node.Position,
                        Quaternion.identity,
                        HandleUtility.GetHandleSize(node.Position) * 0.2f,
                        Vector3.one * 0.5f,
                        Handles.CircleHandleCap
                    );

                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(map, "Node Position Change");
                        node.Position = newPosition;
                    }
                }
            }
        }
    }
}