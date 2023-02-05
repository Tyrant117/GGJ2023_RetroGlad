using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RetroGlad
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField]
        private Button _continueButton;
        [SerializeField]
        private Button _newGameButton;
        [SerializeField]
        private Button _exitButton;

        private CompanyManager _company;

        private void Awake()
        {
            _company = CompanyManager.Load();
            if (_company.Squad.Count == 0)
            {
                _continueButton.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _continueButton.onClick.AddListener(Continue);
            _newGameButton.onClick.AddListener(NewGame);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void OnDisable()
        {
            _continueButton.onClick.RemoveAllListeners();
            _newGameButton.onClick.RemoveAllListeners();
            _exitButton.onClick.RemoveAllListeners();
        }

        private void Continue()
        {
            SceneManager.sceneLoaded += OnContinueLoaded;
            SceneManager.LoadScene("Level 1");
        }

        private void OnContinueLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnContinueLoaded;
        }

        private void NewGame()
        {
            _company = CompanyManager.New();
            SceneManager.sceneLoaded += OnMercSceneLoaded;
            SceneManager.LoadScene("Mercenary Buy Scene");
        }

        private void OnMercSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnMercSceneLoaded;
            var ui = FindObjectOfType<MercenaryBuyUI>();
            ui.LoadNew(_company);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}
