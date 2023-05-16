using hrTeleport;
using rene_roid_player;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace rene_roid
{
    public class GameManager : MonoBehaviour
    {
        private void Awake() {
            _director = GameObject.FindObjectOfType<Director>();
        }

        void Start()
        {
            OnPlayerStart();
            OnScenemanagement();
        }

        void Update()
        {
            PlayerUpdate();
            OnSceneManagementUpdate();
        }

        #region Player
        [Header("Player")]
        public PlayerBase _player;
        private Transform _lastPlayerGrounded;
        private Vector2 _fallPos;
        
        private float _playerFallClock = 5;
        private float _playerFallTimer;

        private void OnPlayerStart()
        {
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerBase>();
        }

        private void PlayerUpdate()
        {
            if (_player == null) return;

            // Every _playerFallClock seconds, check if the player is grounded and save the last grounded position
            _playerFallTimer += Time.deltaTime;
            if (_playerFallTimer >= _playerFallClock)
            {
                if (_player.IsGrounded())
                {
                    _lastPlayerGrounded = _player.transform;
                    _playerFallTimer = 0;
                    _fallPos = _lastPlayerGrounded.transform.position;
                }
            }
            
            if (_player.transform.position.y < -10)
            {
                print("Player fell off the map");
                _player.transform.position = _fallPos;
            }
        }
        #endregion

        #region Scene Management
        [Header("Scene Management")]
        [SerializeField] private TeleportManager _currentTeleport;
        [SerializeField] private List<GameObject> _scenes;
        [SerializeField] private GameObject _loadingScreen;
        private int _currentSceneIndex = 0;
        private Director _director;

        private void OnScenemanagement()
        {
            LoadMap(_currentSceneIndex);
        }

        private void OnSceneManagementUpdate() {
            if (_currentTeleport == null) {
                return;
            }
            
            if (_currentTeleport.IsFinished)
            {
                print("Teleport finished");
                StartCoroutine(LoadNextMap());
            }
        }

        private void LoadMap(int index)
        {
            if (index >= _scenes.Count) return;
            _scenes[index].SetActive(true);
            _currentTeleport = _scenes[index].GetComponentInChildren<TeleportManager>();
        }

        private void UnloadMap(int index)
        {
            if (index >= _scenes.Count) return;
            _scenes[index].SetActive(false);
            _currentTeleport = null;
        }

        private IEnumerator LoadNextMap()
        {
            _loadingScreen.SetActive(true);
            
            _player.gameObject.SetActive(false);

            UnloadMap(_currentSceneIndex);
            if (_director != null) _director.NewPassiveDirectorState(Director.PassiveDirectorState.Innactive);
            yield return Helpers.GetWait(Random.Range(0.5f, 3f));
            _currentSceneIndex++;
            
            _player.gameObject.SetActive(true);
            LoadMap(_currentSceneIndex);
            
            _player.SetPlayerStats();
            _player.transform.position = _scenes[_currentSceneIndex].transform.GetChild(0).transform.position;

            _loadingScreen.SetActive(false);
            if (_director != null) _director.NewPassiveDirectorState(Director.PassiveDirectorState.Gathering);
        }
        #endregion
    }
}
