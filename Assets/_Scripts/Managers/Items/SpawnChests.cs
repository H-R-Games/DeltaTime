using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_player {    
    public class SpawnChests : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        private PlayerBase _player;
        [SerializeField] private Transform[] _chestSpawnPoints;
        //[SerializeField] private Transform[] _specialChestSpawnPoints;
        [SerializeField] private float _chestSpawnPercentage = 0.8f;

        private List<GameObject> _chestList = new List<GameObject>();
        #endregion

        #region External
        [Header("External")]
        public GameObject _chestPrefab;
        public GameObject _bidChestPrefab;

        public GameObject _chestSpawnPointParent;
        //public GameObject _specialChestSpawnPointParent;

        #endregion

        private void OnEnable() {
            LoadChests();
        }

        private void OnDisable() {
            foreach (var chest in _chestList) {
                Destroy(chest);
            }
        }

        private void Start()
        {
            _player = FindObjectOfType<PlayerBase>();
        }

        private void LoadChests() {
            //_chestSpawnPointParent = GameObject.Find("ChestSpawnPoints");
            //_specialChestSpawnPointParent = GameObject.Find("SpecialChestSpawnPoints");

            // Get all child objects of the chest spawn point parent
            //_chestSpawnPoints = _chestSpawnPointParent.GetComponentsInChildren<Transform>();
            //_specialChestSpawnPoints = _specialChestSpawnPointParent.GetComponentsInChildren<Transform>();
            _chestSpawnPointParent = _chestSpawnPoints[0].parent.gameObject;

            // Spawn chests
            var chestSpawnPoints = Mathf.RoundToInt(_chestSpawnPoints.Length * _chestSpawnPercentage);
            var indexArray = new int[_chestSpawnPoints.Length];
            for (int i = 0; i < indexArray.Length; i++) {
                indexArray[i] = i;
            }
            // Shuffle the array
            for (int i = 0; i < indexArray.Length; i++) {
                var temp = indexArray[i];
                var randomIndex = Random.Range(i, indexArray.Length);
                indexArray[i] = indexArray[randomIndex];
                indexArray[randomIndex] = temp;
            }

            for (int i = 0; i < chestSpawnPoints; i++) {
                var chest = Instantiate(_chestPrefab, _chestSpawnPoints[indexArray[i]].position, Quaternion.identity);
                chest.transform.SetParent(_chestSpawnPointParent.transform);
                _chestList.Add(chest);
            }

                // var chest = Instantiate(_chestPrefab, _chestSpawnPoints[randomChest].position, Quaternion.identity);
                // chest.transform.SetParent(_chestSpawnPointParent.transform);
                // _chestList.Add(chest);

            //SpawnSpecialChest();
        }

        // public void SpawnSpecialChest() {
        //     if (_specialChestSpawnPoints.Length == 0) return;
        //     var randomChest = Random.Range(0, _specialChestSpawnPoints.Length);
        //     var chest = Instantiate(_bidChestPrefab, _specialChestSpawnPoints[randomChest].position, Quaternion.identity);
        //     chest.transform.SetParent(_specialChestSpawnPointParent.transform);
        // }
    }
}
