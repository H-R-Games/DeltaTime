using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using rene_roid_enemy;

namespace rene_roid_player {    
    public class ShadowClone : MonoBehaviour
    {
        #region Internal
        private SpriteRenderer _renderer;
        private Sprite _sprite;
        private Vector2 _direction;
        private float _damage;
        private float _proc;
        private PlayerBase _player;
        private bool _oni = false;
        #endregion

        #region External
        [Header("External")]
        public Sprite ShadowCloneSprite1;
        public Sprite ShadowCloneSprite2;

        public bool Clone = false;
        public Transform ClonePoint;
        #endregion

        private void Start() {
            _renderer = GetComponent<SpriteRenderer>();

            if (Clone) {
                DashClone();
            }
        }

        private List<EnemyBase> _enemiesHit = new List<EnemyBase>();
        private void Update() {
            // Overlap box all
            var enemies = Physics2D.OverlapBoxAll(transform.position, new Vector2(1, 1), 0, LayerMask.GetMask("Enemy"));
            foreach (var enemy in enemies) {
                var hit = enemy.gameObject.GetComponent<EnemyBase>();
                if (hit != null && !_enemiesHit.Contains(hit)) {
                    hit.TakeDamage(_player.DealDamage(_damage, _proc));
                    _enemiesHit.Add(hit);
                }

                if (_oni && !Clone && hit != null) {
                    OniClone(hit.transform, _damage, _proc);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss")) {
            //     var enemy = other.gameObject.GetComponent<EnemyBase>();
            //     if (enemy != null) enemy.TakeDamage(_player.DealDamage(_damage, _proc));

            //     if (_oni && !Clone && enemy != null) {
            //         OniClone(enemy.transform, _damage, _proc);
            //     }
            // }
        }

        public void OniClone(Transform point, float perc, float proc) {
            // Instantiate 3 clones in a circle around the point with a radius of 1
            var clone1 = Instantiate(this, point.position + new Vector3(3, 0, 0), Quaternion.identity);
            var clone2 = Instantiate(this, point.position + new Vector3(0, 3, 0), Quaternion.identity);
            var clone3 = Instantiate(this, point.position + new Vector3(-3, 0, 0), Quaternion.identity);

            clone1.Clone = clone2.Clone = clone3.Clone = true;
            clone1.SetPlayer(_player);
            clone2.SetPlayer(_player);
            clone3.SetPlayer(_player);

            ClonePoint = point;
            clone1.ClonePoint = clone2.ClonePoint = clone3.ClonePoint = point;
        }

        private void DashClone() {
            StartCoroutine(IEDashClone(ClonePoint, 10, .5f));
        }

        private IEnumerator IEDashClone(Transform p, float speed, float fadeTime) {
            // Rotate the gameobject to face p (gameobject is looking to the right)
            transform.rotation = Quaternion.LookRotation(Vector3.forward, p.position - transform.position);
            transform.Rotate(0, 0, 90);
            
            _renderer.flipX = false;
            if (transform.rotation.z == 1) _renderer.flipY = true;

            var dir = (p.position - transform.position).normalized;
            var rate = 1.0f / fadeTime;
            var progress = 0.0f;
            float startAlpha = _renderer.color.a;
            _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, .7f);

            while (progress < 1.0f) {
                transform.position += (Vector3)dir * speed * Time.deltaTime;
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(startAlpha, 0, progress));
                progress += rate * Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject, 0.1f);
        }
        

        public void SetFlipX(bool flipX)
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.flipX = flipX;
            _direction = flipX ? Vector2.left : Vector2.right;
        }

        // Dash fast in the direction the player is facing and then fade out
        public void Dash(float speed, float fadeTime, bool oni, PlayerBase player, float perc, float proc)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = ShadowCloneSprite1;

            _player = player;
            _damage = perc;
            _proc = proc;
            _oni = oni;

            StartCoroutine(DashAndFade(speed, fadeTime));
        }

        private IEnumerator DashAndFade(float speed, float fadeTime)
        {
            float startAlpha = _renderer.color.a;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;
            while (progress < 1.0)
            {
                transform.Translate(_direction * speed * Time.deltaTime);
                // _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(startAlpha, 0, progress));
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, .7f);
                progress += rate * Time.deltaTime;


                yield return null;
            }

            // In the end of the dash, change the sprite to the fade out sprite2
            _renderer.sprite = ShadowCloneSprite2;
            var t = 1f;
            var fadet = 0.3f;
            while (t > 0)
            {
                t -= Time.deltaTime / fadet;
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, t);
                yield return null;
            }

            Destroy(gameObject);
        }

        public void SetPlayer(PlayerBase player)
        {
            _player = player;
        }
    }
}
