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
            CreateWormBody();
        }

        public override void UpdateState()
        {
            WormMovement();
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

        private void WormMovement()
        {
            if (_targetPlayer == null) return;

            // Do the bezier curve movement and when it's done, go forward until the enemy is out of bounds
            if (!_fiumm)
            {
                var t = Time.deltaTime / _time;
                _timeLeft += t;

                transform.position = CalculateBezierCuadraticPoint(_startPosition, _middlePoint, _finalPosition, _timeLeft);

                if (_timeLeft >= 1) _fiumm = true;
            }
            else
            {
                // Move forward
                transform.position += (Vector3)_direction * _movementSpeed * Time.deltaTime;
            }

            // Fill array of worm last positions
            _wormPastPositions.Insert(0, transform.position);
            if (_wormPastPositions.Count > _bodyParts) _wormPastPositions.RemoveAt(1000);

            // Fill array of worm last positions time
            _wormPastPositionsTime.Insert(0, Time.deltaTime);
            if (_wormPastPositionsTime.Count > _bodyParts) _wormPastPositionsTime.RemoveAt(1000);
            BodyFollow();

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
        [SerializeField] private int _bodyParts = 10;
        [SerializeField] private float _bodyPartDistance = 1;
        private List<GameObject> _wormBodyParts = new List<GameObject>();
        [SerializeField] private List<Vector2> _wormPastPositions = new List<Vector2>();
        [SerializeField] private List<float> _wormPastPositionsTime = new List<float>();
        [SerializeField] private float _bodyFollowDelay = 0.15f;
        [SerializeField] private float _bodyFirstFollowDelay = 0.15f;

        private void CreateWormBody()
        {
            for (var i = 0; i < _bodyParts; i++)
            {
                var bodyPart = Instantiate(_wormBodyPrefab, transform.position, Quaternion.identity);
                bodyPart.transform.position = transform.position + (Vector3)_direction * _bodyPartDistance * i;
                _wormBodyParts.Add(bodyPart);
                bodyPart.SetActive(true);
            }

            // Fill array of worm last positions
            for (var i = 0; i < 1000; i++)
            {
                _wormPastPositions.Add(transform.position);
                _wormPastPositionsTime.Add(Time.deltaTime);
            }

        }

        private void BodyFollow() {
            // for (int i = 0; i < _wormBodyParts.Count; i++)
            // {
            //     var n = i * 2;
            //     print(n);
            //     var bodyPart = _wormBodyParts[i];
            //     var pastPosition = _wormPastPositions[i + n];

            //     bodyPart.transform.position = Vector2.MoveTowards(bodyPart.transform.position, pastPosition, _movementSpeed * Time.deltaTime);

            //     // Get direction
            //     var dir = pastPosition - _wormPastPositions[i + n + 1];
            //     dir.Normalize();

            //     // Rotate body part
            //     bodyPart.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
            // }

            for (int i = 0; i < _wormBodyParts.Count; i++)
            {
                var bodyPart = _wormBodyParts[i];
                var time_diff = _bodyFollowDelay * i + _bodyFirstFollowDelay;

                var time_pos = 0;
                var t_calc = 0f;
                for (int j = 0; j < _wormPastPositionsTime.Count; j++)
                {
                    t_calc += _wormPastPositionsTime[j];

                    if (t_calc > time_diff)
                    {
                        time_pos = j;
                        break;
                    }
                }

                var pastPosition = _wormPastPositions[(int)time_pos];

                bodyPart.transform.position = Vector2.MoveTowards(bodyPart.transform.position, pastPosition, _movementSpeed * Time.deltaTime);

                // Get direction
                var dir = pastPosition - _wormPastPositions[(int)time_pos + 1];
                dir.Normalize();

                // Rotate body part
                bodyPart.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
            }
        }

        #endregion

    }
}
