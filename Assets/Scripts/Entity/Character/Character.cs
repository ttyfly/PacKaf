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
    public class Character : MonoBehaviour {
        private Fsm<Character> fsm;
        private MapNavAgent navAgent;

        [SerializeField]
        private float speed;

        private void Start() {
            navAgent = GetComponent<MapNavAgent>();
            navAgent.ChaseTarget = GameObject.Find("kaf").GetComponent<MapNavAgent>();
        }

        private void Update() {
        }

        public MapNavAgent NavAgent {
            get { return navAgent; }
        }
    }
}