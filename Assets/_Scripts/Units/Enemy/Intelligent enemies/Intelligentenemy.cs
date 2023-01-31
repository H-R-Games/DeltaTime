using UnityEngine;
using Pathfinding;

namespace rene_roid_enemy
{
    public class Intelligentenemy : EnemyBase
    {
        public override void Start()
        {
            base.Start();

            _seeker = GetComponent<Seeker>();
            _rb = GetComponent<Rigidbody2D>();

            InvokeRepeating("UpdatePath", 0f, _pathUpdateSeconds);

            _enemyState = EnemyStates.Move;
        }

        public override void Update()
        {
            UpdateState();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        #region State Machine
        public override void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    if (TargetInDistance() && _followEnabled && !_isStunned) PathFollow();
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) <= _attackRangeDistance) ChangeState(EnemyStates.Attack);
                    break;
                case EnemyStates.Attack:
                    if (Vector3.Distance(transform.position, _targetPlayer.transform.position) > _attackRangeDistance) ChangeState(EnemyStates.Move);
                    if (!_isStunned) AttackRange();
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
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

        #region Pathfinding
        [Header("Pathfinding Settings")]
        [SerializeField] private float _acivatePathfindingDistance = 50f;
        [SerializeField] private float _pathUpdateSeconds = 0.5f;
        [SerializeField] private float _nextWaypointDistance = 3f;
        [SerializeField] private float _speed = 200f;
        [SerializeField] private float _maxSpeed = 10f;
        [SerializeField] private float _jumpHeightRequired = 1f;
        [SerializeField] private float _jumpModifier = 1f;
        [SerializeField] private float _jumpCheckOffset = 0.5f;
        bool _followEnabled = true;
        bool _jumpEnabled = true;
        bool _directionLookEnabled = true;
        int _currentWaypoint = 0;
        Path _path;
        Seeker _seeker;
        Rigidbody2D _rb;
        RaycastHit2D _isGrounded;

        private void UpdatePath() { if (TargetInDistance() && _followEnabled) _seeker.StartPath(transform.position, _targetPlayer.transform.position, OnPathComplete); }

        private void PathFollow()
        {
            if (_path == null) return;

            if (_currentWaypoint >= _path.vectorPath.Count) return;

            Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
            Vector2 force = direction * _speed * Time.deltaTime * 100;
            Vector3 startOffset = transform.position - new Vector3(0f, GetComponent<Collider2D>().bounds.extents.y + _jumpCheckOffset);

            _isGrounded = Physics2D.Raycast(startOffset, -Vector3.up, 0.05f);

            if (_jumpEnabled && _isGrounded && force.y > _jumpHeightRequired) _rb.velocity = new Vector2(_rb.velocity.x, force.y * _jumpModifier);

            _rb.velocity = new Vector2(force.x, _rb.velocity.y);

            float distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
            if (distance < _nextWaypointDistance) _currentWaypoint++;

            if (_directionLookEnabled) LookDirection(direction);
        }

        private void LookDirection(Vector2 direction)
        {
            if (_rb.velocity.x > 0.05f) transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            else if (_rb.velocity.x < -0.05f) transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

            _rb.velocity = new Vector2(Mathf.Clamp(_rb.velocity.x, -_maxSpeed, _maxSpeed), Mathf.Clamp(_rb.velocity.y, -_maxSpeed, _maxSpeed));
        }

        private bool TargetInDistance()
        {
            if (_targetPlayer == null) return false;
            if (Vector2.Distance(_rb.position, _targetPlayer.transform.position) < _acivatePathfindingDistance) return true;
            return false;
        }

        private void OnPathComplete(Path p)
        {
            if (!p.error)
            {
                _path = p;
                _currentWaypoint = 0;
            }
        }
        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {

        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackRangeDistance);
        }
#endif
    }
}