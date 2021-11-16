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
    public sealed class Fsm<T> {

        private FsmState<T>[] states;

        public Fsm(T owner, params FsmState<T>[] states) {

            if (states == null || states.Length == 0) {
                Debug.LogWarningFormat("Instantiate fsm failed: Fsm '{0}' needs at least one state.", this.GetType().Name);
            }

            Owner = owner;
            this.states = states;

            foreach (FsmState<T> state in states) {
                state.OnInit(this);
            }
        }

        public void Start<U>() where U: FsmState<T> {
            foreach (FsmState<T> state in states) {
                if (state is U) {
                    CurrentState = state;
                    CurrentState.OnEnter(this);
                    return;
                }
            }
            Debug.LogErrorFormat("Start fsm failed: Fsm '{0}' doesn't contain state '{1}'.", this.GetType().Name, typeof(U).Name);
        }

        public void Update() {
            CurrentState.OnUpdate(this);
        }

        public void Destroy() {
            if (CurrentState != null) {
                CurrentState.OnLeave(this);
            }
            foreach (FsmState<T> state in states) {
                state.OnDestroy(this);
            }
        }

        public void ChangeState<U>() where U: FsmState<T> {
            if (CurrentState == null) {
                Debug.LogErrorFormat("Change state failed: Fsm '{0}' is not started.", this.GetType().Name);
                return;
            }
            CurrentState.OnLeave(this);
            foreach (FsmState<T> state in states) {
                if (state is U) {
                    CurrentState = state;
                    CurrentState.OnEnter(this);
                    return;
                }
            }
            Debug.LogErrorFormat("Change state failed: Fsm '{0}' doesn't contain state '{1}'.", this.GetType().Name, typeof(U).Name);
        }

        public T Owner { get; private set; }
        public FsmState<T> CurrentState { get; private set; }
    }
}
