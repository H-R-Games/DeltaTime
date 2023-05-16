using System.Collections;
using UnityEngine;
using TMPro;

namespace rene_roid {
    [RequireComponent(typeof(AudioSource))]
    public class BackgroundMusic : MonoBehaviour
    {
        #region Play Music
        [Header("Music Clips")]
        [SerializeField] private Music[] _musicClips;
        private AudioSource _audioSource;
        private int _currentClipIndex = 0;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void PlayNextClip()
        {
            _currentClipIndex = (_currentClipIndex + 1) % _musicClips.Length;
            _audioSource.clip = _musicClips[_currentClipIndex].Clip;
            _audioSource.Play();

            ShowMusicInfo();
        }

        private void OnEnable() {
            _canvas = GameObject.Find("Current Music").GetComponent<Canvas>();
            _musicName = _canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            _musicArtist = _canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

            _audioSource.clip = _musicClips[_currentClipIndex].Clip;
            _audioSource.Play();
            _audioSource.loop = false;

            ShowMusicInfo();
        }

        private void Update()
        {
            if (!_audioSource.isPlaying)
            {
                PlayNextClip();
            }
        }

        private void OnDisable()
        {
            _audioSource.Stop();

            StopAllCoroutines();
            _musicName.text = "";
            _musicArtist.text = "";
        }
        #endregion

        #region Show Music Info
        [Header("Music Info")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TextMeshProUGUI _musicName;
        [SerializeField] private TextMeshProUGUI _musicArtist;

        private void Start() {
            // _canvas = GameObject.Find("Current Music").GetComponent<Canvas>();
            // _musicName = _canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            // _musicArtist = _canvas.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        // Create a function that shows the current music name and artist in the canvas with a type writer effect for 1 second
        private void ShowMusicInfo()
        {
            StartCoroutine(ShowMusicInfoCoroutine());
        }
    
        private IEnumerator ShowMusicInfoCoroutine()
        {
            var musicName = _musicClips[_currentClipIndex].Name;
            var musicArtist = _musicClips[_currentClipIndex].Artist;
            var musicNameLength = musicName.Length;
            var musicArtistLength = musicArtist.Length;
            var musicNameIndex = 0;
            var musicArtistIndex = 0;

            while (musicNameIndex < musicNameLength || musicArtistIndex < musicArtistLength)
            {
                if (musicNameIndex < musicNameLength)
                {
                    _musicName.text += musicName[musicNameIndex];
                    musicNameIndex++;
                }

                if (musicArtistIndex < musicArtistLength)
                {
                    _musicArtist.text += musicArtist[musicArtistIndex];
                    musicArtistIndex++;
                }

                yield return Helpers.GetWait(0.05f);
            }

            yield return Helpers.GetWait(2f);

            while (musicNameIndex > 0 || musicArtistIndex > 0)
            {
                if (musicNameIndex > 0)
                {
                    _musicName.text = _musicName.text.Remove(musicNameIndex - 1);
                    musicNameIndex--;
                }

                if (musicArtistIndex > 0)
                {
                    _musicArtist.text = _musicArtist.text.Remove(musicArtistIndex - 1);
                    musicArtistIndex--;
                }

                yield return Helpers.GetWait(0.05f);
            }
        }
        #endregion
    }
}
