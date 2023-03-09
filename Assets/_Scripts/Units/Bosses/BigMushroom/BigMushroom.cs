using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;
using rene_roid;

namespace rene_roid_enemy
{
    public class BigMushroom : EnemyBase
    {
        public override void Start()
        {
            base.Start();
            ChangeState(EnemyStates.Target);
            _anim = GetComponentInChildren<Animator>();
            _colliderNibble.GetComponent<Collider2D>().enabled = false;
            _colliderClap.GetComponent<Collider2D>().enabled = false;
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
                    _idleAnim = false;
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
                    _idleAnim = true;
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

            if (_movementDirection.x > 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
                _colliderNibble.transform.localScale = new Vector3(1, 1, 1);
            } 
            else if (_movementDirection.x < 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
                _colliderNibble.transform.localScale = new Vector3(-1, 1, 1);
            } 

            _movementDirection = directionX;
        }
        #endregion

        #region Attack
        [Header("Attack Settings")]
        [SerializeField] private GameObject _sporesPrefabs;
        [SerializeField] private float _sporesCooldown = 1f;
        [SerializeField] private GameObject _colliderNibble;
        [SerializeField] private GameObject _colliderClap;
        [SerializeField] private int _sporesCount = 3;
        bool _attackSpores, _attackClap, _attackNibble;
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
                    StartCoroutine(ClapAttack());
                    break;
                case 2:
                    StartCoroutine(NibbleAttack());
                    break;
            }
        }

        private IEnumerator SporesAttack()
        {
            _attackSpores = true;
            ChangeState(EnemyStates.Idle);
            Instantiate(_sporesPrefabs, this.transform.position, Quaternion.identity);
            yield return Helpers.GetWait(_attackSpoeresAnimTime);
            ChangeState(EnemyStates.Target);
        }

        private IEnumerator ClapAttack()
        {
            _attackClap = true;
            ChangeState(EnemyStates.Idle);
            var players = Physics2D.OverlapBoxAll(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0, _playerLayer);

            _colliderClap.GetComponent<Collider2D>().enabled = true;
            foreach (var player in players)
            {
                var playerBase = player.GetComponent<PlayerBase>();
                if (playerBase != null) playerBase.TakeDamage(_damage);
            }
            yield return Helpers.GetWait(_attackClapAnimTime + 0.5f);
            _colliderClap.GetComponent<Collider2D>().enabled = false;
            ChangeState(EnemyStates.Target);
        }

        private IEnumerator NibbleAttack()
        {
            _attackNibble = true;
            ChangeState(EnemyStates.Idle);
            var players = Physics2D.OverlapBoxAll(_boxCollider2D.bounds.center, _boxCollider2D.bounds.size, 0, _playerLayer);

            foreach (var player in players)
            {
                _colliderNibble.GetComponent<Collider2D>().enabled = true;
                var playerBase = player.GetComponent<PlayerBase>();
                if (playerBase != null) playerBase.TakeDamage(_damage);
            }
            yield return Helpers.GetWait(_attackNibbleAnimTime + 0.5f);
            _colliderNibble.GetComponent<Collider2D>().enabled = false;
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
            private static readonly int AttackClaps = Animator.StringToHash("AttackClaps");
            private static readonly int AttackNibble = Animator.StringToHash("AttackNibble");
            bool _idleAnim;

            [Header("Animation Settings")]
            [SerializeField] private float _attackSpoeresAnimTime = 0.5f;
            [SerializeField] private float _attackClapAnimTime = 0.5f;
            [SerializeField] private float _attackNibbleAnimTime = 0.5f;

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
                    if (_attackSpores) return LockState(AttackSpores, _attackSpoeresAnimTime);
                    if (_attackClap) return LockState(AttackClaps, _attackClapAnimTime);
                    if (_attackNibble) return LockState(AttackNibble, _attackNibbleAnimTime);
                    if (_isStunned) return LockState(StunnedAnim, 5);
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

                void ResetFlags() { _attackSpores = _attackClap = _attackNibble = false;}
            }
        #endregion
    }
}