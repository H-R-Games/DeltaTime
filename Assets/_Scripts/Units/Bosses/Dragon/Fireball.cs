using UnityEngine;
using rene_roid_player;

namespace rene_roid_enemy {
    [RequireComponent(typeof(BoxCollider2D))]
    public class Fireball : MonoBehaviour
    {
        public struct FireballStatsStruct
        {
            public float speed;
            public float damage;

            public FireballStatsStruct(float speed, float damage)
            {
                this.speed = speed;
                this.damage = damage;
            }
        }

        public FireballStatsStruct FireballStats { get; set; }

        private Transform _playerTransform;

        public Transform PlayerTransform { get => _playerTransform; set => _playerTransform = value; }

        private Vector2 _startPos;
        private Vector2 _finalPos;
        private Vector2 _middlePoint;
        [SerializeField] private float _time;
        private float _timer;

        void Start()
        {
            var randomness = Random.Range(-3f, 3f);
            _startPos = transform.position;
            _finalPos = _playerTransform.position + new Vector3(0, 0.5f, 0) + new Vector3(randomness, 0, 0);
            _middlePoint = (_startPos + _finalPos) / 2;
            _middlePoint.y += 15+ randomness;
            // _time = Vector2.Distance(_startPos, _finalPos) / FireballStats.speed;

            Destroy(gameObject, 3);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerBase>().TakeDamage(FireballStats.damage);
                Destroy(gameObject);
            }
        }
        
        void Update()
        {
            _timer += Time.deltaTime / _time;

            transform.position = CalculateBezierCuadraticPoint(_startPos, _middlePoint, _finalPos, _timer);

            //
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

            return p;
        }

        private Vector2 CalculateBezierCuadraticPoint4(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) 
        {
            var u = 1 - t;
            var tt = t * t;
            var uu = u * u;
            var uuu = uu * u;
            var ttt = tt * t;

            var p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;

            // Get the direction of the curve
            var dir = p - (Vector2)transform.position;
            dir.Normalize();

            return p;
        }
    }
}
