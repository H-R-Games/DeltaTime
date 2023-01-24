using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class NamkaAnimations : PlayerAnimator
    {
        public override void Update()
        {
            base.Update();
        }

        #region Skills
        private bool _stopSpecial2 = false;
        public void StopSpecial2()
        {
            _stopSpecial2 = true;
        }
        #endregion

        #region Animation Methods
        public override void HandleAnimations()
        {
            var state = GetState();
            ResetFlags();

            if (_currentState == SpecialAttack2Anim && _stopSpecial2)
            {
                _lockedTill = Time.time + 1f;
                _stopSpecial2 = false;    
            }

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
    }
}
