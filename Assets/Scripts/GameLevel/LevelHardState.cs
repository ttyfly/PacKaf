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
    public class LevelHardState : FsmState<GameLevel> {
        public override void OnEnter(Fsm<GameLevel> fsm) {
            base.OnEnter(fsm);
            fsm.Owner.EnemyChase();
            UnityEngine.Debug.Log("Hard");
        }

        public override void OnUpdate(Fsm<GameLevel> fsm) {
            base.OnUpdate(fsm);

            if (TimeSinceEnter > fsm.Owner.ChasingTime) {
                fsm.ChangeState<LevelRelaxState>();
            }
        }
    }
}
