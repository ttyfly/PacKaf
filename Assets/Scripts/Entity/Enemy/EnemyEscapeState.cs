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
    public class EnemyEscapeState : FsmState<Enemy> {

        private Sprite prevSprite;

        public override void OnEnter(Fsm<Enemy> fsm) {
            base.OnEnter(fsm);
            fsm.Owner.NavAgent.Mode = MapNavAgent.AgentMode.Escaping;
            prevSprite = fsm.Owner.SpriteRenderer.sprite;
            fsm.Owner.SpriteRenderer.sprite = fsm.Owner.AlertSprite;
        }

        public override void OnUpdate(Fsm<Enemy> fsm) {
            base.OnUpdate(fsm);

            if (fsm.Owner.Caught) {
                Game.Instance.Score += 100;
                fsm.ChangeState<EnemyRespawnState>();
            }

            switch (Game.Instance.CurrentLevel.State) {
                case GameLevel.LevelState.Wandering: fsm.ChangeState<EnemyWanderingState>(); break;
                case GameLevel.LevelState.Chasing: fsm.ChangeState<EnemyChasingState>(); break;
                case GameLevel.LevelState.End: fsm.ChangeState<EnemyStopState>(); break;
            }
        }

        public override void OnLeave(Fsm<Enemy> fsm) {
            fsm.Owner.SpriteRenderer.sprite = prevSprite;
            base.OnLeave(fsm);
        }
    }
}