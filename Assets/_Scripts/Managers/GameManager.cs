using rene_roid_player;
using UnityEngine;

namespace rene_roid
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            OnPlayerStart();
        }

        void Update()
        {
            PlayerUpdate();
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
    }
}
