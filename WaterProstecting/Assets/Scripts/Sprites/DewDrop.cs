using UnityEngine;

namespace WaterProstecting
{
    public class DewDrop : MonoBehaviour
    {
        [Header("Анимация")]
        public float rotationSpeed = 90f;
        public float bobSpeed = 2f;
        public float bobHeight = 0.15f;

        [Header("Спрятанная росинка")]
        public bool isHidden = false;
        public bool isSpecial = false;

        [Header("Звук")]
        public AudioClip collectSound; // Сюда перетаскиваем звук в инспекторе
        [Range(0f, 1f)] public float soundVolume = 0.6f; // Громкость
        public UnityEngine.Audio.AudioMixerGroup sfxMixerGroup; // Группа SFX (если есть)

        private Vector3 startPos;
        private bool collected = false;

        void Start()
        {
            startPos = transform.position;

            // Если росинка спрятана, выключаем объект при старте
            if (isHidden)
            {
                gameObject.SetActive(false);
            }
        }

        void Update()
        {
            // Вращение
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Покачивание (по синусоиде вверх-вниз)
            float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }

        void OnTriggerEnter(Collider other)
        {
            // Защита от двойного сбора (если сработало несколько коллайдеров)
            if (collected) return;
            
            // Проверяем, что в нас врезался именно игрок
            if (!other.CompareTag("Player")) return;

            collected = true;

            // 1. ЗВУК: Создаем временный объект, чтобы звук доиграл после удаления капельки
            if (collectSound != null)
            {
                GameObject tempAudio = new GameObject("TempAudio_Dew");
                tempAudio.transform.position = transform.position;
                
                AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
                audioSource.clip = collectSound;
                
                // Настраиваем звук
                audioSource.volume = soundVolume;
                audioSource.pitch = Random.Range(0.9f, 1.2f); // Случайная высота тона
                audioSource.spatialBlend = 1f; // 1 = 3D звук, 0 = 2D. Оставим 3D для реалистичности
                
                // Если назначена группа микшера, подключаем её
                if (sfxMixerGroup != null)
                {
                    audioSource.outputAudioMixerGroup = sfxMixerGroup;
                }
                
                audioSource.Play();
                
                // Удаляем временный объект через 1 секунду (когда звук точно доиграет)
                Destroy(tempAudio, 1f);
            }

            // 2. ДОБАВЛЯЕМ РОСИНКУ В СЧЕТЧИК
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddDew(1);
            }
            else
            {
                Debug.Log("Росинка собрана (GameManager не найден)");
            }

            // 3. ЕСЛИ ЭТО ОСОБЕННАЯ РОСИНКА ДЛЯ КВЕСТА
            if (isSpecial && SheepQuest.Instance != null)
            {
                SheepQuest.Instance.OnSpecialDewCollected();
            }

            // 4. УДАЛЯЕМ КАПЕЛЬКУ
            Destroy(gameObject);
        }
    }
}   