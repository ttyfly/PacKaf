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

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PacKaf {
    public static partial class Utility {
        public static class Scene {

            private static List<string> loadingScenes = new List<string>();
            private static List<string> loadedScenes = new List<string>();
            private static List<string> unloadingScenes = new List<string>();

            public static void LoadScene(string sceneName) {
                AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                loadingScenes.Add(sceneName);

                op.completed += delegate (AsyncOperation op) {
                    loadingScenes.Remove(sceneName);
                    loadedScenes.Add(sceneName);
                };
            }

            public static void UnloadScene(string sceneName) {
                if (!loadedScenes.Contains(sceneName)) {
                    Debug.LogWarningFormat("Unload scene failed: scene '{0}' is not loaded.", sceneName);
                }

                AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

                loadedScenes.Remove(sceneName);
                unloadingScenes.Add(sceneName);

                op.completed += delegate (AsyncOperation op) {
                    unloadingScenes.Remove(sceneName);
                };
            }

            public static void UnloadAll() {
                foreach (string sceneName in loadedScenes) {
                    AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

                    unloadingScenes.Add(sceneName);

                    op.completed += delegate (AsyncOperation op) {
                        unloadingScenes.Remove(sceneName);
                    };
                }
                loadedScenes.Clear();
            }

            public static bool IsLoaded(string sceneName) {
                return loadedScenes.Contains(sceneName);
            }

            public static bool IsLoading(string sceneName) {
                return loadingScenes.Contains(sceneName);
            }

            public static bool IsLoading() {
                return loadingScenes.Count != 0;
            }

            public static bool IsUnloading(string sceneName) {
                return unloadingScenes.Contains(sceneName);
            }

            public static bool IsUnloading() {
                return unloadingScenes.Count != 0;
            }
        }
    }
}