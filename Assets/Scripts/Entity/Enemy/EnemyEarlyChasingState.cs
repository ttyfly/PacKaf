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
    public class EnemyEarlyChasingState : EnemyChasingState {

        private MapNode intermediateNode;
        private bool afterIntermediateNode;

        public override void OnEnter(Fsm<Enemy> fsm){
            base.OnEnter(fsm);

            intermediateNode = fsm.Owner.TargetAgent.NextNode;
            afterIntermediateNode = false;
            fsm.Owner.NavAgent.SetTarget(intermediateNode);
        }

        public override void OnUpdate(Fsm<Enemy> fsm) {
            base.OnUpdate(fsm);

            if (fsm.Owner.TargetAgent.NextNode != intermediateNode) {
                intermediateNode = fsm.Owner.TargetAgent.NextNode;
                afterIntermediateNode = false;
                fsm.Owner.NavAgent.SetTarget(intermediateNode);
            }

            if (!afterIntermediateNode && fsm.Owner.NavAgent.IsOnTargetNode) {
                afterIntermediateNode = true;
                fsm.Owner.NavAgent.SetTarget(fsm.Owner.TargetAgent);
            }
        }
    }
}
