using UnityEngine;
using static rene_roid_enemy.EnemyBase;

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
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    HorizontalEnemyMovement();
                    break;
                case EnemyStates.Attack:
                    break;
                case EnemyStates.Stun:
                    StunUpdate();
                    break;
                case EnemyStates.Dead:
                    break;
            }
        }

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
                    StunnStart(5);
                    break;
                case EnemyStates.Dead:
                    break;
            }
            _enemyState = newState;
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 500, 100), "Stun"))
            {
                ChangeState(EnemyStates.Stun);
            }
        }
    }
}
