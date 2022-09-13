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
    public class EnemyShyChasingState : EnemyChasingState {

        private float minDistance = 3;
        private float maxDistance = 5;

        public override void OnEnter() {
            base.OnEnter();
            fsm.Owner.NavAgent.SetTarget(fsm.Owner.TargetAgent);
        }

        public override void OnUpdate() {
            MapNavAgent agent = fsm.Owner.NavAgent;
            MapNavAgent target = fsm.Owner.TargetAgent;

            float distance = agent.DistanceToAgent(fsm.Owner.TargetAgent);

            if (distance > maxDistance) {
                fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Chasing;
            } else if (distance < minDistance) {
                fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Escaping;
            }

            base.OnUpdate();
        }
    }
}
