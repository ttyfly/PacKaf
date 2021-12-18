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
using UnityEngine.UI;

namespace PacKaf {
    public class GameUI : MonoBehaviour {

        private Text inGameScoreText;
        private Text scoreText;
        private Text winText;
        private Text lossText;
        private Button backButton;
        private Button nextButton;
        private Image panel;

        private void Start() {
            inGameScoreText = transform.Find("Canvas Layer 0/Ingame Score Text").GetComponent<Text>();
            scoreText = transform.Find("Canvas Layer 2/Score Text").GetComponent<Text>();
            winText = transform.Find("Canvas Layer 2/Win Text").GetComponent<Text>();
            lossText = transform.Find("Canvas Layer 2/Loss Text").GetComponent<Text>();
            backButton = transform.Find("Canvas Layer 2/Back Button").GetComponent<Button>();
            nextButton = transform.Find("Canvas Layer 2/Next Button").GetComponent<Button>();
            panel = transform.Find("Canvas Layer 1/Panel").GetComponent<Image>();

            scoreText.gameObject.SetActive(false);
            winText.gameObject.SetActive(false);
            lossText.gameObject.SetActive(false);
            backButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
            panel.gameObject.SetActive(false);
        }

        private void Update() {
            inGameScoreText.text = string.Format("Score: {0:000000}", Game.Instance.Score);
        }

        public void ShowBoardWin() {
            nextButton.interactable = Game.Instance.HasNextLevel();
            scoreText.text = string.Format("Score: {0:000000}", Game.Instance.Score);

            scoreText.gameObject.SetActive(true);
            winText.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            panel.gameObject.SetActive(true);
        }

        public void ShowBoardLoss() {
            nextButton.interactable = false;
            scoreText.text = string.Format("Score: {0:000000}", Game.Instance.Score);

            scoreText.gameObject.SetActive(true);
            lossText.gameObject.SetActive(true);
            backButton.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(true);
            panel.gameObject.SetActive(true);
        }

        public void OnNextButtonClick() {
            Game.Instance.StartNextLevel();
        }

        public void OnBackButtonClick() {
            Game.Instance.OpenMenu();
        }
    }
}
