using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace WaterProstecting
{
    public class PauseManager : MonoBehaviour
    {
        [Header("UI")]
        public GameObject pausePanel;

        private bool isPaused = false;

        void Start()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }

        void Update()
        {
            //Проверка Esc
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                //(Победа/Поражение) - Esc ничего не делает
                if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
                {
                    return; 
                }

                //Игра идет нормально - вкл/выкл паузы
                TogglePause();
            }
        }

        public void TogglePause()
        {
            //Нельзя поставить паузу, если игра уже кончилась
            if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
            {
                return;
            }

            if (isPaused) ResumeGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            isPaused = true;
            if (pausePanel != null) pausePanel.SetActive(true);
            
            //Остановка времени
            Time.timeScale = 0f; 
        }

        public void ResumeGame()
        {
            isPaused = false;
            if (pausePanel != null) pausePanel.SetActive(false);
            
            //Возобновление времени
            Time.timeScale = 1f; 
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu"); 
        }
    }
}