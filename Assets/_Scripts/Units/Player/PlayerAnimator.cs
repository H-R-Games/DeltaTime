using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        #region Internal Variables
        [SerializeField] protected IPlayerController _player;
        protected PlayerBase _playerBase;
        protected Animator _anim;
        protected SpriteRenderer _renderer;
        protected AudioSource _source;
        #endregion

        protected void Awake()
        {
            _anim = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _source = GetComponent<AudioSource>();
            
            _player = GetComponentInParent<IPlayerController>();
            _playerBase = GetComponentInParent<PlayerBase>();
        }

        protected void Start()
        {
            _player.GroundedChanged += OnGroundedChanged;
            _player.WallGrabChanged += OnWallGrabChanged;
            _player.Jumped += OnJumped;
            _player.AirJumped += OnAirJumped;
            _player.BasicAttack1 += OnBasicAttack1;
            _player.SpecialAttack1 += OnSpecialAttack1;
            _player.SpecialAttack2 += OnSpecialAttack2;
            _player.UltimateAttack += OnUltimateAttack;
        }

        protected void Update()
        {
            HandleSpriteFlipping();
            HandleGroundEffects();
            HandleWallSlideEffects();
            HandleAnimations();
        }

        protected void HandleSpriteFlipping()
        {
            if (_player.WallDirection != 0) _renderer.flipX = _player.WallDirection == -1;
            else if (Mathf.Abs(_player.Input.x) > 0.1f) _renderer.flipX = _player.Input.x < 0;
        }

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

        #region Ground Movement
        [Header("GROUND MOVEMENT")]
        [SerializeField] protected AudioClip[] _footstepClips;
        [SerializeField] protected float _tiltChangeSpeed = .05f;
        protected Vector2 _tiltVelocity;

        protected void HandleGroundEffects()
        {
            // Move particles get bigger as you gain momentum
            var speedPoint = Mathf.InverseLerp(0, _playerBase.MaxStats.MovementSpeed, Mathf.Abs(_player.Speed.x));
            // To:Do: Add particle system

            // Tilt with slopes
            transform.up = Vector2.SmoothDamp(transform.up, _grounded ? _player.GroundNormal : Vector2.up, ref _tiltVelocity, _tiltChangeSpeed);
        }

        protected int _stepIndex = 0;

        public void PlayFootstep()
        {
            // To:Do: Footsep sounds
            _stepIndex = (_stepIndex + 1) % _footstepClips.Length;
            PlaySound(_footstepClips[_stepIndex], 0.01f);
        }

        #endregion

        #region Wall Sliding and Climbing
        [Header("WALL")]

        protected bool _hitWall, _isOnWall, _isSliding, _dismountedWall;

        protected void OnWallGrabChanged(bool onWall)
        {
            _hitWall = _isOnWall = onWall;
            _dismountedWall = !onWall;
        }

        protected void HandleWallSlideEffects()
        {
            var slidingThisFrame = _isOnWall && !_grounded && _player.Speed.y < 0;

            if (!_isSliding && slidingThisFrame)
            {
                _isSliding = true;
            }
            else if (_isSliding && !slidingThisFrame)
            {
                _isSliding = false;
            }
        }

        protected int _wallClimbIndex = 0;

        public void PlayWallClimbSound()
        {
            //_wallClimbIndex = (_wallClimbIndex + 1) % _wallClimbClips.Length;
            //PlaySound(_wallClimbClips[_wallClimbIndex], 0.1f);
        }

        #endregion

        #region Ladders

        [Header("LADDER")]
        [SerializeField] protected AudioClip[] _ladderClips;
        protected int _climbIndex = 0;

        public void PlayLadderClimbClip()
        {
            if (_player.Speed.y < 0) return;
            _climbIndex = (_climbIndex + 1) % _ladderClips.Length;
        }

        #endregion

        #region Jumping & Landing
        [Header("JUMPING")]
        [SerializeField] protected float _minImpactForce = 20;

        protected bool _jumpTriggered;
        protected bool _landed;
        protected bool _grounded;
        protected bool _wallJumped;

        protected void OnJumped(bool wallJumped)
        {
            _jumpTriggered = true;
            _wallJumped = wallJumped;
        }

        protected void OnAirJumped()
        {
            _jumpTriggered = true;
            _wallJumped = false;
        }

        protected void OnGroundedChanged(bool grounded, float impactForce)
        {
            _grounded = grounded;

            if (impactForce >= _minImpactForce)
            {
                var p = Mathf.InverseLerp(0, _minImpactForce, impactForce);
                _landed = true;
            }
        }
        #endregion

        #region Animation Methods
        protected float _lockedTill;

        protected void HandleAnimations()
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

        #region Audio
        protected void PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
        {
            _source.pitch = pitch;
            _source.PlayOneShot(clip, volume);
        }

        #endregion
    }
}
