using UnityEngine;

namespace rene_roid_enemy {
    public class TestEnemy : EnemyBase
    {

        public override void Start()
        {
            base.Start();
            EnemyType = EnemyTypes.EnemyHorizontal;
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
