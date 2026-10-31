using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace WaterProstecting
{
    public class AudioSettings : MonoBehaviour
    {
        [Header("Audio Mixer")]
        public AudioMixer mainMixer;

        [Header("Слайдеры (перетащи сюда)")]
        public Slider musicSlider;
        public Slider sfxSlider;
        public Slider ambientSlider;

        void Start()
        {
            //Сохранённые настройки
            float musicVol = PlayerPrefs.GetFloat("MusicVol", 0.04f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVol", 0.75f);
            float ambientVol = PlayerPrefs.GetFloat("AmbientVol", 0.75f);

            //Применение к ползункам
            if (musicSlider != null) musicSlider.SetValueWithoutNotify(musicVol);
            if (sfxSlider != null) sfxSlider.SetValueWithoutNotify(sfxVol);
            if (ambientSlider != null) ambientSlider.SetValueWithoutNotify(ambientVol);

            //Применение к микшеру
            ApplyVolume("MusicVol", musicVol);
            ApplyVolume("SFXVol", sfxVol);
            ApplyVolume("AmbientVol", ambientVol);

            //Подписка ползунки
            if (musicSlider != null) musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(musicSlider.value); });
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(sfxSlider.value); });
            if (ambientSlider != null) ambientSlider.onValueChanged.AddListener(delegate { SetAmbientVolume(ambientSlider.value); });
        }

        public void SetMusicVolume(float value)
        {
            ApplyVolume("MusicVol", value);
            PlayerPrefs.SetFloat("MusicVol", value);
        }

        public void SetSFXVolume(float value)
        {
            ApplyVolume("SFXVol", value);
            PlayerPrefs.SetFloat("SFXVol", value);
        }

        public void SetAmbientVolume(float value)
        {
            ApplyVolume("AmbientVol", value);
            PlayerPrefs.SetFloat("AmbientVol", value);
        }

        //Формула перевода в децибелы
        private void ApplyVolume(string parameterName, float value)
        {
            if (mainMixer == null) return;

            //Ползунок на самом 0 = выкл звук
            if (value <= 0.0001f)
            {
                mainMixer.SetFloat(parameterName, -80f);
            }
            else
            {
                //Перевод в децибелы
                float db = Mathf.Log10(value) * 20f;
                mainMixer.SetFloat(parameterName, db);
            }
        }
    }
}