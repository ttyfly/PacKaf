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

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PacKaf {
    public class Game : MonoBehaviour {

        public static Game Instance;

        [SerializeField]
        private string[] LevelSceneNames;

        private int currentLevelIndex;
        private Image faderImage;
        private float faderAlpha;
        private Fsm<Game> gameFsm;

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            currentLevelIndex = -1;
            faderImage = GameObject.Find("Scene Fader Panel").GetComponent<Image>();
            faderImage.color = new Color(0, 0, 0, 0);
            faderAlpha = 0;

            gameFsm = new Fsm<Game>(this, new MenuState(), new PlayingState(), new SplashState());
            gameFsm.Start<SplashState>();
        }

        private void Update() {
            gameFsm.Update();
        }

        public void StartNextLevel() {
            StartLevel(currentLevelIndex + 1);
        }

        public bool HasNextLevel() {
            return currentLevelIndex < LevelSceneNames.Length - 1;
        }

        public void OpenMenu() {
            currentLevelIndex = -1;
            gameFsm.ChangeState<MenuState>();
        }

        public void StartLevel(int levelIndex) {
            if (levelIndex < 0 || levelIndex >= LevelSceneNames.Length) {
                throw new System.IndexOutOfRangeException("Level Index Out Of Range.");
            }

            currentLevelIndex = levelIndex;
            LevelSceneName = LevelSceneNames[levelIndex];
            gameFsm.ChangeState<PlayingState>();
        }

        public void ChangeScene(string sceneName) {
            StartCoroutine(ChangeSceneCo(sceneName));
        }

        public void Quit() {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        private IEnumerator ChangeSceneCo(string sceneName) {
            yield return StartCoroutine(ScreenFadeOutCo(0.5f));
            Utility.Scene.UnloadAll();
            Utility.Scene.LoadScene(sceneName);
            yield return StartCoroutine(ScreenFadeInCo(0.5f));
        }

        private IEnumerator ScreenFadeOutCo(float fadeTime) {
            while (faderAlpha <= 1) {
                faderAlpha += Time.deltaTime / fadeTime;
                faderImage.color = new Color(0, 0, 0, Mathf.Clamp(faderAlpha, 0, 1));
                yield return null;
            }
        }

        private IEnumerator ScreenFadeInCo(float fadeTime) {
            while (faderAlpha >= 0) {
                faderAlpha -= Time.deltaTime / fadeTime;
                faderImage.color = new Color(0, 0, 0, Mathf.Clamp(faderAlpha, 0, 1));
                yield return null;
            }
        }

        public GameLevel CurrentLevel { get; set; }
        public int Score { get; set; }
        public string LevelSceneName { get; set; }
    }
}