using UnityEngine;

namespace rene_roid_enemy
{
    public class HorizontalEnemy : EnemyBase
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
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _attackRangeDistance) ChangeState(EnemyStates.Move);
                    if (!_isStunned) AttackRange();
                    break;
                case EnemyStates.Stun:
                    StunUpdate();
                    break;
                case EnemyStates.Target:
                    if (!TargetPlayer()) UnTargetPlayer();
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _targetDistanceUnfollow) ChangeState(EnemyStates.Move);
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance) ChangeState(EnemyStates.Attack);
                    if (!_isStunned) FollowerPlayer();
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

            if (_isGround) _movementDirection = _movementDirection * -1;
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
        [Header("Target Player Settings")]
        [SerializeField] private float _targetDistanceWatchin = 10f;
        [SerializeField] private float _targetDistanceNotWatchin = 5f;
        [SerializeField] private float _targetDistanceUnfollow = 50f;
        float _timeUnTarget = 0;

        private bool TargetPlayer()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            bool isRange = Vector3.Distance(transform.position, _targetPlayer.transform.position) <= (watchin ? _targetDistanceWatchin : _targetDistanceNotWatchin);
            if (_hitTarget.collider != null && _hitTarget.collider.gameObject.tag == "Player" && isRange) return true;
            else return false;
        }

        private void UnTargetPlayer()
        {
            _timeUnTarget += Time.deltaTime * 0.5f;

            if (_timeUnTarget < 3)
            {
                if (_hitTarget.collider != null && _hitTarget.collider.gameObject.tag == "Player") { _timeUnTarget = 0; return;};
                if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _targetDistanceUnfollow) { _timeUnTarget = 0; return;};
            }
            else if (_timeUnTarget >= 1) { _timeUnTarget = 0; ChangeState(EnemyStates.Move); }
        }

        private void FollowerPlayer()
        {
            Vector3 directionX = (_targetPlayer.transform.position.x - this.transform.position.x) > 0 ? Vector3.right : Vector3.left;
            if (!_isGround && !_walled) transform.Translate(directionX * _movementSpeed * _movementSpeedMultiplier * Time.deltaTime);
            _movementDirection = directionX;
            Vertical();
        }
        #endregion
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var p = (_targetPlayer.transform.position - this.transform.position).normalized;
            bool watchin = (p.x > 0 && _movementDirection.x > 0) || (p.x < 0 && _movementDirection.x < 0);
            bool isRange = Vector3.Distance(transform.position, _targetPlayer.transform.position) < (watchin ? _targetDistanceWatchin : _targetDistanceNotWatchin);

            Gizmos.color = isRange ? Color.blue : Color.green;
            Gizmos.DrawWireSphere(transform.position, _targetDistanceNotWatchin);
            Gizmos.DrawWireSphere(transform.position, (isRange ? _targetDistanceWatchin : _targetDistanceNotWatchin));

            Vector2 dir = _movementDirection;
            Vector2 dir2 = p;

            Gizmos.color = isRange ? Color.blue : Color.green;
            Gizmos.DrawLine((Vector2)transform.position, (dir) * (isRange ? _targetDistanceWatchin : _targetDistanceNotWatchin) + (Vector2)transform.position);
            Gizmos.DrawLine((Vector2)transform.position, (dir2) * (isRange ? _targetDistanceWatchin : _targetDistanceNotWatchin) + (Vector2)transform.position);

            Gizmos.color = Color.white;
            Gizmos.DrawRay((Vector2)transform.position + new Vector2((_movementDirection.x > 0 ? 1 : -1), 0), Vector2.down * 5);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _attackRangeDistance);
        }
#endif
    }
}