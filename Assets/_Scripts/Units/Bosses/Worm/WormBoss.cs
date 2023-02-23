using UnityEngine;

namespace rene_roid_enemy
{
    public class WormBoss : EnemyBase
    {
        public override void Awake()
        {
            base.Awake();
            _enemyState = EnemyStates.Attack;
            GetNextPosition();
        }

        public override void UpdateState()
        {
            WormMovement();
        }

        #region Movement
        [Header("Movement")]
        [SerializeField] private Vector2 _nextPosition;
        [SerializeField] private Vector2 _boundsMin;
        [SerializeField] private Vector2 _boundsMax;
        private Vector2 _direction;
        private bool _inBounds;
        private void WormMovement()
        {
            if (_targetPlayer == null) return;

            // Move the enemy forward
            transform.position += (Vector3)_direction * _movementSpeed * Time.deltaTime;

            // Check if the enemy is in bounds
            _inBounds = transform.position.x > _boundsMin.x && transform.position.x < _boundsMax.x &&
                        transform.position.y > _boundsMin.y && transform.position.y < _boundsMax.y;

            // If the enemy is not in bounds, get the next position
            if (!_inBounds) GetNextPosition();
        }

        private void GetNextPosition()
        {
            if (_targetPlayer == null) return;
            var dir = _targetPlayer.transform.position - transform.position;
            _direction = dir.normalized;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
        }

        #endregion
    }
}
