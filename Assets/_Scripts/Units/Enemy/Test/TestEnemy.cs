using rene_roid_player;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace rene_roid_enemy
{
    public class TestEnemy : EnemyBase
    {
        public override void Start()
        {
            base.Start();
            ChangeState(EnemyStates.Move);
        }

        public override void Update()
        {
            UpdateState();
        }

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
                    break;
                case EnemyStates.Stun:
                    StunUpdate();
                    break;
                case EnemyStates.Target:
                    FollowerPlayer();
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
        private void HorizontalEnemyMovement()
        {
            if (!_isStunned) Horizontal();
            Vertical();
        }

        #region Horeizontal
        private void Horizontal()
        {
            Vector3 direction = new Vector3(_movementDirection.x, 0, 0);

            if (_walled) _movementDirection = _movementDirection * -1;
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

        #region Target Player
        private bool TargetPlayer()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            return Vector3.Distance(transform.position, _targetPlayer.transform.position) < (watchin ? 10 : 5);
        }

        private void FollowerPlayer()
        {
            if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > 50) ChangeState(EnemyStates.Move);

            float directionX = (_targetPlayer.transform.position.x - this.transform.position.x);

            transform.Translate(new Vector3(directionX, 0, 0) * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);
        }
        #endregion
        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 500, 100), "Stun"))
            {
                ChangeState(EnemyStates.Target);
            }
        }

        private void OnDrawGizmos()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            bool isRange = Vector3.Distance(transform.position, _targetPlayer.transform.position) < (watchin ? 10 : 5);


            Gizmos.DrawWireSphere(transform.position, .5f);
            Gizmos.DrawWireSphere(transform.position, (isRange ? 10 : 5));

            // add 15 degrees to _directionJoystick
            Vector2 dir = _movementDirection;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            //angle += _aimAssistAngle;
            //dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            // remove 15 degrees to _directionJoystick
            Vector2 dir2 = p;
            float angle2 = Mathf.Atan2(dir2.y, dir2.x) * Mathf.Rad2Deg;
            //angle2 -= _aimAssistAngle;
            //dir2 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad));

            Gizmos.DrawLine((Vector2)transform.position, (dir) * (isRange ? 10 : 5) + (Vector2)transform.position);
            Gizmos.DrawLine((Vector2)transform.position, (dir2) * (isRange ? 10 : 5) + (Vector2)transform.position);
        }
#endif
    }
}