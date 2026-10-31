using UnityEngine;
using System.Collections.Generic;

namespace WaterProstecting
{
    [System.Serializable]
    public class ShopItem
    {
        public string itemName = "Шляпка";
        public int price = 5;
        public GameObject prefab;
        public Color bodyColor = Color.white;
        public bool isColorChange = false;
    }

    public class SheepQuest : MonoBehaviour
    {
        public static SheepQuest Instance;

        [Header("Фразы")]
        public string[] idlePhrases = new string[] { "Бе-е! Собери 3 росинки!", "Бе-е-е! Помоги цветочкам!" };
        public string questPhrase = "Бе-е! Принеси 3 росинки — покажу спрятанное!";
        public string thanksPhrase = "Бе-е! Спасибо! Смотри, что я нашла!";

        [Header("Квест")]
        public int questDewRequired = 3;

        [Header("Спрятанные росинки")]
        public List<GameObject> hiddenDews = new List<GameObject>();

        [Header("Магазин")]
        public List<ShopItem> shopItems = new List<ShopItem>();

        [Header("Звуки (Audio)")]
        public AudioSource audioSource;     // Ссылка на AudioSource
        public AudioClip baaSound;          // Звук "Бе-е!"
        [Range(0f, 1f)] public float soundVolume = 0.8f; // Громкость

        [Header("Отладка")]
        public bool showDebugLogs = true;

        private bool questGiven = false;
        private bool questCompleted = false;
        private int selectedShopIndex = 0;
        private Transform playerTransform;
        private PlayerCustomization playerCustomization;

        void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                playerCustomization = player.GetComponent<PlayerCustomization>();
            }

            foreach (GameObject dew in hiddenDews)
            {
                if (dew != null) dew.SetActive(false);
            }
        }

        public void Interact()
        {
            if (GameManager.Instance == null) return;

            // При каждом нажатии E проигрываем звук
            PlayBaaSound();

            if (!questGiven)
            {
                TryGiveQuest();
            }
            else if (questCompleted)
            {
                OpenShop();
            }
            else
            {
                GameManager.Instance.ShowMessage("Бе-е! Жду росинки...");
            }
        }

        // Метод для воспроизведения звука
        private void PlayBaaSound()
        {
            if (baaSound != null && audioSource != null)
            {
                // Немного меняем тон, чтобы каждый раз звучало чуть по-разному
                audioSource.pitch = Random.Range(0.85f, 1.15f); 
                audioSource.PlayOneShot(baaSound, soundVolume);
            }
        }

        private void TryGiveQuest()
        {
            if (GameManager.Instance.TotalDewCollected >= questDewRequired)
            {
                questGiven = true;

                if (!GameManager.Instance.SpendDew(questDewRequired))
                {
                    questGiven = false;
                    return;
                }

                GameManager.Instance.ShowMessage(questPhrase);
                UnhideDews();
                questCompleted = true;

                Invoke("SayThanks", 2f);

                if (showDebugLogs) Debug.Log("Квест барашка выполнен");
            }
            else
            {
                string phrase = idlePhrases[Random.Range(0, idlePhrases.Length)];
                GameManager.Instance.ShowMessage(phrase);
            }
        }

        private void SayThanks()
        {
            if (GameManager.Instance != null)
            {
                // Барашек снова беекает, когда благодарит
                PlayBaaSound();
                GameManager.Instance.ShowMessage(thanksPhrase);
            }
        }

        private void UnhideDews()
        {
            foreach (GameObject dew in hiddenDews)
            {
                if (dew != null) dew.SetActive(true);
            }
        }

        public void OnSpecialDewCollected()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.ShowMessage("Бе-е! Ты нашёл особенную росинку!");
        }

        private void OpenShop()
        {
            if (shopItems == null || shopItems.Count == 0)
            {
                GameManager.Instance.ShowMessage("Бе-е! Пока ничего нет.");
                return;
            }

            ShopItem item = shopItems[selectedShopIndex];

            if (GameManager.Instance.SpendDew(item.price))
            {
                if (playerCustomization != null)
                    playerCustomization.ApplyItem(item);

                GameManager.Instance.ShowMessage("Куплено: " + item.itemName + "!");

                if (showDebugLogs) Debug.Log("Куплен предмет: " + item.itemName);

                selectedShopIndex++;
                if (selectedShopIndex >= shopItems.Count) selectedShopIndex = 0;
            }
            else
            {
                GameManager.Instance.ShowMessage("Нужно " + item.price + " росинок!");
            }
        }
    }
}