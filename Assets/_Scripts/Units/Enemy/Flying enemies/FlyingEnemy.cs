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
                    KnockBack(7);
                    break;
                case EnemyStates.Move:
                    ChangeState(EnemyStates.Target);
                    break;
                case EnemyStates.Attack:
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
                    FollowPlayer();
                    break;
                case EnemyStates.KnockBack:
                    KnockUpdate();

                    if (_knockBackDuration < Time.time)
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

        private void KnockUpdate() {
            // Push to the opposite direction of the target
            var dir = transform.position - _target.position;
            transform.Translate(dir.normalized * Time.deltaTime * _knockBackForce);
        }
        #endregion
    }
}
