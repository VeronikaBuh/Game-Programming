using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

namespace WaterProstecting
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public static MusicManager Instance;

        [Header("Настройки")]
        public float baseVolume = 0.04f;
        public AudioMixer mainMixer;

        private AudioSource audioSource;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.loop = true;
                audioSource.playOnAwake = true;
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (audioSource != null)
            {
                audioSource.volume = baseVolume;
            }

            if (mainMixer != null)
            {
                float musicVol = PlayerPrefs.GetFloat("MusicVol", 0.75f);
                mainMixer.SetFloat("MusicVol", musicVol <= 0.001f ? -80f : Mathf.Log10(musicVol) * 20f);
            }
        }
    }
}