using rene_roid;
using rene_roid_player;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace rene_roid_enemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBase : MonoBehaviour
    {
        public enum EnemyStates { Idle, Move, Attack, Stun, Target }

        [Header("Enemy stats")]
        [SerializeField] private EnemyBaseStats _enemyBaseStats;
        protected EnemyStates _enemyState;

        #region Internal Variables
        [Header("Internal Variables")]
        [SerializeField] protected BoxCollider2D _boxCollider2D;
        [SerializeField] protected LayerMask _enemyLayer;
        [SerializeField] protected LayerMask _wallLayer;

        protected PlayerBase _targetPlayer = null;

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
            _targetPlayer = FindObjectOfType<PlayerBase>();
        }

        public virtual void Start()
        {
            GetPlayerDirection();
        }

        public virtual void Update()
        {
            UpdateState();
        }

        public virtual void FixedUpdate()
        {
            _fixedFrame++;
            CheckCollisions();
        }

        #region Enemy Stats
        [Header("Enemy Stats")]
        [SerializeField] protected int _level = 1;
        [SerializeField] protected float _health;
        [SerializeField] protected float _damage;
        [SerializeField] protected float _armor;
        [SerializeField] protected float _movementSpeed;

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

        public void LevelUp() { _level++; }
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

        #region State Machine
        public virtual void ChangeState(EnemyStates newState) { }

        public virtual void UpdateState() { }
        #endregion

        #region Movement
        [Header("Movement")]
        [SerializeField] protected float _movementSpeedMultiplier = 1f;
        [SerializeField] protected float _headLevel = 0.5f;
        [SerializeField] protected float _gravity = -9.15f;
        [SerializeField] protected float _timeStun = 0;
        
        protected Vector2 _movementDirection = Vector2.right;
        protected bool _grounded = false;
        protected bool _walled = false;
        protected bool _isStunned = false;
        protected bool _isGround = true;

        #region Raycast
        private RaycastHit2D _feetRaycast;
        private RaycastHit2D _headRaycast;
        private RaycastHit2D _grounRaycast;

        public virtual void CheckCollisions()
        {
            _feetRaycast = Physics2D.Raycast(transform.position, Vector2.down, _boxCollider2D.bounds.extents.y + 0.1f, ~_enemyLayer);
            _grounded = _feetRaycast.collider != null;
            
            _grounRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2((_movementDirection.x > 0 ? 1 : -1), 0), Vector2.down, _boxCollider2D.bounds.extents.y + 5, ~_enemyLayer);
            _isGround = _grounRaycast.collider == null;

            _headRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection, _boxCollider2D.bounds.extents.y + 1f, _wallLayer);
            _walled = _headRaycast.collider != null;
        }

        public virtual void GetPlayerDirection()
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            _movementDirection = (player.transform.position - this.transform.position).normalized;
        }
        #endregion

        #region Stun
        public virtual void StunnStart(int t)
        {
            _timeStun = Time.time + t;
            _isStunned = true;
        }

        public virtual void StunUpdate()
        {
            if (Time.time >= _timeStun)
            {
                _isStunned = false;
                ChangeState(EnemyStates.Move);
            }
        }
        #endregion
        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector2.down * (_boxCollider2D.bounds.extents.y + 0.1f));

            Gizmos.color = Color.green;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection * new Vector2(0, _boxCollider2D.bounds.extents.y + 1f) * Vector2.right);
        }
    }
}
