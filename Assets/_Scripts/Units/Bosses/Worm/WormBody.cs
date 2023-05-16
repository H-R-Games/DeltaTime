using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_enemy {    
    public class WormBody : EnemyBase
    {
        public Transform Target;
        private Transform _head;
        [SerializeField] private float _rotSpeed = 10;
        public WormBoss Worm;
        public void SetTarget(Transform target) {
            Target = target;
        }

        public override void Start() {
            _head = transform.GetChild(1);
        }

        private float _damageCD = 0.5f;
        private float _damageTimer = 0f;
        public override void Update() {
            if (Target == null) return;

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

            var rot = Quaternion.Lerp(_head.rotation, Target.rotation, Time.deltaTime * ((Worm.GetWormSpeed() / 3) * 2));
            transform.rotation = rot;
            
            var dist = _head.transform.position - transform.position;
            var pos = _head.transform.position;

            // Use pos to move towards the target
            pos = Target.position;

            pos = new Vector3(pos.x, pos.y, 0);
            pos -= dist;
            transform.position = pos;

        }

        public override void TakeDamage(float damage, bool item = false)
        {
            Worm.TakeDamage(damage);
        }
    }
}
