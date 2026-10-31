using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace WaterProstecting
{
    public class PauseManager : MonoBehaviour
    {
        [Header("UI")]
        public GameObject pausePanel;
        public GameObject settingsPanel;
        public Slider volumeSlider;

        private bool isPaused = false;

        void Start()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false); //Без настроек при старте
            Time.timeScale = 1f;

            //Настройка громкости
            if (volumeSlider != null)
            {
                volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
        }

        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                //Настройки открыты, Esc их закрывает
                if (settingsPanel != null && settingsPanel.activeSelf)
                {
                    CloseSettings();
                    return;
                }

                if (GameManager.Instance != null && GameManager.Instance.IsGameEnded)
                {
                    return; 
                }

                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (GameManager.Instance != null && GameManager.Instance.IsGameEnded) return;

            if (isPaused) ResumeGame();
            else PauseGame();
        }

        public void PauseGame()
        {
            isPaused = true;
            if (pausePanel != null) pausePanel.SetActive(true);
            Time.timeScale = 0f; 
        }

        public void ResumeGame()
        {
            isPaused = false;
            if (pausePanel != null) pausePanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false); //Закрытие настроек при выходе из паузы
            Time.timeScale = 1f; 
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu"); 
        }

        //Методы для настроек
        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
            //Панель паузы можно скрыть
            if (pausePanel != null) pausePanel.SetActive(false); 
        }

        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
            //Возвращение панели паузы
            if (pausePanel != null) pausePanel.SetActive(true); 
        }

        public void SetVolume(float volume)
        {
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);
        }
    }
}