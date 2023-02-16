using System.Collections;
using rene_roid;
using UnityEngine;
using rene_roid_enemy;

namespace rene_roid_player
{
    public class Musashi : PlayerBase
    {
        #region Internal
        private CapsuleCollider2D _collider;
        private SpriteRenderer _renderer;
        [SerializeField] private CloneFade _cloneFadePrefab;
        private float _shadowCloneTimer = 0;
        private float _shadowCloneCooldown = 0.1f;

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

            if (_shadowCloneTimer < Time.time)
            {
                var clone = Instantiate(_cloneFadePrefab, transform.position, Quaternion.identity);
                // Set the clone position on the left 
                clone.SetFade(0.5f);
                clone.SetSprite(_renderer.sprite, _renderer.flipX);
                clone.SetFadeTime(0.2f);
                clone.Fade();
                _shadowCloneTimer = Time.time + _shadowCloneCooldown;
            }
        }

        #region Skills
        [Header("Skills")]
        [Header("Basic Attack")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _basicAttackPercentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoBasic = 1f;
        [SerializeField] private BoxCollider2D _basicAttackCollider;

        private int _basicAttackCount = 1;
        private float _comboFinishTimer = 0;
        private float _resetComboTimer = 0;

        public override void BasicAttack()
        {
            if (Time.time < _comboFinishTimer) return;
            // If the player is not grounded, then we don't want to do the combo
            if (!IsGrounded()) return;
            base.BasicAttack();

            StartCoroutine(RemoveControl(.1f));

            // If the player does not attack for 0.5 seconds, then the combo is reset
            if (Time.time > _resetComboTimer) _basicAttackCount = 1;
            _resetComboTimer = Time.time + 0.5f;

            // Detect enemies in the collider
            var enemies = Physics2D.OverlapBoxAll(_basicAttackCollider.transform.position, _basicAttackCollider.size, 0, _enemyLayer);
            foreach (var enemy in enemies)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase == null) continue;
                enemyBase.TakeDamage(DealDamage(_basicAttackPercentage, _procCoBasic));
                print("Hit enemy");
            }


            _basicAttackCount++;
            if (_basicAttackCount > 3)
            {
                _basicAttackCount = 1;
                _comboFinishTimer = Time.time + 0.5f;
            }
        }


        public int GetAttackCount()
        {
            return _basicAttackCount;
        }


        [Header("Skill 1")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _skill1Percentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoSkill1 = 1f;
        [SerializeField] private GameObject _shadowClonePrefab;


        public override void Skill1()
        {
            base.Skill1();

            if (IsGrounded())
            {
                StartCoroutine(RemoveControlAny(.2f));
                _rb.velocity = new Vector2(0, 0);
            }

            var clone = Instantiate(_shadowClonePrefab, transform.position, Quaternion.identity);
            var shadowClone = clone.GetComponent<ShadowClone>();
            shadowClone.SetFlipX(_renderer.flipX);
            shadowClone.Dash(50, 0.2f, _isOniMode, this, _skill1Percentage, _procCoSkill1);
        }


        [Header("Skill 2")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _skill2Percentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoSkill2 = 1f;
        [SerializeField] private GameObject _smokePrefab;
        [SerializeField] private BoxCollider2D _dashCollider;

        public override void Skill2()
        {
            base.Skill2();

            if (_isOniMode) {
                var rad = 10;
                var enemies = Physics2D.OverlapCircleAll(transform.position, rad, _enemyLayer);

                StartCoroutine(SuperCoolDash(enemies));
            } else {
                StartCoroutine(RemoveControlAny(.1f));
                _rb.velocity = new Vector2(0, _rb.velocity.y);

                var dist = 10;
                var ms = _baseStats.MovementSpeed;
                var cms = _currentMovementSpeed;

                var c = 1 + ((cms / ms) / ms);
                StartCoroutine(Dash(dist * c, 0.1f));
            }

        }


        private IEnumerator Dash(float dist, float t)
        {
            var dir = _renderer.flipX ? -1 : 1;
            var pos = transform.position;
            var target = new Vector2(pos.x + (dist * dir), pos.y);

            var ray = Physics2D.Raycast(pos, Vector2.right * dir, dist, _wallLayerMask);
            if (ray.collider != null)
            {
                target = ray.point;
            }

            yield return Helpers.GetWait(t);
            var smoke = Instantiate(_smokePrefab, transform.position, Quaternion.identity);
            var render = smoke.GetComponent<SpriteRenderer>();
            render.flipX = _renderer.flipX;

            Destroy(smoke, 1f);
            transform.position = target;

            // Detect enemies in the collider
            var enemies = Physics2D.OverlapBoxAll(_dashCollider.transform.position, _dashCollider.size, 0, _enemyLayer);
            yield return Helpers.GetWait(0.1f);
            foreach (var enemy in enemies)
            {
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase == null) continue;
                enemyBase.TakeDamage(DealDamage(_skill2Percentage, _procCoSkill2));
                print("Dash hit to: " + enemyBase.name);
            }
        }

        private IEnumerator SuperCoolDash(Collider2D[] enemies) {
            this.TakeAwayControl(true);
            _rb.velocity = new Vector2(0, 0);
            // For each enemy in the collider
            foreach (var enemy in enemies)
            {
                _rb.velocity = new Vector2(0, 0);
                var enemyBase = enemy.GetComponent<EnemyBase>();
                if (enemyBase == null) continue;

                var dir = enemyBase.transform.position - transform.position;
                var pos = transform.position;
                var speed = 10;
                dir.Normalize();

                // Move to the enemy
                // while (Vector2.Distance(pos, enemyBase.transform.position) > 0.3f)
                // {
                //     _rb.velocity = new Vector2(0, 0);
                //     // Lerp to the enemy
                //     pos = Vector2.Lerp(pos, enemyBase.transform.position, Time.deltaTime * speed);
                //     transform.position = pos;
                //     yield return null;
                // }

                yield return Helpers.GetWait(0.2f);

                transform.position = enemy.transform.position;

                // Deal damage
                enemyBase.TakeDamage(DealDamage(_skill2Percentage, _procCoSkill2));

                print("Dash hit to: " + enemyBase.name);
            }

            ReturnControl();
            print("Dash finished");

            yield return null;
        }

        [Header("Ultimate")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _ultimatePercentage = 5;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoUltimate = 2f;
        private bool _isOniMode = false;

        public override void Ultimate()
        {
            base.Ultimate();

            if (_isOniMode) return;
            StartCoroutine(OniMode());
        }

        private IEnumerator OniMode()
        {
            _isOniMode = true;
            _renderer.color = Color.red;

            var dmg = _maxStats.Damage;
            var spd = _maxStats.MovementSpeed;

            AddFlatDamageBonus(dmg);
            AddMovementSpeedFlat(spd);

            UpdateCurrentStats();

            yield return Helpers.GetWait(15f);

            RemoveFlatDamageBonus(dmg);
            RemoveMovementSpeedFlat(spd);

            UpdateCurrentStats();

            _renderer.color = Color.white;
            _isOniMode = false;
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

        private IEnumerator RemoveControlAny(float t)
        {
            this.TakeAwayControl(false);
            yield return Helpers.GetWait(t);
            ReturnControl();
        }
        #endregion
    }
}
