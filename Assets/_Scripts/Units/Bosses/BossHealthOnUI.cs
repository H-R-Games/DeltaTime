using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace rene_roid_enemy {
    public class BossHealthOnUI : MonoBehaviour
    {
        private EnemyBase _enemyBase;
        [SerializeField] private Image _healthBar;
        private float maxHP;
        private void Start() {
            _enemyBase = GetComponent<EnemyBase>();
            maxHP = _enemyBase.Health;
        }

        private void Update() {
            if (_enemyBase == null) return;
            _healthBar.fillAmount = _enemyBase.Health / maxHP;
        }
    }
}