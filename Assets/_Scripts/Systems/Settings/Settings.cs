using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace rene_roid
{
    public class Settings : MonoBehaviour
    {
        private void Awake()
        {
            AwakeAudioSettings();
        }

        void Start()
        {
            GraphicAndResulotionStart();
            LanguageStart();
        }

        void Update()
        {

        }

        #region Audio Settings
        [Header("Audio Settings")]
        // Audio Settings
        [SerializeField] private float _masterVolume = 1f;
        [SerializeField] private float _musicVolume = 1f;
        [SerializeField] private float _sfxVolume = 1f;

        [SerializeField] private bool _isMuted = false;

        // UI Elements
        [SerializeField] private Slider _masterVolumeSlider;
        [SerializeField] private Slider _musicVolumeSlider;
        [SerializeField] private Slider _sfxVolumeSlider;
        [SerializeField] private Toggle _muteButton;

        [SerializeField] private AudioMixer _audioMixer;

        private void AwakeAudioSettings()
        {
            _masterVolumeSlider.value = _masterVolume;
            _musicVolumeSlider.value = _musicVolume;
            _sfxVolumeSlider.value = _sfxVolume;
            _muteButton.isOn = _isMuted;
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = volume;
            _audioMixer.SetFloat("MasterVolume", Helpers.FromPercentageToRange(volume, -80f, 0f, true));
            PlayerPrefs.SetFloat("MasterVolume", volume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            _audioMixer.SetFloat("MusicVolume", Helpers.FromPercentageToRange(volume, -80f, 0f, true));
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            _audioMixer.SetFloat("SFXVolume", Helpers.FromPercentageToRange(volume, -80f, 0f, true));
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            _audioMixer.SetFloat("MasterVolume", _isMuted ? -80 : Helpers.FromPercentageToRange(_masterVolume, -80f, 0f, true));
            PlayerPrefs.SetInt("IsMuted", _isMuted ? 1 : 0);
        }
        #endregion

        #region Graphics Settings & Resolution
        [Header("Graphics Settings")]
        // Graphics Settings
        [SerializeField] private int _resolutionIndex = 0;
        [SerializeField] private int _qualityIndex = 0;
        [SerializeField] private bool _isFullscreen = false;

        // UI Elements
        [SerializeField] private TMP_Dropdown _resolutionDropdown;
        [SerializeField] private TMP_Dropdown _qualityDropdown;
        [SerializeField] private Toggle _fullscreenToggle;

        private Resolution[] _resolutions;

        private void GraphicAndResulotionStart()
        {
            _resolutions = Screen.resolutions;

            _resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            _resolutionIndex = currentResolutionIndex;

            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = _resolutionIndex;
            _resolutionDropdown.RefreshShownValue();

            // Get Quality Settings
            _qualityDropdown.ClearOptions();

            List<string> qualityOptions = new List<string>();
            for (int i = 0; i < QualitySettings.names.Length; i++)
            {
                qualityOptions.Add(QualitySettings.names[i]);
            }

            _qualityIndex = QualitySettings.GetQualityLevel();
            _qualityDropdown.AddOptions(qualityOptions);
            _qualityDropdown.value = _qualityIndex;
            _qualityDropdown.RefreshShownValue();

            _fullscreenToggle.isOn = _isFullscreen;

            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
            PlayerPrefs.SetInt("QualityIndex", _qualityIndex);
            PlayerPrefs.SetInt("IsFullscreen", _isFullscreen ? 1 : 0);
        }

        public void SetResolution(int resolutionIndex)
        {
            _resolutionIndex = resolutionIndex;
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            PlayerPrefs.SetInt("ResolutionIndex", _resolutionIndex);
        }

        public void SetQuality(int qualityIndex)
        {
            _qualityIndex = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex);

            PlayerPrefs.SetInt("QualityIndex", _qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            _isFullscreen = isFullscreen;
            Screen.fullScreen = isFullscreen;

            PlayerPrefs.SetInt("IsFullscreen", _isFullscreen ? 1 : 0);
        }
        #endregion

        #region Keybinds Keyboards
        
        #endregion

        #region Language Settings
        [Header("Language Settings")]
        [SerializeField] private TMP_Dropdown _languageDropdown;
        [SerializeField] private string _language = "English";
        [SerializeField] private string[] _languages = new string[] { "English", "Spanish" };
        [SerializeField] private LanguageManager _languageManager;

        private void LanguageStart()
        {
            _languageDropdown.ClearOptions();

            List<string> languageOptions = new List<string>();
            for (int i = 0; i < _languages.Length; i++)
            {
                languageOptions.Add(_languages[i]);
            }

            _languageDropdown.AddOptions(languageOptions);
            _languageDropdown.value = Array.IndexOf(_languages, _language);
            _languageDropdown.RefreshShownValue();

            // If player prefs has a language, load it
            if (PlayerPrefs.HasKey("Language"))
            {
                _language = PlayerPrefs.GetString("Language");
                _languageDropdown.value = Array.IndexOf(_languages, _language);
                _languageDropdown.RefreshShownValue();
            } else {
                // Get default language from system
                _language = Application.systemLanguage.ToString();
                _languageDropdown.value = Array.IndexOf(_languages, _language);
                _languageDropdown.RefreshShownValue();

                PlayerPrefs.SetString("Language", _language);
            }

            PlayerPrefs.SetString("Language", _language);
        }

        public void SetLanguage(int languageIndex)
        {
            _language = _languages[languageIndex];
            _languageManager.SetLanguage(_language);
            PlayerPrefs.SetString("Language", _language);
        }

        #endregion

        public void SaveSettings()
        {
            PlayerPrefs.Save();
        }

        public void LoadSettings() {
            _masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            _isMuted = PlayerPrefs.GetInt("IsMuted", 0) == 1 ? true : false;

            _resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", 0);
            _qualityIndex = PlayerPrefs.GetInt("QualityIndex", 0);
            _isFullscreen = PlayerPrefs.GetInt("IsFullscreen", 0) == 1 ? true : false;

            AwakeAudioSettings();
            GraphicAndResulotionStart();
        }
    }
}