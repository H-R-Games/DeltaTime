using rene_roid_enemy;
using UnityEngine;

namespace rene_roid_player {
    public class TestPlayer : PlayerBase
    {
        #region Internal
        private CapsuleCollider2D _collider;
        private SpriteRenderer _renderer;
        
        [SerializeField] private LayerMask _enemyLayer;
        #endregion

        public override void Start()
        {
            base.Start();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<CapsuleCollider2D>();
        }

        public override void Update()
        {
            base.Update();
        }

        #region Skills
        [Header("Skills")]
        [Header("Basic Attack")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _basicAttackPercentage = 1;
        
        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCo = 1f;
        public override void BasicAttack()
        {
            base.BasicAttack();

            RaycastHit2D hit = Physics2D.Raycast(_collider.bounds.center, _renderer.flipX ? Vector2.left : Vector2.right, _collider.bounds.extents.x + 100f, _enemyLayer);
            if (hit.collider != null)
            {
                var enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(DealDamage(_basicAttackPercentage, _procCo));
                print("Hit!: " + enemy.gameObject.name);
            }
        }
        #endregion

        #region Functions
        #endregion
    }
}
