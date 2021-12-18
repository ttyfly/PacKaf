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
    [RequireComponent(typeof(MapNavAgent))]
    public class Player : MonoBehaviour {

        [SerializeField]
        private float moveSpeed = 2;
        private Fsm<Player> fsm;

        private void Start() {
            Rigidbody = GetComponent<Rigidbody2D>();
            Renderer = GetComponent<SpriteRenderer>();

            fsm = new Fsm<Player>(
                this,
                new PlayerIdleState(),
                new PlayerWalkDownState(),
                new PlayerWalkLeftState(),
                new PlayerWalkRightState(),
                new PlayerWalkUpState(),
                new PlayerFreezeState()
            );
            fsm.Start<PlayerIdleState>();
        }

        private void Update() {
            fsm.Update();
        }

        private void OnTriggerEnter2D(Collider2D collider) {
            if (collider.tag == "Cake") {
                Game.Instance.CurrentLevel.EnterEscapeState();
                Game.Instance.Score += 53;
                collider.gameObject.SetActive(false);
            } else if (collider.tag == "Coin") {
                Game.Instance.Score += 5;
                collider.gameObject.SetActive(false);
            }
        }

        public void Freeze() {
            fsm.ChangeState<PlayerFreezeState>();
        }

        public float MoveSpeed {
            get { return moveSpeed; }
        }

        public Rigidbody2D Rigidbody { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
    }
}