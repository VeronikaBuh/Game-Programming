using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

namespace WaterProstecting
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [Header("UI — текст")]
        public TextMeshProUGUI dewText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI messageText;

        [Header("UI — панели")]
        public GameObject winPanel;
        public GameObject losePanel;

        [Header("Таймер")]
        public float startTime = 60f;

        [Header("Отладка")]
        public bool showDebugLogs = true;

        private float currentTime;
        private bool gameEnded = false;
        public bool IsGameEnded { get { return gameEnded; } }
        private int dewCount = 0;
        private int totalDewCollected = 0;
        private int dewSpentOnFlowers = 0;

        private List<Flower> allFlowers = new List<Flower>();
        private int flowersWatered = 0;

        public int DewCount { get { return dewCount; } }
        public int TotalDewCollected { get { return totalDewCollected; } }
        public int FlowersWatered { get { return flowersWatered; } }
        public int TotalFlowers { get { return allFlowers.Count; } }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            currentTime = startTime;

            //Все цветы на сцене
            allFlowers.Clear();
            allFlowers.AddRange(FindObjectsByType<Flower>(FindObjectsSortMode.None));

            if (winPanel != null) winPanel.SetActive(false);
            if (losePanel != null) losePanel.SetActive(false);
            if (messageText != null) messageText.text = "";

            UpdateUI();

            if (showDebugLogs)
                Debug.Log("GameManager запущен. Цветов на сцене: " + allFlowers.Count);
        }

        void Update()
        {
            if (gameEnded) return;

            currentTime -= Time.deltaTime;

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                LoseGame();
            }

            UpdateUI();
        }

        //Росинки
        public void AddDew(int amount)
        {
            dewCount += amount;
            totalDewCollected += amount;
            UpdateUI();

            if (showDebugLogs)
                Debug.Log("Росинка +" + amount + " | Всего на руках: " + dewCount);
        }
        
        public bool SpendDew(int amount)
        {
            if (dewCount < amount) return false;

            dewCount -= amount;
            UpdateUI();
            return true;
        }

        //Время 
        public void AddTime(float seconds)
        {
            currentTime += seconds;
            ShowMessage("+" + seconds + " секунд!");

            if (showDebugLogs)
                Debug.Log("Время +" + seconds);
        }

        //Цветы
        public void OnFlowerWatered(Flower flower)
        {
            flowersWatered++;
            dewSpentOnFlowers += flower.dewCost;

            if (showDebugLogs)
                Debug.Log("Цветок полит! " + flowersWatered + " / " + allFlowers.Count);

            CheckWin();
        }

        //Вин / луз
        private void CheckWin()
        {
            //Все политы
            bool allFlowersDone = flowersWatered >= allFlowers.Count && allFlowers.Count > 0;

            //Все росинки, потрачены на цветы
            bool allDewSpentOnFlowers = dewSpentOnFlowers >= totalDewCollected;

            //В руках пустл
            bool noDewLeft = dewCount == 0;

            if (allFlowersDone && allDewSpentOnFlowers && noDewLeft)
            {
                WinGame();
            }
        }

        private void WinGame()
        {
            if (gameEnded) return;
            gameEnded = true;

            if (winPanel != null) winPanel.SetActive(true);
            if (showDebugLogs) Debug.Log("ПОБЕДА!");
        }

        private void LoseGame()
        {
            if (gameEnded) return;
            gameEnded = true;

            if (losePanel != null) losePanel.SetActive(true);
            if (showDebugLogs) Debug.Log("ПОРАЖЕНИЕ — время вышло");
        }

        // === UI ===

        private void UpdateUI()
        {
            if (dewText != null)
                dewText.text = "Росинки: " + dewCount;

            if (timerText != null)
                timerText.text = "Время: " + Mathf.CeilToInt(currentTime);
        }

        public void ShowMessage(string msg)
        {
            if (messageText == null) return;

            messageText.text = msg;
            CancelInvoke("ClearMessage");
            Invoke("ClearMessage", 2f);
        }

        private void ClearMessage()
        {
            if (messageText != null) messageText.text = "";
        }

        // === КНОПКИ ===

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void GoToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Menu");
        }
    }
}