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

        public void StartLevel(int levelIndex) {
            currentLevelIndex = levelIndex;
            StartCoroutine(LoadLevel(levelIndex));
        }

        public void OpenMenu() {
            StartCoroutine(LoadMenu());
        }

        private IEnumerator LoadMenu() {
            currentLevelIndex = -1;

            while (!SceneFadeOut(0.8f)) {
                yield return null;
            }

            gameFsm.ChangeState<MenuState>();

            while (!SceneFadeIn(0.8f)) {
                yield return null;
            }

            yield break;
        }

        private IEnumerator LoadLevel(int levelIndex) {
            while (!SceneFadeOut(0.8f)) {
                yield return null;
            }

            if (levelIndex < 0 || levelIndex >= LevelSceneNames.Length) {
                throw new System.IndexOutOfRangeException("Level Index Out Of Range.");
            }

            LevelSceneName = LevelSceneNames[levelIndex];
            gameFsm.ChangeState<PlayingState>();

            while (!SceneFadeIn(0.8f)) {
                yield return null;
            }

            yield break;
        }

        private bool SceneFadeOut(float fadeTime) {
            if (faderAlpha > 1) {
                return true;
            }

            faderAlpha += Time.deltaTime / fadeTime;
            faderImage.color = new Color(0, 0, 0, Mathf.Clamp(faderAlpha, 0, 1));

            return false;
        }

        private bool SceneFadeIn(float fadeTime) {
            if (faderAlpha < 0) {
                return true;
            }

            faderAlpha -= Time.deltaTime / fadeTime;
            faderImage.color = new Color(0, 0, 0, Mathf.Clamp(faderAlpha, 0, 1));

            return false;
        }

        public GameLevel CurrentLevel { get; set; }
        public int Score { get; set; }
        public string LevelSceneName { get; set; }
    }
}