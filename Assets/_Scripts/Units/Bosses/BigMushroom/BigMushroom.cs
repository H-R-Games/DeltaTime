using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_enemy
{
    public class BigMushroom : EnemyBase
    {
        public override void Start()
        {
            base.Start();
            ChangeState(EnemyStates.Target);
            _anim = GetComponentInChildren<Animator>();
        }

        public override void Update() { UpdateState(); }

        #region State Machine
        public override void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    break;
                case EnemyStates.Attack:
                    if (!_isStunned) SetAttack();
                    // if (Vector3.Distance(this.transform.position, _targetPlayer.transform.position) > 2) ChangeState(EnemyStates.Target);
                    break;
                case EnemyStates.Stun:
                    StunUpdate();
                    break;
                case EnemyStates.Target:
                    if (!_isStunned) FollowerPlayer();
                    if (Vector3.Distance(this.transform.position, _targetPlayer.transform.position) < 2) ChangeState(EnemyStates.Attack);
                    break;
            }

            HandleAnimations();
            GravityEnemy();
        }

        public override void ChangeState(EnemyStates newState)
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
                case EnemyStates.Target:
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
                    StunnStart(5);
                    break;
                case EnemyStates.Target:
                    break;
            }
            _enemyState = newState;
        }
        #endregion

        #region Movement
        /// <summary>
        /// Function that makes the enemy follow the player
        /// </summary>
        private void FollowerPlayer()
        {
            Vector3 directionX = (_targetPlayer.transform.position.x - this.transform.position.x) > 0 ? Vector3.right : Vector3.left;
            if (Vector3.Distance(-new Vector3(0, transform.position.y, 0), new Vector3(0, _targetPlayer.transform.position.y, 0)) > 3) Jump();
            if (!_isGround && !_walled) transform.Translate(directionX * _movementSpeed * (_movementSpeedMultiplier + _rand) * Time.deltaTime);

            if (_movementDirection.x > 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX =_movementDirection.x < 0;
            else if (_movementDirection.x < 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f) this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;

            _movementDirection = directionX;
        }
        #endregion

        #region Attack
        [Header("Attack Settings")]
        [SerializeField] private GameObject _sporesPrefabs;
        [SerializeField] private float _sporesCooldown = 1f;
        bool _attackSpores;
        float _timerSpores;

        private void SetAttack()
        {
            int num = Random.Range(0, 3);

            switch (num)
            {
                case 0:
                    StartCoroutine(SporesAttack());
                    break;
                case 1:
                    StartCoroutine(SporesAttack());
                    break;
                case 2:
                    StartCoroutine(SporesAttack());
                    break;
            }
        }

        private IEnumerator SporesAttack()
        {
            _attackSpores = true;
            ChangeState(EnemyStates.Idle);
            yield return new WaitForSeconds(_attackSpoeresAnimTime);
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
            private static readonly int AttackSpores = Animator.StringToHash("AttackSpores");

            [SerializeField] private float _attackSpoeresAnimTime = 0.5f;

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
                    if (_attackSpores) return LockState(AttackSpores, _attackSpoeresAnimTime);

                    // NO SKILL PRESSED
                    return Run;

                    // State and time to lock
                    int LockState(int s, float t)
                    {
                        _lockedTill = Time.time + t;
                        return s;
                    }

                }

                void ResetFlags() { _attackSpores = false;}
            }
        #endregion
    }
}