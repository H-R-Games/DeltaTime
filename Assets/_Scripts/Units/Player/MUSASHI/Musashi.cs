using System.Collections;
using rene_roid;
using UnityEngine;

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

            if (_shadowCloneTimer < Time.time) {
                var clone = Instantiate(_cloneFadePrefab, transform.position, Quaternion.identity);
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

        private int _basicAttackCount = 1;
        private float _comboFinishTimer = 0;
        private float _resetComboTimer = 0;

        public override void BasicAttack()
        {
            if (Time.time < _comboFinishTimer) return;
            // If the player is not grounded, then we don't want to do the combo
            if (!IsGrounded()) return;
            base.BasicAttack();

            // If the player does not attack for 0.5 seconds, then the combo is reset
            if (Time.time > _resetComboTimer) _basicAttackCount = 1;
            _resetComboTimer = Time.time + 0.5f;


            _basicAttackCount++;
            if (_basicAttackCount > 3) {
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

            if (IsGrounded()) {
                StartCoroutine(RemoveControl(.2f));
                _rb.velocity = new Vector2(0, 0);
            }

            var clone = Instantiate(_shadowClonePrefab, transform.position, Quaternion.identity);
            var shadowClone = clone.GetComponent<ShadowClone>();
            shadowClone.SetFlipX(_renderer.flipX);
            shadowClone.Dash(50, 0.2f);
        }


        [Header("Skill 2")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _skill2Percentage = 1;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoSkill2 = 1f;        


        public override void Skill2()
        {
            base.Skill2();

        }


        [Header("Ultimate")]
        [Tooltip("The damage of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _ultimatePercentage = 5;

        [Tooltip("The proc chance of the basic attack: 0 = 0% | 1 = 100% | 1.5 = 150%...")]
        [SerializeField] private float _procCoUltimate = 2f;

        public override void Ultimate()
        {
            base.Ultimate();
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
