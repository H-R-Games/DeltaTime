using UnityEngine;

namespace rene_roid_enemy {
    public class TimeSentinel : EnemyBase
    {
        public Transform[] Waypoints;
        private Vector3[] _waypoints;
        private int _currentWaypointIndex = 0;
        private Vector2 _direction;

        public override void Start()
        {
            base.Start();

            _waypoints = new Vector3[Waypoints.Length];

            for (int i = 0; i < Waypoints.Length; i++)
            {
                _waypoints[i] = Waypoints[i].position;
            }
        }

        public override void Update()
        {
            base.Update();

            Movement();
        }

        private void Movement() {
            if (_currentWaypointIndex < _waypoints.Length)
            {
                transform.position = Vector2.MoveTowards(transform.position, _waypoints[_currentWaypointIndex], 5 * Time.deltaTime);
                if (Vector2.Distance(transform.position, _waypoints[_currentWaypointIndex]) < 0.1f)
                {
                    _currentWaypointIndex += 1;
                }
                _direction = (_waypoints[_currentWaypointIndex] - transform.position).normalized;
            }
            else
            {
                _currentWaypointIndex = 0;

                _direction = (_waypoints[_currentWaypointIndex] - transform.position).normalized;
            }

            transform.localScale = new Vector3(_direction.x > 0 ? 1 : -1, 1, 1);
        }

        public override void TakeDamage(float damage)
        {
            base.TakeDamage(damage);

            bool playerHitBehind = PlayerHitBehind();
            if (playerHitBehind) {
                print("Player hit behind");
            }
        }
        
        private bool PlayerHitBehind() {
            Transform player = _targetPlayer.transform;
            Vector2 playerDirection = (player.position - transform.position).normalized;
            
            // See if the player is behind the enemy
            if (playerDirection.x < 0 && _direction.x > 0) {
                return true;
            }
            else if (playerDirection.x > 0 && _direction.x < 0) {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
