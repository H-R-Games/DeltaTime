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

        public override void Update() {
            if (Target == null) return;

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

        public override void TakeDamage(float damage)
        {
            Worm.TakeDamage(damage);
        }
    }
}
