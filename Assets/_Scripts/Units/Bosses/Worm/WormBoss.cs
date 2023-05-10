using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_enemy
{
    public class WormBoss : EnemyBase
    {
        public override void Awake()
        {
            base.Awake();
            _enemyState = EnemyStates.Attack;

            _boundsMin = _mixPref.transform.position;
            _boundsMax = _maxPref.transform.position;


            GetNextPosition();
        }

        public override void Start()
        {
            base.Start();
            GenerateBody();
        }

        private float _damageCD = 0.5f;
        private float _damageTimer = 0f;
        public override void UpdateState()
        {
            if (_damageTimer > 0)
            {
                _damageTimer -= Time.deltaTime;
            } else
            {
                // Overlap box and detect player
                Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.tag == "Player")
                    {
                        _targetPlayer.TakeDamage(_damage);
                        _damageTimer = 0.5f;
                    }
                }
            }

            WormMovement();

            RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.up, 1f);
            if (ray != null && ray.collider != null) {
                if (ray.collider.tag == "Untagged")
                {
                    // Deactivate istrigger
                    this.GetComponent<BoxCollider2D>().isTrigger = true;
                }
            }
        }

        #region Movement
        [Header("Movement")]
        [SerializeField] private Vector2 _nextPosition;
        private Vector2 _startPosition;
        private Vector2 _finalPosition;
        private Vector2 _middlePoint;
        private float _time;
        private float _timeLeft = 0;
        private bool _fiumm = false;
        [SerializeField] private GameObject _mixPref;
        [SerializeField] private GameObject _maxPref;
        [SerializeField] private Vector2 _boundsMin;
        [SerializeField] private Vector2 _boundsMax;
        private Vector2 _direction;
        private bool _inBounds;

        [SerializeField] private bool _isSmol = false;
        private void WormMovement()
        {
            if (_targetPlayer == null) return;

            if (_isSmol) {
                _boundsMin = new Vector2(_targetPlayer.transform.position.x - 10, _targetPlayer.transform.position.y - 10);
                _boundsMax = new Vector2(_targetPlayer.transform.position.x + 10, _targetPlayer.transform.position.y + 10);
            }

            // Do the bezier curve movement and when it's done, go forward until the enemy is out of bounds
            if (!_fiumm)
            {
                var t = Time.deltaTime / _time;
                _timeLeft += t;

                var pos = CalculateBezierCuadraticPoint(_startPosition, _middlePoint, _finalPosition, _timeLeft);
                transform.position = Vector2.Lerp(transform.position, pos, 0.2f);

                if (_timeLeft >= 1) _fiumm = true;
            }
            else
            {
                // Move forward
                transform.position += (Vector3)_direction * _movementSpeed * Time.deltaTime;
            }

            // Check if the enemy is in bounds
            _inBounds = transform.position.x > _boundsMin.x && transform.position.x < _boundsMax.x &&
                        transform.position.y > _boundsMin.y && transform.position.y < _boundsMax.y;

            // If the enemy is not in bounds, get the next position
            if (!_inBounds && _fiumm) GetNextPosition();
        }

        private void GetNextPosition()
        {
            if (_targetPlayer == null) return;
            var dir = _targetPlayer.transform.position - transform.position;
            _direction = dir.normalized;

            _fiumm = false;
            _timeLeft = 0;

            transform.rotation = Quaternion.LookRotation(Vector3.forward, _direction);
            _startPosition = transform.position;
            _finalPosition = _targetPlayer.transform.position;
            _middlePoint = GetMiddlePoint(_startPosition, _finalPosition);

            // Get total distance from bezier curve
            var distance = Vector2.Distance(_startPosition, _middlePoint) + Vector2.Distance(_middlePoint, _finalPosition);

            // Get time to move the distance
            _time = CalculateTime(distance, _movementSpeed);
        }

        private void OnDrawGizmos() {
            // Draw a line following the bezier curve
            Gizmos.color = Color.red;
            var p0 = _startPosition;
            var p1 = _middlePoint;
            var p2 = _finalPosition;
            for (var i = 0; i < 100; i++)
            {
                var t = i / 100f;
                var u = 1 - t;
                var tt = t * t;
                var uu = u * u;

                var p = (Vector2)(uu * p0);
                p += 2 * u * t * p1;
                p += (Vector2)(tt * p2);

                Gizmos.DrawSphere(p, 0.1f);
            }
        }

        private Vector2 CalculateBezierCuadraticPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;

            var p = uu * p0;
            p += 2 * u * t * p1;
            p += tt * p2;

            // Get the direction of the curve
            var dir = p - (Vector2)transform.position;
            dir.Normalize();

            transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
            _direction = dir;

            return p;
        }

        private Vector2 GetMiddlePoint(Vector2 p0, Vector2 p1)
        {
            var middlePoint = (p0 + p1) / 2;
            var dir = p1 - p0;

            // Move dir 90 degrees clockwise or counterclockwise
            var temp = dir.x;
            dir.x = -dir.y;
            dir.y = temp;

            // Normalize dir
            dir.Normalize();

            // Get distance between p0 and p1
            var distance = Vector2.Distance(p0, p1);

            // Move middlePoint in the direction of dir by distance
            middlePoint += dir * distance;

            return middlePoint;
        }

        private float CalculateTime(float distance, float speed)
        {
            return distance / speed;
        }

        #endregion

        #region Worm Body
        [Header("Worm Body")]
        [SerializeField] private GameObject _wormBodyPrefab;
        [SerializeField] private int _wormBodyCount = 10;

        private List<WormBody> _wormBodyParts = new List<WormBody>();

        private void GenerateBody() {
            for (var i = 0; i < _wormBodyCount; i++) {
                var body = Instantiate(_wormBodyPrefab, transform.position, Quaternion.identity);
                _wormBodyParts.Add(body.GetComponent<WormBody>());
                body.GetComponent<WormBody>().Worm = this;
                if (i == 0) {
                    body.GetComponent<WormBody>().Target = this.transform.GetChild(1);
                    continue;
                }
                var firstChild = _wormBodyParts[i - 1].transform.GetChild(2);
                body.GetComponent<WormBody>().Target = firstChild.transform;
            }
        }
        #endregion

        public float GetWormSpeed() {
            return _movementSpeed;
        }

        private void OnTriggerExit2D(Collider2D other) {
            if (other.gameObject.tag == "Untagged") {
                // Deactivate is trigger
                this.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }

        public override void TakeDamage(float damage)
        {
            if (_armor > 0) damage *= 100 / (100 + _armor);
            if (_armor < 0) damage *= 2 - 100 / (100 - _armor);

            _health -= damage;
            _targetPlayer.OnEnemyHit(damage, this);

            Debug.Log("Enemy health: " + _health);

            if (_health <= 0)
            {
                for (int i = 0; i < _wormBodyParts.Count; i++)
                {
                    Destroy(_wormBodyParts[i].gameObject);
                }
                _health = 0;
                // this.gameObject.SetActive(false);
                Destroy(this.gameObject);
                _targetPlayer.OnEnemyDeath(damage, this);
                return;
            }
        }
    }
}
