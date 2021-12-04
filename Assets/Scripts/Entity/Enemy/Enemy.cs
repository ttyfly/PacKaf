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
    [RequireComponent(typeof(MapNavAgent), typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour {

        public enum EnemyState {
            Chasing, Escaping, Wandering
        }

        public enum ChaseMode {
            Late, Early, Predict, Shy
        }

        [SerializeField]
        private ChaseMode chaseMode;

        [SerializeField]
        private MapNavAgent targetAgent;

        [SerializeField]
        private float escapeTime;

        public EnemyState State { get; set; }
        public bool Caught { get; set; }
        public MapNavAgent NavAgent { get; private set; }

        private Fsm<Enemy> fsm;
        private bool moving = false;
        private float moveStartTime;
        private Vector2 prevPosition;

        private void Start() {
            NavAgent = GetComponent<MapNavAgent>();
            Rigidbody = GetComponent<Rigidbody2D>();

            if (TargetAgent == null) {
                Debug.LogWarningFormat("Target of enemy is null.");
            }

            EnemyChasingState chasingState = (EnemyChasingState)Activator.CreateInstance(ChasingStateType);
            fsm = new Fsm<Enemy>(this, new EnemyEscapeState(), new EnemyWanderingState(), chasingState);
            fsm.Start(ChasingStateType);
        }

        private void Update() {
            fsm.Update();

            if ((prevPosition - Position).magnitude > 0) {
                if (!moving) {
                    moving = true;
                    moveStartTime = Time.time;
                }
                float delta = Time.time - moveStartTime;
                transform.localScale = new Vector3(0.8f + 0.05f * Mathf.Sin(4 * delta), 0.8f - 0.1f * Mathf.Sin(4 * delta));
            } else if (moving) {
                moving = false;
                transform.localScale = new Vector3(0.8f, 0.8f);
            }

            prevPosition = Position;
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            if (collider.name == targetAgent.gameObject.name) {
                Caught = true;
            }
        }

        public Type ChasingStateType {
            get {
                switch (chaseMode) {
                    case ChaseMode.Late: return typeof(EnemyLateChasingState);
                    case ChaseMode.Early: return typeof(EnemyEarlyChasingState);
                    case ChaseMode.Predict: return typeof(EnemyPredictiveChasingState);
                    case ChaseMode.Shy: return typeof(EnemyShyChasingState);
                    default: throw new NotImplementedException("Not implemented chase mode: " + chaseMode.ToString());
                }
            }
        }

        public MapNavAgent TargetAgent {
            get { return targetAgent; }
            set { targetAgent = value; }
        }

        public Rigidbody2D Rigidbody { get; set; }

        public float EscapeTime {
            get { return escapeTime; }
        }

        public Vector2 Position {
            get { return transform.position; }
        }
    }
}