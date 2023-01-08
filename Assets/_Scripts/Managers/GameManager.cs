using rene_roid_player;
using UnityEngine;

namespace rene_roid {
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            OnPlayerStart();
        }

        void Update()
        {
            
        }

        #region Player
        [Header("Player")]
        public PlayerBase _player;
        private Transform _lastPlayerGrounded;
        private float _playerFallClock = 5;
        private float _playerFallTimer;
        
        private void OnPlayerStart()
        {
            _player = GameObject.FindWithTag("Player").GetComponent<PlayerBase>();
        }

        private void PlayerUpdate()
        {
            if (_player == null) return;

        }
        #endregion
    }
}
