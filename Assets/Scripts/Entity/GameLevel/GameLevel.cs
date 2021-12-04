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
    public class GameLevel : MonoBehaviour {

        [SerializeField]
        private Player player;

        [SerializeField]
        private Enemy[] enemys;

        [SerializeField]
        private float wanderingTime;

        [SerializeField]
        private float chasingTime;

        [SerializeField]
        private float escapeTime;

        private Fsm<GameLevel> fsm;

        private void Awake() {
            if (Game.Instance != null) {
                Game.Instance.CurrentLevel = this;
            }

            foreach (Enemy enemy in enemys) {
                enemy.TargetAgent = player.GetComponent<MapNavAgent>();
            }
        }

        private void Start() {
            fsm = new Fsm<GameLevel>(this, new LevelRelaxState(), new LevelHardState());
            fsm.Start<LevelRelaxState>();
        }

        private void Update() {
            fsm.Update();
        }

        public void EnemyEscape() {
            foreach (Enemy enemy in enemys) {
                enemy.State = Enemy.EnemyState.Escaping;
            }
        }

        public void EnemyWander() {
            foreach (Enemy enemy in enemys) {
                enemy.State = Enemy.EnemyState.Wandering;
            }
        }

        public void EnemyChase() {
            foreach (Enemy enemy in enemys) {
                enemy.State = Enemy.EnemyState.Chasing;
            }
        }

        public float WanderingTime {
            get { return wanderingTime; }
        }

        public float ChasingTime {
            get { return chasingTime; }
        }

        public float EscapeTime {
            get { return escapeTime; }
        }
    }
}
