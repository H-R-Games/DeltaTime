using UnityEngine;

namespace rene_roid_player {    
    public class SpawnChests : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        private PlayerBase _player;
        private Transform[] _chestSpawnPoints;
        private Transform[] _specialChestSpawnPoints;
        [SerializeField] private float _chestSpawnPercentage = 0.8f;
        #endregion

        #region External
        [Header("External")]
        public GameObject _chestPrefab;
        public GameObject _bidChestPrefab;

        public GameObject _chestSpawnPointParent;
        public GameObject _specialChestSpawnPointParent;

        #endregion

        private void Start()
        {
            _player = FindObjectOfType<PlayerBase>();
            //LoadChests();
        }

        private void LoadChests() {
            _chestSpawnPointParent = GameObject.Find("ChestSpawnPoints");
            _specialChestSpawnPointParent = GameObject.Find("SpecialChestSpawnPoints");

            // Get all child objects of the chest spawn point parent
            _chestSpawnPoints = _chestSpawnPointParent.GetComponentsInChildren<Transform>();
            _specialChestSpawnPoints = _specialChestSpawnPointParent.GetComponentsInChildren<Transform>();

            // Spawn chests
            var chestSpawnPoints = Mathf.RoundToInt(_chestSpawnPoints.Length * _chestSpawnPercentage);
            for (int i = 0; i < chestSpawnPoints; i++) {
                var chest = Instantiate(_chestPrefab, _chestSpawnPoints[i].position, Quaternion.identity);
                chest.transform.SetParent(_chestSpawnPointParent.transform);
            }

            SpawnSpecialChest();
        }

        public void SpawnSpecialChest() {
            if (_specialChestSpawnPoints.Length == 0) return;
            var randomChest = Random.Range(0, _specialChestSpawnPoints.Length);
            var chest = Instantiate(_bidChestPrefab, _specialChestSpawnPoints[randomChest].position, Quaternion.identity);
            chest.transform.SetParent(_specialChestSpawnPointParent.transform);
        }
    }
}
