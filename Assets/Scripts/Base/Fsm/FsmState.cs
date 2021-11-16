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
    public class FsmState<T> {

        public virtual void OnInit(Fsm<T> fsm) {}

        public virtual void OnEnter(Fsm<T> fsm) {
            EnterTime = Time.time;
        }
        public virtual void OnUpdate(Fsm<T> fsm) {}

        public virtual void OnLeave(Fsm<T> fsm) {}

        public virtual void OnDestroy(Fsm<T> fsm) {}

        protected float TimeSinceEnter {
            get { return Time.time - EnterTime; }
        }

        protected float EnterTime { get; private set; }
    }
}