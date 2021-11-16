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
    public static partial class Utility {
        public static class Assembly {

            private static Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

            public static Type GetType(string typeName) {

                if (string.IsNullOrEmpty(typeName)) {
                    throw new ArgumentNullException("typeName", "Type name is invalid.");
                }

                Type type = null;
                if (typeCache.TryGetValue(typeName, out type)) {
                    return type;
                }

                type = Type.GetType(typeName);
                if (type != null) {
                    typeCache[typeName] = type;
                    return type;
                }

                return null;
            }
        }
    }
}