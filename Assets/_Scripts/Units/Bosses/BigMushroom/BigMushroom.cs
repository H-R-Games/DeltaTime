using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;

namespace rene_roid_enemy
{
    public class BigMushroom : EnemyBase
    {
        public override void Start()
        {
            base.Start();
            ChangeState(EnemyStates.Target);
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
                    GravityEnemy();
                    if (!_isStunned) FollowerPlayer();
                    if (!_isStunned) AttackController();
                    break;
                case EnemyStates.Stun:
                    GravityEnemy();
                    StunUpdate();
                    break;
                case EnemyStates.Target:
                    GravityEnemy();
                    if (!_isStunned) FollowerPlayer();
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance) ChangeState(EnemyStates.Attack);
                    break;
            }
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
        #region Target Player
        private void FollowerPlayer()
        {
            Vector3 directionX = (_targetPlayer.transform.position.x - this.transform.position.x) > 0 ? Vector3.right : Vector3.left;

            if (Vector3.Distance(-new Vector3(0, transform.position.y, 0), new Vector3(0, _targetPlayer.transform.position.y, 0)) > 3) Jump();

            if (!_isGround && !_walled) transform.Translate(directionX * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);

            _movementDirection = directionX;
        }
        #endregion
        #endregion

        #region Attacks
        [SerializeField] private GameObject _sporesPrefab;
        float _timeAwait = 0;
        float _sporesTimer = 0;

        private void AttackController()
        {
            if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _attackRangeDistance) ChangeState(EnemyStates.Target);

            if (_timeAwait <= 0)
            {
                int attack = Random.Range(0, 3);
                switch (attack)
                {
                    case 0:
                        AttackNibble();
                        break;
                    case 1:
                        if (_sporesTimer <= 0) AttackSpores();
                        break;
                    case 2:
                        AttackClaps();
                        break;
                }
                _timeAwait = _attackRangeCooldown;
            }
            else _timeAwait -= Time.deltaTime;
            _sporesTimer -= Time.deltaTime;
        }

        private void AttackNibble()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);

            if (!watchin) return;
            if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance / 2 && watchin) _targetPlayer.GetComponent<PlayerBase>().TakeDamage(_damage);
        }

        private void AttackSpores()
        {
            _sporesTimer = 8;
            if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance) Instantiate(_sporesPrefab, transform.position, Quaternion.identity);
        }

        private void AttackClaps()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance / 1.5f) _targetPlayer.GetComponent<PlayerBase>().TakeDamage(_damage);
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _attackRangeDistance);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackRangeDistance / 2);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _attackRangeDistance / 1.5f);
        }
#endif
    }
}
