using rene_roid;
using rene_roid_enemy;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace rene_roid_player
{
    public class Namka : PlayerBase
    {
        #region Internal
        private CapsuleCollider2D _collider;
        private SpriteRenderer _renderer;
        private Rigidbody2D _rb2;

        [SerializeField] private LayerMask _enemyLayer;
        #endregion

        public override void Start()
        {
            base.Start();
            _renderer = GetComponentInChildren<SpriteRenderer>();
            _collider = GetComponent<CapsuleCollider2D>();
            _rb2 = GetComponent<Rigidbody2D>();
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
        [SerializeField] private float _procCoBasic = 1f;

        public override void BasicAttack()
        {
            base.BasicAttack();

            StartCoroutine(RemoveControl(.3f));

            if (IsGrounded()) _rb2.velocity = new Vector2(0, _rb2.velocity.y);

            RaycastHit2D hit = Physics2D.Raycast(_collider.bounds.center, _renderer.flipX ? Vector2.left : Vector2.right, _collider.bounds.extents.x + 100f, _enemyLayer);
            if (hit.collider != null)
            {
                var enemy = hit.collider.GetComponent<EnemyBase>();
                if (enemy != null) enemy.TakeDamage(DealDamage(_basicAttackPercentage, _procCoBasic));
                print("Hit!: " + enemy.gameObject.name);
            }
        }


        [Header("Skill 1")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _skill1Percentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoSkill1 = 1f;

        [SerializeField] private BoxCollider2D _skill1Rad;

        public override void Skill1()
        {
            if (!IsGrounded()) return;

            base.Skill1();

            StartCoroutine(RemoveControl(1f));

            _rb2.velocity = new Vector2(0, 0);

            StartCoroutine(DealDamage(_skill1Percentage, _procCoSkill1, 0.28f));
        }

        private IEnumerator DealDamage(float percentage, float procCo, float delay)
        {
            yield return new WaitForSeconds(delay);

            // Flip _skill1Rad to the correct direction
            if (_renderer.flipX) _skill1Rad.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            else _skill1Rad.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            // Detect if _enemyLayer is inside _skill2Rad
            var enemies = Physics2D.OverlapBoxAll(_skill1Rad.bounds.center, _skill1Rad.bounds.size, 0, _enemyLayer);
            foreach (var enemy in enemies)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase != null) enemyBase.TakeDamage(DealDamage(percentage, procCo));
            }
        }
        #endregion

        #region Functions
        private IEnumerator RemoveControl(float t)
        {
            if (IsGrounded())
            {
                this.TakeAwayControl(false);
                yield return new WaitForSeconds(t);
                ReturnControl();
            }
        }
        #endregion
    }
}
