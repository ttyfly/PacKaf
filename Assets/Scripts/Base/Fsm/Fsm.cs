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

using System;
using UnityEngine;

namespace PacKaf {
    public sealed class Fsm<T> {

        private FsmState<T>[] states;

        /// <summary>
        /// Finite State Machine.
        /// </summary>
        /// <param name="owner">Owner of this fsm</param>
        /// <param name="states">States the fsm contains</param>
        public Fsm(T owner, params FsmState<T>[] states) {

            if (states == null || states.Length == 0) {
                Debug.LogWarningFormat("Instantiate fsm failed: Fsm 'Fsm<{0}>' needs at least one state.", typeof(T).Name);
            }

            Owner = owner;
            this.states = states;

            foreach (FsmState<T> state in states) {
                state.OnInit(this);
            }
        }

        /// <summary>
        /// Start the fsm.
        /// </summary>
        /// <typeparam name="U">Initial state</typeparam>
        public void Start<U>() where U: FsmState<T> {
            foreach (FsmState<T> state in states) {
                if (state is U) {
                    CurrentState = state;
                    CurrentState.OnEnter();
                    return;
                }
            }
            Debug.LogErrorFormat("Start fsm failed: Fsm 'Fsm<{0}>' doesn't contain state '{1}'.", typeof(T).Name, typeof(U).Name);
        }

        /// <summary>
        /// Start the fsm.
        /// </summary>
        /// <param name="stateType">Type of the initial state</param>
        public void Start(Type stateType) {
            foreach (FsmState<T> state in states) {
                if (stateType.IsInstanceOfType(state)) {
                    CurrentState = state;
                    CurrentState.OnEnter();
                    return;
                }
            }
            Debug.LogErrorFormat("Start fsm failed: Fsm 'Fsm<{0}>' doesn't contain state '{1}'.", typeof(T).Name, stateType.Name);
        }

        /// <summary>
        /// This function should be called each frame when the fsm is enabled.
        /// </summary>
        public void Update() {
            if (CurrentState == null) {
                Debug.LogErrorFormat("Update fsm failed: Fsm 'Fsm<{0}>' is not started.", typeof(T).Name);
                return;
            }
            CurrentState.OnUpdate();
        }

        /// <summary>
        /// Destroy the fsm.
        /// </summary>
        public void Destroy() {
            if (CurrentState != null) {
                CurrentState.OnLeave();
            }
            for (int i = 0; i < states.Length; i++) {
                states[i].OnDestroy();
                states[i] = null;
            }
            states = null;
        }

        /// <summary>
        /// Change the state of fsm.
        /// </summary>
        /// <typeparam name="U">New state</typeparam>
        public void ChangeState<U>() where U: FsmState<T> {
            if (CurrentState == null) {
                Debug.LogErrorFormat("Change state failed: Fsm 'Fsm<{0}>' is not started.", typeof(T).Name);
                return;
            }
            CurrentState.OnLeave();
            foreach (FsmState<T> state in states) {
                if (state is U) {
                    CurrentState = state;
                    CurrentState.OnEnter();
                    return;
                }
            }
            Debug.LogErrorFormat("Change state failed: Fsm 'Fsm<{0}>' doesn't contain state '{1}'.", typeof(T).Name, typeof(U).Name);
        }

        /// <summary>
        /// Change the state of fsm.
        /// </summary>
        /// <param name="stateType">Type of the new state</param>
        public void ChangeState(Type stateType) {
            if (CurrentState == null) {
                Debug.LogErrorFormat("Change state failed: Fsm 'Fsm<{0}>' is not started.", typeof(T).Name);
                return;
            }
            CurrentState.OnLeave();
            foreach (FsmState<T> state in states) {
                if (stateType.IsInstanceOfType(state)) {
                    CurrentState = state;
                    CurrentState.OnEnter();
                    return;
                }
            }
            Debug.LogErrorFormat("Change state failed: Fsm 'Fsm<{0}>' doesn't contain state '{1}'.", typeof(T).Name, stateType.Name);
        }

        /// <summary>
        /// Owner of fsm.
        /// </summary>
        /// <value>Owner</value>
        public T Owner { get; private set; }

        /// <summary>
        /// Current state of fsm.
        /// </summary>
        /// <value>Current state</value>
        public FsmState<T> CurrentState { get; private set; }
    }
}
