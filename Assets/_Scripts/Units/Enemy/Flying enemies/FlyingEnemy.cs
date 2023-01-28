using UnityEngine;

namespace rene_roid_enemy
{
    public class FlyingEnemy : EnemyBase
    {
        #region Internal
        private Transform _target;
        #endregion

        #region External

        #endregion

        public override void Start()
        {
            base.Start();
            _target = _targetPlayer.transform;
        }

        public override void Update()
        {
            base.Update();
        }

        #region State Machine
        public override void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    ChangeState(EnemyStates.Target);
                    break;
                case EnemyStates.Attack:
                    FollowPlayer();
                    Attack();

                    if (Vector2.Distance(transform.position, _target.position) > 2f) ChangeState(EnemyStates.Target);
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
                    FollowPlayer();

                    if (Vector2.Distance(transform.position, _target.position) < 2f) ChangeState(EnemyStates.Attack);
                    break;
                case EnemyStates.KnockBack:
                    KnockUpdate();

                    if (_knockBackTime < Time.time)
                        ChangeState(EnemyStates.Move);
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
                case EnemyStates.KnockBack:
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
                case EnemyStates.Target:
                    break;
                case EnemyStates.KnockBack:
                    _knockBackTime = Time.time + _knockBackDuration;
                    break;
            }

            _enemyState = newState;
        }
        #endregion

        #region Movement
        private float _rotSpeed = 2f;
        private float _knockBackTime = 0;
        private void FollowPlayer()
        {
            if (_target == null) return;
            if (_isStunned) return;

            var dir = _target.position - transform.position;
            var currRotSpeed = _rotSpeed;

            // Follow the dir using torque
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            var q = Quaternion.AngleAxis(angle, Vector3.forward);

            if (Vector2.Distance(transform.position, _target.position) > 2f) currRotSpeed = _rotSpeed;
            else currRotSpeed = _rotSpeed * 2f;

            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * currRotSpeed);

            // Move forward
            transform.Translate(Vector3.up * Time.deltaTime * _movementSpeed);
        }

        private void KnockUpdate()
        {
            if (_target == null) return;
            if (_isStunned) return;

            // Push to the opposite direction of the target and rotate to the target direction instantly
            var dir = _target.position - transform.position;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            var q = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * _rotSpeed * 10f);
            transform.Translate(-Vector3.up * Time.deltaTime * _knockBackForce);


        }
        #endregion

        #region Attack
        [Header("Attack")]
        [SerializeField] private float _attackRange = 1.5f;
        [SerializeField] private float _attackDamage = 1f;
        [SerializeField] private float _attackCooldown = 3.5f;
        private float _attackCooldownTimer = 0f;
        private bool _isAttacking = false;

        private void Attack()
        {
            if (_target == null) return;
            if (_isStunned) return;
            if (Vector2.Distance(transform.position, _target.position) < _attackRange)
            {
                if (!_isAttacking)
                {
                    _isAttacking = true;
                    _attackCooldownTimer = Time.time + _attackCooldown;
                    _targetPlayer.TakeDamage(DealDamage(_attackDamage));
                }
                else
                {
                    if (_attackCooldownTimer < Time.time)
                    {
                        _isAttacking = false;
                    }
                }
            }
            else
            {
                _isAttacking = false;
            }
        }
        #endregion

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}
