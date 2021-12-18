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

namespace PacKaf {
    public class LevelPlayingState : FsmState<GameLevel> {
        public override void OnUpdate(Fsm<GameLevel> fsm) {
            int activeCount = 0;
            foreach (GameObject pickableItem in fsm.Owner.PickableItems) {
                if (pickableItem.activeInHierarchy) {
                    activeCount++;
                }
            }
            if (activeCount == 0) {
                fsm.Owner.UI.ShowBoardWin();
                fsm.ChangeState<LevelEndState>();
            }

            base.OnUpdate(fsm);
        }
    }
}
