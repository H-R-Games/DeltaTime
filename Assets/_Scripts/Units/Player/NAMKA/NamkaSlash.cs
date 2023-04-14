using UnityEngine;
using System.Collections.Generic;
using rene_roid_enemy;
using rene_roid_player;

namespace rene_roid {
    public class NamkaSlash : MonoBehaviour
    {
        [SerializeField] private Namka _namka;

        [SerializeField] private LayerMask _enemyLayer;
        [SerializeField] private float _damagePercentage = 1;
        [SerializeField] private float _procCo = 1f;

        [SerializeField] private float _speed = 10f;
        [SerializeField] private float _lifeTime = 2f;
        [SerializeField] private int _maxHits = 3;
        private BoxCollider2D _boxCollider2D;
        private List<GameObject> _enemiesHit = new List<GameObject>();

        private bool _xFlip;

        void Start()
        {
            // Destroy the slash after a certain amount of time
            Destroy(gameObject, _lifeTime);
            _xFlip = this.GetComponent<SpriteRenderer>().flipX;
            _boxCollider2D = GetComponent<BoxCollider2D>();
        }

        void Update()
        {
            Move();
        }

        private void FixedUpdate() {
            if (_boxCollider2D == null) return;
            if (_namka == null) return;
            if (_maxHits <= 0) return;
            var enemies = Physics2D.OverlapBoxAll(transform.position, _boxCollider2D.size, 0, _enemyLayer);

            foreach (var enemy in enemies) {
                if (!_enemiesHit.Contains(enemy.gameObject)) {
                    var enemyBase = enemy.GetComponent<EnemyBase>();
                    if (enemyBase != null) enemyBase.TakeDamage(_namka.DealDamage(_damagePercentage, _procCo));
                    print("Hit!: " + enemy.gameObject.name);
                    _enemiesHit.Add(enemy.gameObject);

                    // Destroy the slash after a certain amount of hits
                    _maxHits--;
                    if (_maxHits <= 0) Destroy(gameObject, .25f);
                }
            }
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                var enemy = other.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(_namka.DealDamage(_damagePercentage, _procCo));
                print("Hit!: " + enemy.gameObject.name);

                // Destroy the slash after a certain amount of hits
                _maxHits--;
                if (_maxHits <= 0) Destroy(gameObject, .25f);
            }
        }

        private void Move()
        {
            // Move the slash to the right or left depending on the direction the player is facing
            transform.Translate(Vector2.right * _speed * Time.deltaTime * (_xFlip ? -1 : 1));
        }

        public void SetFlipX(bool flipX)
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.flipX = flipX;
        }

        public void SetSlashInfo(Namka namka, float damagePercentage, float procCo, float speed)
        {
            _namka = namka;
            _damagePercentage = damagePercentage;
            _procCo = procCo;
            _speed = speed;
        }
    }
}
