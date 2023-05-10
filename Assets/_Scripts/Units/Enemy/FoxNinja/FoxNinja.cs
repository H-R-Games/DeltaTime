using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;
using rene_roid;

namespace rene_roid_enemy
{
    public class FoxNinja : EnemyBase
    {
        public override void Start()
        {
            base.Start();
            _anim = GetComponentInChildren<Animator>();
            _enemyType = EnemyType.Horizontal;
        }

        public override void Update() { UpdateState(); HandleAnimations(); }

        #region State Machine
        public override void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    HorizontalEnemyMovement();
                    if (TargetPlayer()) ChangeState(EnemyStates.Target);
                    break;
                case EnemyStates.Attack:
                    GravityEnemy();
                    Attack();
                    break;
                case EnemyStates.Stun:
                    GravityEnemy();
                    StunUpdate();
                    break;
                case EnemyStates.Target:
                    GravityEnemy();
                    if (!TargetPlayer()) UnTargetPlayer();
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _targetDistanceUnfollow) ChangeState(EnemyStates.Move);
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackDistance + _rand) ChangeState(EnemyStates.Attack);
                    if (!_isStunned) FollowerPlayer();
                    break;
            }

            // HandleAnimations();
            _onHitRange = _attackDistance;
        }

        public override void ChangeState(EnemyStates newState)
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    _idleAnim = false;
                    break;
                case EnemyStates.Attack:
                    _onAttack = false;
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
                    break;
            }

            switch (newState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    _idleAnim = true;
                    break;
                case EnemyStates.Attack:
                    _onAttack = true;
                    break;
                case EnemyStates.Stun:
                    StunnStart(5);
                    break;
                case EnemyStates.Target:
                    break;
            }
            _enemyState = newState;
        }
        #endregion

        #region Movement
        private void HorizontalEnemyMovement()
        {
            if (!_isStunned) Horizontal();
            GravityEnemy();
        }

        #region Horeizontal
        /// <summary>
        /// Function that moves the enemy horizontally
        /// </summary>
        private void Horizontal()
        {
            Vector3 direction = new Vector3(_movementDirection.x + _rand, 0, 0);

            if (_isGround)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
                _movementDirection = _movementDirection * -1;
            }

            if (_walled)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
                _movementDirection = _movementDirection * -1;
            }

            if (!_isStunned) transform.Translate(direction * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);

            if (_movementDirection.x > 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
            else if (_movementDirection.x < 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
        }
        #endregion
        
        #region Target Player
        [Header("Target Player Settings")]
        [SerializeField] private float _targetDistanceWatchin = 10f;
        [SerializeField] private float _targetDistanceNotWatchin = 5f;
        [SerializeField] private float _targetDistanceUnfollow = 50f;
        [SerializeField] private float _targetTimeUnfollow = 3f;
        float _timeUnTarget = 0;
        
        /// <summary>
        /// Function that checks if the player is in the range of the enemy and the enemy is looking at the player
        /// </summary>
        private bool TargetPlayer()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            bool isRange = Vector3.Distance(transform.position, _targetPlayer.transform.position) <= (watchin ? _targetDistanceWatchin : _targetDistanceNotWatchin);
            if (_hitTarget.collider != null && _hitTarget.collider.gameObject.tag == "Player" && isRange) return true;
            else return false;
        }

        /// <summary>
        /// UnTargetPlayer is called when the player is not in the range of the enemy and the enemy is not looking at the player
        /// </summary>
        private void UnTargetPlayer()
        {
            _timeUnTarget += Time.deltaTime * 0.5f;

            if (_timeUnTarget < _targetTimeUnfollow)
            {
                if (_hitTarget.collider != null && _hitTarget.collider.gameObject.tag == "Player") { _timeUnTarget = 0; return;};
                if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _targetDistanceUnfollow) { _timeUnTarget = 0; return;};
            }
            else if (_timeUnTarget >= 1) { _timeUnTarget = 0; ChangeState(EnemyStates.Move); }
        }

        /// <summary>
        /// Function that makes the enemy follow the player
        /// </summary>
        private void FollowerPlayer()
        {
            Vector3 directionX = (_targetPlayer.transform.position.x - this.transform.position.x) > 0 ? Vector3.right : Vector3.left;
            if (Vector3.Distance(-new Vector3(0, transform.position.y, 0), new Vector3(0, _targetPlayer.transform.position.y, 0)) > 3) Jump();
            if (!_isGround && !_walled) transform.Translate(directionX * _movementSpeed * (_movementSpeedMultiplier + _rand) * Time.deltaTime);

            if (_movementDirection.x > 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
            else if (_movementDirection.x < 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;

            _movementDirection = directionX;
        }
        #endregion
        #endregion

        #region Attack
        [Header("Attack Settings")]
        [SerializeField] private float _attackDistance = 1f;
        [SerializeField] private float _attackTime = 1f;
        [SerializeField] private Transform _proyectileSpawn;
        [SerializeField] private GameObject _proyectile;
        [SerializeField] private float _attackCooldown = 1f;
        float _timeAttack = 0;

        private void Attack()
        {
            _timeAttack += Time.deltaTime * 0.5f;

            if (_timeAttack >= _attackTime)
            {
                if (_proyectileSpawn == null) return;
                if (_proyectile == null) return;

                _timeAttack = 0;
                _attackCooldown = 0;

                if(Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) < 3f) return;

                StartCoroutine(AttackCoroutine());
            }
        }

        IEnumerator AttackCoroutine()
        {
            ChangeState(EnemyStates.Attack);
            var proyectil = Instantiate(_proyectile, _proyectileSpawn.position, Quaternion.identity);
            proyectil.GetComponent<ProyectilContreoller>().SetValues(_damage, _movementDirection);
            yield return Helpers.GetWait(0.5f);
            ChangeState(EnemyStates.Target);
        }

        #endregion

        #region Animation
        private Animator _anim;
            private int _currentAnimation = 0;
            private float _lockedTill;
            private static readonly int Idle = Animator.StringToHash("Idle");
            private static readonly int Run = Animator.StringToHash("Run");
            private static readonly int Death = Animator.StringToHash("Die");
            private static readonly int StunnedAnim = Animator.StringToHash("Stunned");
            private static readonly int AttackAnim = Animator.StringToHash("Attack");
            bool _idleAnim;
            bool _onAttack;

            [SerializeField] private float _attackAnimTime = 0.5f;

            private void HandleAnimations()
            {
                var state = GetState();
                ResetFlags();
                if (state == _currentAnimation) return;

                _anim.Play(state, 0); //_anim.CrossFade(state, 0, 0);
                _currentAnimation = state;

                int GetState()
                {
                    if (Time.time < _lockedTill) return _currentAnimation;

                    // ANY SKILL PRESSED
                    // if (_isStunned) return LockState(UltimateAttackAnim, 5);
                    if (_onAttack) return LockState(AttackAnim, _attackAnimTime);
                    if (_idleAnim) return Idle;

                    // NO SKILL PRESSED
                    return Run;

                    // State and time to lock
                    int LockState(int s, float t)
                    {
                        _lockedTill = Time.time + t;
                        return s;
                    }

                }

                void ResetFlags() { _onAttack = false; }
            }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_targetPlayer == null) return;
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            // bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            // bool isRange = Vector3.Distance(transform.position, _targetPlayer.transform.position) < (watchin ? _targetDistanceWatchin : _targetDistanceNotWatchin);

            // Gizmos.color = isRange ? Color.blue : Color.green;
            // Gizmos.DrawWireSphere(transform.position, _targetDistanceNotWatchin);
            // Gizmos.DrawWireSphere(transform.position, (isRange ? _targetDistanceWatchin : _targetDistanceNotWatchin));

            Vector2 dir = _movementDirection;
            // Vector2 dir2 = p;

            // Gizmos.color = isRange ? Color.blue : Color.green;
            // Gizmos.DrawLine((Vector2)transform.position, (dir) + (Vector2)transform.position);
            // Gizmos.DrawLine((Vector2)transform.position, (dir2) * (isRange ? _targetDistanceWatchin : _targetDistanceNotWatchin) + (Vector2)transform.position);

            // Gizmos.color = Color.white;
            // Gizmos.DrawRay((Vector2)transform.position + new Vector2((_movementDirection.x > 0 ? 1 : -1), 0), Vector2.down * 5);

            // Gizmos.color = Color.yellow;
            // Gizmos.DrawLine(transform.position, Vector2.up * 5 + (Vector2)transform.position);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 3f);

            Gizmos.color = Color.red;
            // Gizmos.DrawLine((Vector2)transform.position + new Vector2((_movementDirection.x > 0 ? 1 : -1), 0) + Vector2.left, _boxCollider2D.bounds.extents.x + 5);
        }
#endif
    }
}