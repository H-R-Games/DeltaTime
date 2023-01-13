using System;
using UnityEngine;

namespace rene_roid_enemy {
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBase : MonoBehaviour
    {
        [Header("Enemy stats")]
        [SerializeField] private EnemyBaseStats _enemyBaseStats;

        #region Internal Variables
        [Header("Internal Variables")]
        [SerializeField] private BoxCollider2D _boxCollider2D;
        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private LayerMask _wallLayer;

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
            GetPlayerDirection();
        }

        public virtual void Update()
        {
            
        }

        public virtual void FixedUpdate()
        {
            _fixedFrame++;

            CheckCollisions();
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
            //OnHit.Invoke(damage);

            if (_health <= 0)
            {
                // DIE();
                OnDeath?.Invoke();
                return;
            }
        }

        public float DealDamage()
        {
            return _damage;
        }
        #endregion

        #region Movement
        [Header("Movement")]
        [SerializeField] private float _movementSpeedMultiplier = 1f;
        [SerializeField] private float _headLevel = 0.5f;
        private Vector2 _movementDirection = Vector2.right;

        private bool _grounded = false;
        private bool _walled = false;

        #region Raycast
        private RaycastHit2D _feetRaycast;
        private RaycastHit2D _headRaycast;

        private void CheckCollisions()
        {
            _feetRaycast = Physics2D.Raycast(transform.position, Vector2.down, _boxCollider2D.bounds.extents.y + 0.1f, ~_enemyLayer);
            _grounded = _feetRaycast.collider != null;

            _headRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection, _boxCollider2D.bounds.extents.y + 10f, _wallLayer);
            _walled = _headRaycast.collider != null;
        }

        private void GetPlayerDirection()
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            _movementDirection = (player.transform.position - this.transform.position).normalized;
        }

        #region Horizontal
        private void Horizontal()
        {
            if (_grounded && !_walled)
            {
                transform.Translate(_movementDirection * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);
            }
        }
        #endregion
        #endregion

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector2.down * (_boxCollider2D.bounds.extents.y + 0.1f));

            Gizmos.color = Color.green;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2(0, _headLevel), Vector2.right * (_boxCollider2D.bounds.extents.y + 10f));
        }
    }
}
