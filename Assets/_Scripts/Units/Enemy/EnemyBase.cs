using rene_roid;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_enemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBase : MonoBehaviour
    {
        public enum EnemyTypes { EnemyHorizontal, EnemyFly, EnemyPro };
        public enum EnemyStates { Idle, Move, Attack, Stun, Dead }

        [Header("Enemy stats")]
        [SerializeField] private EnemyBaseStats _enemyBaseStats;
        [SerializeField] private EnemyStates _enemyState;

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
        public EnemyTypes EnemyType;
        #endregion

        public virtual void Awake()
        {
            AwakeEnemyStats();
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        public virtual void Start()
        {
            GetPlayerDirection();
            _enemyState = EnemyStates.Move;
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
        [SerializeField] private float _gravity = -9.15f;
        private Vector2 _movementDirection = Vector2.right;
        [SerializeField] private float _timeStun = 0;

        private bool _grounded = false;
        private bool _walled = false;
        private bool _isTarget = false;
        private bool _isStunned = false;

        #region State Machine
        private void ChangeState(EnemyStates newState)
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:

                    break;
                case EnemyStates.Move:

                    break;
                case EnemyStates.Attack:

                    break;
                case EnemyStates.Stun:

                    break;
                case EnemyStates.Dead:

                    break;
            }

            switch (newState)
            {
                case EnemyStates.Idle:

                    break;
                case EnemyStates.Move:

                    break;
                case EnemyStates.Attack:

                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Dead:

                    break;
            }

            _enemyState = newState;
        }

        public void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:

                    break;
                case EnemyStates.Move:
                    switch (EnemyType)
                    {
                        case EnemyTypes.EnemyHorizontal:
                            HorizontalEnemyMovement();
                            break;
                        case EnemyTypes.EnemyFly:
                            break;
                        case EnemyTypes.EnemyPro:
                            break;
                    }
                    break;
                case EnemyStates.Attack:

                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Dead:

                    break;
            }

            StunUpdate();
        }

        #endregion
        #region Raycast
        private RaycastHit2D _feetRaycast;
        private RaycastHit2D _headRaycast;

        private void CheckCollisions()
        {
            _feetRaycast = Physics2D.Raycast(transform.position, Vector2.down, _boxCollider2D.bounds.extents.y + 0.1f, ~_enemyLayer);
            _grounded = _feetRaycast.collider != null;

            _headRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection, _boxCollider2D.bounds.extents.y + 1f, _wallLayer);
            _walled = _headRaycast.collider != null;
        }

        private void GetPlayerDirection()
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            _movementDirection = (player.transform.position - this.transform.position).normalized;
        }

        #endregion

        #region Stun
        private void StunnStart(int t)
        {
            _timeStun = Time.time + t;
            _isStunned = true;
        }

        private void StunUpdate()
        {
            if (Time.time > _timeStun) _isStunned = false;
        }
        #endregion

        private void HorizontalEnemyMovement()
        {
            Horizontal();
            Vertical();
        }

        #region Horeizontal
        private void Horizontal()
        {
            Vector3 direction = new Vector3(_movementDirection.x, 0, 0);

            if (_isStunned) return;
            if (_walled) _movementDirection = _movementDirection * -1;
            if (!_grounded) _movementDirection = _movementDirection * -1;
            if (!_isStunned) transform.Translate(direction * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);
        }
        #endregion

        #region Verticall
        private void Vertical()
        {
            Mathf.Clamp(_gravity, -_gravity, _gravity);
            if (!_grounded) transform.Translate(new Vector3(0, _gravity * Time.deltaTime, 0));
        }
        #endregion

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, Vector2.down * (_boxCollider2D.bounds.extents.y + 0.1f));

            Gizmos.color = Color.green;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection * new Vector2(0, _boxCollider2D.bounds.extents.y + 1f) * Vector2.right);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 5f);
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 500, 100), "Stun"))
            {
                StunnStart(100);
            }
        }
    }
}
