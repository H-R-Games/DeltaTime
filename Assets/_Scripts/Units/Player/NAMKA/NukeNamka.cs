using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_player {
    public class NukeNamka : MonoBehaviour
    {
        public float Damage = 10f;

        private float _tickDelay = 0.1f;
        private float _tickTimer = 0f;
        private CircleCollider2D _circleCollider2D;
        public Namka parent;


        private void Start() {
            _circleCollider2D = GetComponent<CircleCollider2D>();
        }

        void Update()
        {
            // make the gameobject scale bigger until it reaches a certain size
            if (transform.localScale.x < 30f) {
                transform.localScale += new Vector3(1f, 1f, 0f) * Time.deltaTime * 20;
            }
            else {
                // destroy the gameobject
                Destroy(gameObject);
            }

            // every tickdelay deal damage to all enemies in range
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= _tickDelay) {
                _tickTimer = 0f;
                var enemies = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x / 2f);
                foreach (var enemy in enemies) {
                    var enemyBase = enemy.GetComponent<rene_roid_enemy.EnemyBase>();
                    if (enemyBase != null) enemyBase.TakeDamage(parent.DealDamage(Damage, 2));
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            // every tickdelay deal damage to all enemies in range
            _tickTimer = 0f;
            var enemies = Physics2D.OverlapCircleAll(transform.position, 1f);
            foreach (var enemy in enemies) {
                var enemyBase = enemy.GetComponent<rene_roid_enemy.EnemyBase>();
                if (enemyBase != null) enemyBase.TakeDamage(Damage);
            }
        }
    }
}