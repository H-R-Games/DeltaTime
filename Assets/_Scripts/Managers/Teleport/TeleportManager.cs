using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;
using rene_roid_enemy;

namespace hrTeleport
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Teleport Settings")]
        [SerializeField] private List<Places> _places;
        [SerializeField] private GameObject _piece;
        [SerializeField] private int _piecesToActivate = 3;
        [SerializeField][Range(0f, 100f)] private float _timeToFinnishLoad = 5f;
        [SerializeField] private LayerMask _playerLayer;

        [Header("Boss Settings")]
        [SerializeField] private GameObject _boss;
        [SerializeField] private GameObject _bossSpawnPoint;

        PlayerBase _player;
        BoxCollider2D _collider;
        bool _isBossDied = false;
        bool _isSpwaned = false;
        bool _isFinished = false;
        bool _isActive = false;
        float _timeLoaded = 0f;
        public static int _piecesActivated = 0;
        EnemyBase _bossScript;
        List<GameObject> _pieces = new List<GameObject>();
        
        void Start()
        {
            _player = GameObject.FindObjectOfType<PlayerBase>();
            _collider = GetComponentInChildren<BoxCollider2D>();
        }

        private void OnEnable() 
        {
            for(int i = 0; i < _piecesToActivate; i++)
            {
                int index = Random.Range(0, _places.Count);
                var pos = _places[index].transform.position;

                if(_places[index].isUsed)
                {
                    i--;
                    continue;
                }

                var piece = Instantiate(_piece, pos, Quaternion.identity);

                _pieces.Add(piece);
                _places[index].isUsed = true;   
            }
        }

        private void OnDisable() 
        {
            _isBossDied = false;
            _isFinished = false;
            _isActive = false;
            _timeLoaded = 0f;
            _piecesActivated = 0;

            for(int i = 0; i < _pieces.Count; i++)
            {
                Destroy(_pieces[i]);
            }

            _pieces = new List<GameObject>();

            for(int i = 0; i < _places.Count; i++)
            {
                _places[i].isUsed = false;
            }
        }

        void Update()
        {
            ActivateTeleport();
            loadTeleport();
            Debug.Log(_bossScript);
        }

        void ActivateTeleport()
        {
            var player = Physics2D.OverlapBoxAll(_collider.bounds.center, _collider.bounds.size, 0, _playerLayer);

            if (player.Length > 0 && !_isActive && _piecesActivated == _piecesToActivate)
            {
                // Debug.Log("Teleport Activated");
                _isActive = true;
                var boss = Instantiate(_boss, _bossSpawnPoint.transform.position, Quaternion.identity);
                _bossScript = boss.GetComponent<EnemyBase>();
                _isSpwaned = true;
            }
        }

        void loadTeleport()
        {
            // Debug.Log("Piesas activas: " + _piecesActivated + " / " + _piecesToActivate);
            if (!_isActive) return;
            if (_isSpwaned && _bossScript == null) _isBossDied = true;
            if (_timeLoaded >= _timeToFinnishLoad && _isBossDied) _isFinished = true;
            else if (_timeLoaded < _timeToFinnishLoad) _timeLoaded += Time.deltaTime;
        }
    }

    [System.Serializable]
    public class Places
    {
        public Transform transform;
        public bool isUsed;

        public Places(Transform pos, bool isUsed)
        {
            transform = pos;
            isUsed = isUsed;
        }
    }
}