using UnityEngine;
using UnityEngine.SceneManagement; // Для загрузки сцен
using UnityEngine.Audio; // Для звука
using UnityEngine.UI;

namespace WaterProstecting
{
    public class MainMenu : MonoBehaviour
    {
        [Header("UI Элементы")]
        public GameObject settingsPanel;
        public Slider volumeSlider;

        void Start()
        {
            // Инициализация громкости (если используешь AudioMixer)
            if (volumeSlider != null)
            {
                volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
                volumeSlider.onValueChanged.AddListener(SetVolume);
            }
        }

        // Кнопка Играть
        public void PlayGame()
        {
            // Загружаем следующую сцену по индексу (игровую)
            SceneManager.LoadScene(1); 
        }

        // Кнопка Настройки (Открыть)
        public void OpenSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
        }

        // Кнопка Настройки (Закрыть)
        public void CloseSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        // Кнопка Выход
        public void QuitGame()
        {
            Debug.Log("Игра закрыта!");
            Application.Quit();
        }

        // Настройка звука
        public void SetVolume(float volume)
        {
            // AudioListener.volume меняет громкость всей игры
            AudioListener.volume = volume;
            PlayerPrefs.SetFloat("Volume", volume);
        }
    }
}