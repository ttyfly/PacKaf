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
using System.Collections.Generic;

namespace PacKaf {
    public static class EventManager {

        private static Dictionary<int, EventHandler> eventHandlerDict = new Dictionary<int, EventHandler>();

        public static void InvokeEvent(int eventID, object sender, EventArgs e) {
            if (eventHandlerDict.TryGetValue(eventID, out EventHandler handler)) {
                handler(sender, e);
            }
        }

        public static void AddListener(int eventID, EventHandler handler) {
            if (eventHandlerDict.TryGetValue(eventID, out EventHandler handlerChain)) {
                handlerChain += handler;
            } else {
                eventHandlerDict.Add(eventID, handler);
            }
        }

        public static void RemoveListener(int eventID, EventHandler handler) {
            if (eventHandlerDict.TryGetValue(eventID, out EventHandler handlerChain)) {
                handlerChain -= handler;
                if (handlerChain == null) {
                    eventHandlerDict.Remove(eventID);
                }
            }
        }
    }
}
