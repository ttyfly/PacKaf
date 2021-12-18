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
    public class EnemyChasingState : FsmState<Enemy> {

        public override void OnEnter(Fsm<Enemy> fsm) {
            base.OnEnter(fsm);
            fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Chasing;
        }

        public override void OnUpdate(Fsm<Enemy> fsm) {
            base.OnUpdate(fsm);

            if (fsm.Owner.Caught) {
                Game.Instance.CurrentLevel.LevelFail();
            }

            switch (Game.Instance.CurrentLevel.State) {
                case GameLevel.LevelState.Escaping: fsm.ChangeState<EnemyEscapeState>(); break;
                case GameLevel.LevelState.Wandering: fsm.ChangeState<EnemyWanderingState>(); break;
                case GameLevel.LevelState.End: fsm.ChangeState<EnemyStopState>(); break;
            }
        }
    }
}
