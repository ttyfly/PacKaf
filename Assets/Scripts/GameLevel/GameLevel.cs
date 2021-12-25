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

using System.Linq;
using UnityEngine;

namespace PacKaf {
    public class GameLevel : MonoBehaviour {

        public enum LevelState {
            Chasing, Wandering, Escaping, End
        }

        [SerializeField]
        private float wanderingTime;

        [SerializeField]
        private float chasingTime;

        [SerializeField]
        private float escapeTime;

        private GameUI ui;
        private Player player;
        private Enemy[] enemys;
        private GameObject[] pickableItems;
        private Fsm<GameLevel> fsm;

        public LevelState State { get; set; }

        private void Awake() {
            // if (Game.Instance != null) {
            //     Game.Instance.CurrentLevel = this;
            // }

            ui = GameObject.Find("UI").GetComponent<GameUI>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
            enemys = GameObject.FindGameObjectsWithTag("Enemy").Select((GameObject obj) => obj.GetComponent<Enemy>()).ToArray<Enemy>();
            pickableItems = GameObject.FindGameObjectsWithTag("Coin").Concat<GameObject>(GameObject.FindGameObjectsWithTag("Cake")).ToArray<GameObject>();
            // pickableItems = GameObject.FindGameObjectsWithTag("Cake");

            foreach (Enemy enemy in enemys) {
                enemy.TargetAgent = player.GetComponent<MapNavAgent>();
            }
        }

        private void Start() {
            if (Game.Instance != null) {
                Game.Instance.CurrentLevel = this;
            }

            fsm = new Fsm<GameLevel>(this, new LevelChasingState(), new LevelEscapingState(), new LevelWanderingState(), new LevelEndState());
            fsm.Start<LevelWanderingState>();
        }

        private void Update() {
            fsm.Update();
        }

        public void EnterEscapeState() {
            fsm.ChangeState<LevelEscapingState>();
        }

        public void LevelFail() {
            player.Freeze();
            ui.ShowBoardLoss();
            fsm.ChangeState<LevelEndState>();
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

        public GameObject[] PickableItems {
            get { return pickableItems; }
        }

        public GameUI UI {
            get { return ui; }
        }
    }
}
