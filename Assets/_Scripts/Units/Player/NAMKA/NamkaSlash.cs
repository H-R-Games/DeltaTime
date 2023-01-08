using UnityEngine;
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

        private bool _xFlip;

        void Start()
        {
            // Destroy the slash after a certain amount of time
            Destroy(gameObject, _lifeTime);
            _xFlip = this.GetComponent<SpriteRenderer>().flipX;
        }

        void Update()
        {
            Move();
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
