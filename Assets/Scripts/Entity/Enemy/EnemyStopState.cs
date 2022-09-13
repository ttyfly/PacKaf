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
    public class EnemyStopState : FsmState<Enemy> {
        public override void OnEnter() {
            base.OnEnter();
            fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Passive;
        }

        public override void OnUpdate() {
            base.OnUpdate();

            switch (Game.Instance.CurrentLevel.State) {
                case GameLevel.LevelState.Chasing: fsm.ChangeState<EnemyChasingState>(); break;
                case GameLevel.LevelState.Wandering: fsm.ChangeState<EnemyWanderingState>(); break;
                case GameLevel.LevelState.Escaping: fsm.ChangeState<EnemyEscapeState>(); break;
            }
        }

        public override void OnLeave() {
            fsm.Owner.Caught = false;
            base.OnLeave();
        }
    }
}
