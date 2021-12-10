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

namespace PacKaf {
    public class EnemyPredictiveChasingState : EnemyChasingState {

        private MapNode intermediateNode;
        private bool afterIntermediateNode;

        public override void OnEnter(Fsm<Enemy> fsm){
            base.OnEnter(fsm);

            intermediateNode = null;
            afterIntermediateNode = false;
        }

        public override void OnUpdate(Fsm<Enemy> fsm) {
            base.OnUpdate(fsm);

            if (fsm.Owner.TargetAgent.NextNode != intermediateNode) {
                intermediateNode = GetIntermediateNode(fsm.Owner.TargetAgent);
                afterIntermediateNode = false;
                fsm.Owner.NavAgent.SetTarget(intermediateNode);
            }

            if (!afterIntermediateNode && fsm.Owner.NavAgent.IsOnTargetNode) {
                afterIntermediateNode = true;
                fsm.Owner.NavAgent.SetTarget(fsm.Owner.TargetAgent);
            }
        }

        private MapNode GetIntermediateNode(MapNavAgent target) {
            MapNode node = target.NextNode;
            MapNode prevNode = target.CurrentNode;
            while (true) {
                MapNode newNode;
                if (node.Edges.Count == 2 && node.HasNeighbor(prevNode)) {
                    newNode = node.Edges[0].neighbor == prevNode ? node.Edges[1].neighbor : node.Edges[0].neighbor;
                } else if (node.Edges.Count == 1 && node.Edges[0].neighbor != prevNode) {
                    newNode = node.Edges[0].neighbor;
                } else {
                    break;
                }
                prevNode = node;
                node = newNode;
            }
            return node;
        }
    }
}
