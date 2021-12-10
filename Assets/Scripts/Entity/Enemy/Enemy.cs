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
    [RequireComponent(typeof(MapNavAgent), typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour {

        public enum ChaseMode {
            Late, Early, Predict, Shy
        }

        [SerializeField]
        private ChaseMode chaseMode;

        [SerializeField]
        private float escapeTime;

        [SerializeField]
        private int respawnNodeIndex;

        public bool Caught { get; set; }
        public MapNavAgent NavAgent { get; private set; }
        public MapNavAgent TargetAgent { get; set; }

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

            fsm = new Fsm<Enemy>(this, new EnemyEscapeState(), new EnemyWanderingState(), new EnemyRespawnState(), new EnemyStopState(), GenerateChasingState());
            fsm.Start<EnemyChasingState>();
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
            if (collider.tag == "Player") {
                Caught = true;
            }
        }

        private EnemyChasingState GenerateChasingState() {
            switch (chaseMode) {
                case ChaseMode.Late: return new EnemyLateChasingState();
                case ChaseMode.Early: return new EnemyEarlyChasingState();
                case ChaseMode.Predict: return new EnemyPredictiveChasingState();
                case ChaseMode.Shy: return new EnemyShyChasingState();
                default: throw new System.NotImplementedException("Not implemented chase mode: " + chaseMode.ToString());
            }
        }

        public Rigidbody2D Rigidbody { get; set; }

        public float EscapeTime {
            get { return escapeTime; }
        }

        public Vector2 Position {
            get { return transform.position; }
        }

        public int RespawnNodeIndex {
            get { return respawnNodeIndex; }
        }
    }
}