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

namespace PacKaf {
    public static partial class Constant {
        public static class EventID {
            public const int LevelStart = 100;
            public const int LevelFailed = 101;
            public const int LevelWin = 102;

            public const int PlayerPickCoin = 500;
            public const int PlayerEatCake = 501;
            public const int PlayerCatchEnemy = 502;

            public const int EnemyStartWandering = 600;
            public const int EnemyStartChasing = 601;
            public const int EnemyStartEscaping = 602;
        }
    }
}
