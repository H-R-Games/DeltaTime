using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class NamkaAnimations : PlayerAnimator
    {
        #region Skills
        [Header("Skills")]
        [SerializeField] private float _basicAttack1Time;
        [SerializeField] private float _specialAttack1Time, _specialAttack2Time, _ultimateAttackTime;
        private bool _basicAttack1, _specialAttack1, _specialAttack2, _ultimateAttack;

        protected void OnBasicAttack1()
        {
            _basicAttack1 = true;
        }

        protected void OnSpecialAttack1()
        {
            _specialAttack1 = true;
        }

        protected void OnSpecialAttack2()
        {
            _specialAttack2 = true;
        }

        protected void OnUltimateAttack()
        {
            _ultimateAttack = true;
        }
        #endregion

        #region Animation Methods

        public override void HandleAnimations()
        {
            var state = GetState();
            ResetFlags();
            if (state == _currentState) return;

            _anim.Play(state, 0); //_anim.CrossFade(state, 0, 0);
            _currentState = state;

            int GetState()
            {
                if (Time.time < _lockedTill) return _currentState;

                // ANY SKILL PRESSED
                if (_ultimateAttack) return LockState(UltimateAttackAnim, _ultimateAttackTime);
                if (_specialAttack2) return LockState(SpecialAttack2Anim, _specialAttack2Time);
                if (_specialAttack1) return LockState(SpecialAttack1Anim, _specialAttack1Time);
                if (_basicAttack1) return LockState(BasicAttackAnim, _basicAttack1Time);
                // return LockState(BasicAttackAnim, _basicAttack1Time);

                if (!_grounded)
                {
                    // Hit wall?
                    if (_isOnWall)
                    {
                        // Wall animations?
                    }
                }

                if (_jumpTriggered) return _wallJumped ? Jump : Jump;

                if (_grounded) return _player.Input.x == 0 ? Idle : Run;
                if (_player.Speed.y > 0) return _wallJumped ? Jump : Jump;
                return Fall;

                // State and time to lock
                int LockState(int s, float t)
                {
                    _lockedTill = Time.time + t;
                    return s;
                }
            }

            void ResetFlags()
            {
                _basicAttack1 = _specialAttack1 = _specialAttack2 = _ultimateAttack = false;

                _jumpTriggered = false;
                _landed = false;
                _hitWall = false;
                _dismountedWall = false;
            }
        }
        #endregion

        #region Cached Properties
        protected int _currentState;

        protected static readonly int Idle = Animator.StringToHash("Idle");
        protected static readonly int Run = Animator.StringToHash("Run");

        protected static readonly int Jump = Animator.StringToHash("Jump");
        protected static readonly int Fall = Animator.StringToHash("Fall");

        protected static readonly int BasicAttackAnim = Animator.StringToHash("BasicAttack");
        protected static readonly int SpecialAttack1Anim = Animator.StringToHash("SpecialAttack1");
        protected static readonly int SpecialAttack2Anim = Animator.StringToHash("SpecialAttack2");
        protected static readonly int UltimateAttackAnim = Animator.StringToHash("UltimateAttack");
        #endregion

    }
}
