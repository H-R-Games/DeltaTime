using System;
using UnityEngine;

namespace rene_roid_enemy {
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy stats")]
        [SerializeField] private EnemyBaseStats _enemyBaseStats;

        #region Internal Variables
        [Header("Internal Variables")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private LayerMask _enemyLayer;

        private int _fixedFrame;
        #endregion

        #region External Variables
        public event Action<float> OnHit;
        public event Action OnDeath;
        #endregion

        public virtual void Awake()
        {
            AwakeEnemyStats();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void Start()
        {
            
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            _fixedFrame++;
        }

        #region Enemy Stats
        [Header("Enemy Stats")]
        [SerializeField] private int _level = 1;

        [SerializeField] private float _health;
        [SerializeField] private float _damage;
        [SerializeField] private float _armor;
        [SerializeField] private float _movementSpeed;

        //private EnemyBaseStats _enemyStats;

        private void AwakeEnemyStats()
        {
            //_enemyStats = Instantiate(_enemyBaseStats);

            SetEnemyStats();
        }

        private void SetEnemyStats()
        {
            _health = _enemyBaseStats.Health * _level;
            _damage = _enemyBaseStats.Damage * _level;
            _armor = _enemyBaseStats.Armor * _level;
            _movementSpeed = _enemyBaseStats.MovementSpeed * _level;
        }

        public void LevelUp()
        {
            _level++;
        }
        #endregion

        #region Health & Damage
        public void TakeDamage(float damage)
        {
            if (_armor > 0) damage *= 100 / (100 + _armor);

            _health -= damage;

            if (_health <= 0)
            {
                // DIE();
            }
        }

        public float DealDamage()
        {
            return _damage;
        }
        #endregion
    }
}
