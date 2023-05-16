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
            _colliderNibble = transform.GetChild(1).GetComponent<BoxCollider2D>();
            _colliderClap = transform.GetChild(2).GetComponent<BoxCollider2D>();
            _colliderNibble.GetComponent<BoxCollider2D>().enabled = false;
            _colliderClap.GetComponent<BoxCollider2D>().enabled = false;
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
            } 
            else if (_movementDirection.x < 0 && Vector3.Distance(new Vector3(_targetPlayer.transform.position.x, 0, 0), new Vector3(this.transform.position.x, 0, 0)) > 0.5f)
            {
                this.gameObject.GetComponentInChildren<SpriteRenderer>().flipX = _movementDirection.x < 0;
            } 

            _movementDirection = directionX;
        }
        #endregion

        #region Attack
        [Header("Attack Settings")]
        [SerializeField] private GameObject _sporesPrefabs;
        [SerializeField] private float _sporesCooldown = 1f;
        [SerializeField] private BoxCollider2D _colliderNibble;
        [SerializeField] private BoxCollider2D _colliderClap;
        [SerializeField] private int _sporesCount = 3;
        bool _attackSpores, _attackClap, _attackNibble;
        float _timerSpores;

        private void SetAttack()
        {
            int num = Random.Range(0, 3);

            switch (num)
            {
                case 0:
                    if (_timerSpores > Time.deltaTime) 
                    {
                        _timerSpores = 0;
                        StartCoroutine(SporesAttack());
                    } else  _timerSpores += Time.deltaTime * 0.5f;
                    break;
                case 1:
                    StartCoroutine(ClapAttack());
                    SetAttack();
                    break;
                case 2:
                    StartCoroutine(NibbleAttack());
                    SetAttack();
                    break;
            }
        }

        private IEnumerator SporesAttack()
        {
            if (_attackSpores) yield break;
            _attackSpores = true;
            _attackSporesAnim = true;
            ChangeState(EnemyStates.Idle);
            Instantiate(_sporesPrefabs, this.transform.position, Quaternion.identity);
            yield return Helpers.GetWait(_attackSpoeresAnimTime);
            _attackSpores = false;
            ChangeState(EnemyStates.Target);
        }

        private IEnumerator ClapAttack()
        {
            if (_attackClap) yield break;
            _attackClap = true;
            _attackClapAnim = true;
            ChangeState(EnemyStates.Idle);

            _colliderClap.GetComponent<Transform>().transform.position = transform.position;
            _colliderClap.GetComponent<Transform>().transform.Translate(new Vector3(_movementDirection.x > 0 ? 1.5f : -1.5f, 0, 0));

            yield return Helpers.GetWait(0.5f);

            _colliderClap.enabled = true;

            var players = Physics2D.OverlapBoxAll(_colliderClap.bounds.center, _colliderClap.bounds.size, 0, _playerLayer);
            
            foreach (var player in players)
            {
                var playerBase = player.GetComponent<PlayerBase>();
                if (playerBase != null) playerBase.TakeDamage(_damage);
            }
            
            yield return Helpers.GetWait(_attackClapAnimTime + 0.5f);
            
            _colliderClap.enabled = false;
            _attackClap = false;
            ChangeState(EnemyStates.Target);
        }

        private IEnumerator NibbleAttack()
        {
            if (_attackNibble) yield break;
            _attackNibble = true;
            _attackNibbleAnim = true;
            ChangeState(EnemyStates.Idle);

            _colliderNibble.GetComponent<Transform>().transform.position = transform.position;
            _colliderNibble.GetComponent<Transform>().transform.Translate(new Vector3(_movementDirection.x > 0 ? 1.5f : -1.5f, 0, 0));

            yield return Helpers.GetWait(0.5f);

            _colliderNibble.enabled = true;

            var players = Physics2D.OverlapBoxAll(_colliderNibble.bounds.center, _colliderNibble.bounds.size, 0, _playerLayer);

            foreach (var player in players)
            {
                var playerBase = player.GetComponent<PlayerBase>();
                if (playerBase != null) playerBase.TakeDamage(_damage);
            }

            yield return Helpers.GetWait(_attackNibbleAnimTime + 0.5f);
            
            _colliderNibble.enabled = false;
            _attackNibble = false;
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
            bool _attackSporesAnim;
            bool _attackClapAnim;
            bool _attackNibbleAnim;

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
                    if (_attackSporesAnim) return LockState(AttackSpores, _attackSpoeresAnimTime);
                    if (_attackClapAnim) return LockState(AttackClaps, _attackClapAnimTime);
                    if (_attackNibbleAnim) return LockState(AttackNibble, _attackNibbleAnimTime);
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

                void ResetFlags() { _attackSporesAnim = _attackClapAnim = _attackNibbleAnim = false;}
            }
        #endregion
    }
}