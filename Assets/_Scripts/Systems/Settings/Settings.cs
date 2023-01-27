using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

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
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            _audioMixer.SetFloat("MusicVolume", Helpers.FromPercentageToRange(volume, -80f, 0f, true));
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            _audioMixer.SetFloat("SFXVolume", Helpers.FromPercentageToRange(volume, -80f, 0f, true));
        }

        public void SetMute(bool isMuted)
        {
            _isMuted = isMuted;
            _audioMixer.SetFloat("MasterVolume", _isMuted ? -80 : Helpers.FromPercentageToRange(_masterVolume, -80f, 0f, true));
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
        }

        public void SetResolution(int resolutionIndex)
        {
            _resolutionIndex = resolutionIndex;
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetQuality(int qualityIndex)
        {
            _qualityIndex = qualityIndex;
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            _isFullscreen = isFullscreen;
            Screen.fullScreen = isFullscreen;
        }
        #endregion

        #region Keybinds Keyboards
        
        #endregion
    }
}