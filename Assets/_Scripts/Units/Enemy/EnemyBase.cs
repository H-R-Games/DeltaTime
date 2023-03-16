using rene_roid_player;
using UnityEngine;
using System;

namespace rene_roid_enemy
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class EnemyBase : MonoBehaviour
    {
        public enum EnemyStates { Idle, Move, Attack, Stun, Target, KnockBack, Dead, Attack2 }
        public enum EnemyType { Horizontal, Flying, Boss }

        [Header("Enemy stats")]
        [SerializeField] private EnemyBaseStats _enemyBaseStats;
        [SerializeField] protected EnemyStates _enemyState;
        [SerializeField] protected EnemyType _enemyType;

        #region Internal Variables
        [Header("Internal Variables")]
        [SerializeField] protected BoxCollider2D _boxCollider2D;
        [SerializeField] protected LayerMask _enemyLayer;
        [SerializeField] protected LayerMask _wallLayer;
        [SerializeField] protected LayerMask _playerLayer;
        protected PlayerBase _targetPlayer = null;
        protected int _fixedFrame;
        protected float _rand = 0.1f;
        #endregion

        #region External Variables
        public event Action<float> OnHit;

        public EnemyBaseStats EnemyBaseStats { get => _enemyBaseStats; }
        #endregion

        public virtual void Awake()
        {
            AwakeEnemyStats();
            _boxCollider2D = GetComponent<BoxCollider2D>();
            _targetPlayer = FindObjectOfType<PlayerBase>();
            _rand = UnityEngine.Random.Range(-0.2f, 0.2f);
        }

        public virtual void Start() { GetPlayerDirection(); }

        public virtual void Update() { UpdateState(); }

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

        private void AwakeEnemyStats() { SetEnemyStats(); }

        /// <summary>
        /// Set the enemy stats based on the base stats and the level
        /// </summary>
        private void SetEnemyStats()
        {
            _health = _enemyBaseStats.Health * _level;
            _damage = _enemyBaseStats.Damage * _level;
            _armor = _enemyBaseStats.Armor * _level;
            _movementSpeed = _enemyBaseStats.MovementSpeed * _level + _rand;
        }

        public void LevelUp() { _level++; }
        #endregion

        #region Health & Damage
        /// <summary>
        /// Function to take damage from the player
        /// </summary>
        public void TakeDamage(float damage)
        {
            if (_armor > 0) damage *= 100 / (100 + _armor);
            if (_armor < 0) damage *= 2 - 100 / (100 - _armor);

            _health -= damage;
            _targetPlayer.OnEnemyHit(damage, this);

            Debug.Log("Enemy health: " + _health);

            if (_health <= 0)
            {
                _health = 0;
                // this.gameObject.SetActive(false);
                Destroy(this.gameObject);
                _targetPlayer.OnEnemyDeath(damage, this);
                return;
            }
        }

        /// <summary>
        /// Calculate the damage the enemy will deal to the player
        /// </summary>
        public float DealDamage(float damage) { return damage; }
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
        protected float _knockBackForce = 0;
        protected float _knockBackDuration = 1;
        protected float _onHitRange = 0.5f;
        protected bool _grounded = false;
        protected bool _walled = false;
        protected bool _isStunned = false;
        protected bool _isGround = true;
        protected bool _isBlockedUp = false;
        protected bool _onHit = false;

        #region Raycast
        private RaycastHit2D _feetRaycast;
        private RaycastHit2D _headRaycast;
        private RaycastHit2D _grounRaycast;
        private RaycastHit2D _detectUp;
        protected RaycastHit2D _hitTarget;
        protected RaycastHit2D _hitPlayer;

        /// <summary>
        /// Check the collisions of the enemy with the environment
        /// </summary>
        public virtual void CheckCollisions()
        {
            _feetRaycast = Physics2D.Raycast(transform.position, Vector2.down, _boxCollider2D.bounds.extents.y + 0.1f, ~_enemyLayer);
            _grounded = _feetRaycast.collider != null;
            
            _grounRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2((_movementDirection.x > 0 ? 1 : -1), 0), Vector2.down, _boxCollider2D.bounds.extents.y + 5, ~_enemyLayer);
            _isGround = _grounRaycast.collider == null;

            _headRaycast = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, _headLevel), _movementDirection, _boxCollider2D.bounds.extents.y + 1f, _wallLayer);
            _walled = _headRaycast.collider != null;

            _detectUp = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, _headLevel), Vector2.up, _boxCollider2D.bounds.extents.y + 4f,  ~_enemyLayer);
            _isBlockedUp = _detectUp.collider != null;

            _hitTarget = Physics2D.Linecast(this.transform.position, _targetPlayer.transform.position, ~_enemyLayer);

            _hitPlayer = Physics2D.Raycast(transform.position, _movementDirection, _boxCollider2D.bounds.extents.y + _onHitRange, _playerLayer);
            _onHit = _hitPlayer.collider != null;
        }

        /// <summary>
        /// Get the direction of the player
        /// </summary>
        public virtual void GetPlayerDirection()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            _movementDirection = (player.transform.position - this.transform.position).normalized;
        }

        /// <summary>
        /// Move the enemy
        /// </summary>
        public virtual void KnockBack(float force) {
            _knockBackForce = force;
            ChangeState(EnemyStates.KnockBack);
        }

        #region Jump
        [Header("Jump Settings")]
        [SerializeField] protected bool _activateJump = false;
        [SerializeField] protected float _jumpForce = 5;
        protected bool _isJumping = false;

        /// <summary>
        /// Function to make the enemy jump
        /// </summary>
        public virtual void Jump()
        {
            if(!_activateJump) return;

            if (_grounded && !_isJumping && !_isBlockedUp)
            {
                _isJumping = true;
                _jumpForce = 0.1f;
            }

            if (_grounded && _jumpForce < 0) 
            {
                _isJumping = false;
                return;
            } 

            Mathf.Clamp(_jumpForce, -0.05f, 0.1f);
            
            _jumpForce -= Time.deltaTime * Mathf.Abs(-0.5f * 0.5f);

            transform.position = new Vector3(transform.position.x, transform.position.y + _jumpForce, transform.position.z);

            if (_grounded) return;
        }
        #endregion
        #region Vertical
        /// <summary>
        /// Function creates gravity for the enemy
        /// </summary>
        public virtual void GravityEnemy()
        {
            Mathf.Clamp(_gravity, -_gravity, _gravity);
            if (!_grounded) transform.Translate(new Vector3(0, _gravity * Time.deltaTime, 0));
        }
        #endregion
        #endregion

        #region Stun
        /// <summary>
        /// Function to stun the enemy
        /// </summary>
        public virtual void StunnStart(int t)
        {
            _timeStun = Time.time + t;
            _isStunned = true;
        }

        /// <summary>
        /// Function to update the stun
        /// </summary>
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
