using UnityEngine;

namespace rene_roid_enemy {    
    public class DragonBody : EnemyBase
    {
        [SerializeField] private Dragon _dragon;
        [SerializeField] private float _dmgMultiplier = 1;

        public override void TakeDamage(float damage, bool item = false)
        {
            damage *= _dmgMultiplier;
            _dragon.TakeDamage(damage);
        }
    }
}
