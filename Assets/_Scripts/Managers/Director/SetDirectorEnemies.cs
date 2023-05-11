using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_enemy;

namespace rene_roid {
    public class SetDirectorEnemies : MonoBehaviour
    {
        private Director _director;
        [Header("Enemies")]
        [SerializeField] private List<EnemyBase> _stageEnemies;

        private void Awake() {
            if (_director == null) {
                // Find director
                _director = GameObject.FindObjectOfType<Director>();
            }
        }
        
        private void OnEnable() {
            if (_director == null) {
                // Find director
                _director = GameObject.FindObjectOfType<Director>();
            }
            _director.ClearStageEnemies();
            // Set enemies
            _director.SetStageEnemies(_stageEnemies);
        }
    }
}
