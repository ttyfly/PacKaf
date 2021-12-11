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
    public class EnemyRespawnState : FsmState<Enemy> {
        public override void OnEnter(Fsm<Enemy> fsm) {
            base.OnEnter(fsm);
            // fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Chasing;
            // fsm.Owner.NavAgent.SetTarget(fsm.Owner.RespawnNodeIndex);
            fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Passive;
            fsm.Owner.Rigidbody.gravityScale = 1;
        }

        public override void OnUpdate(Fsm<Enemy> fsm) {
            base.OnUpdate(fsm);

            // if (fsm.Owner.NavAgent.IsOnTargetNode) {
            //     fsm.ChangeState<EnemyStopState>();
            // }

            if (TimeSinceEnter > 3) {
                fsm.ChangeState<EnemyStopState>();
            }
        }

        public override void OnLeave(Fsm<Enemy> fsm) {
            fsm.Owner.Caught = false;
            fsm.Owner.Rigidbody.gravityScale = 0;
            fsm.Owner.Rigidbody.velocity = UnityEngine.Vector2.zero;
            fsm.Owner.transform.position = fsm.Owner.RespawnPoint;
            base.OnLeave(fsm);
        }
    }
}
