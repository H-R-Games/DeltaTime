using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
    public class PlayerAnimator : MonoBehaviour
    {
        #region Internal Variables
        [SerializeField] private IPlayerController _player;
        private PlayerBase _playerBase;
        private Animator _anim;
        private SpriteRenderer _renderer;
        private AudioSource _source;
        #endregion

        private void Awake()
        {
            _anim = GetComponent<Animator>();
            _renderer = GetComponent<SpriteRenderer>();
            _source = GetComponent<AudioSource>();
            
            _player = GetComponentInParent<IPlayerController>();
            _playerBase = GetComponentInParent<PlayerBase>();
        }

        private void Start()
        {
            _player.GroundedChanged += OnGroundedChanged;
            _player.WallGrabChanged += OnWallGrabChanged;
            _player.Jumped += OnJumped;
            _player.AirJumped += OnAirJumped;
        }

        private void Update()
        {
            HandleSpriteFlipping();
            HandleGroundEffects();
            HandleWallSlideEffects();
            HandleAnimations();
        }

        private void HandleSpriteFlipping()
        {
            if (_player.WallDirection != 0) _renderer.flipX = _player.WallDirection == -1;
            else if (Mathf.Abs(_player.Input.x) > 0.1f) _renderer.flipX = _player.Input.x < 0;
        }


        #region Ground Movement
        [Header("GROUND MOVEMENT")]
        [SerializeField] private AudioClip[] _footstepClips;
        [SerializeField] private float _tiltChangeSpeed = .05f;
        private Vector2 _tiltVelocity;

        private void HandleGroundEffects()
        {
            // Move particles get bigger as you gain momentum
            var speedPoint = Mathf.InverseLerp(0, _playerBase.MaxStats.MovementSpeed, Mathf.Abs(_player.Speed.x));
            // To:Do: Add particle system

            // Tilt with slopes
            transform.up = Vector2.SmoothDamp(transform.up, _grounded ? _player.GroundNormal : Vector2.up, ref _tiltVelocity, _tiltChangeSpeed);
        }

        private int _stepIndex = 0;

        public void PlayFootstep()
        {
            // To:Do: Footsep sounds
            _stepIndex = (_stepIndex + 1) % _footstepClips.Length;
            PlaySound(_footstepClips[_stepIndex], 0.01f);
        }

        #endregion

        #region Wall Sliding and Climbing
        [Header("WALL")]

        private bool _hitWall, _isOnWall, _isSliding, _dismountedWall;

        private void OnWallGrabChanged(bool onWall)
        {
            _hitWall = _isOnWall = onWall;
            _dismountedWall = !onWall;
        }

        private void HandleWallSlideEffects()
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

        private int _wallClimbIndex = 0;

        public void PlayWallClimbSound()
        {
            //_wallClimbIndex = (_wallClimbIndex + 1) % _wallClimbClips.Length;
            //PlaySound(_wallClimbClips[_wallClimbIndex], 0.1f);
        }

        #endregion


        #region Ladders

        [Header("LADDER")]
        [SerializeField] private AudioClip[] _ladderClips;
        private int _climbIndex = 0;

        public void PlayLadderClimbClip()
        {
            if (_player.Speed.y < 0) return;
            _climbIndex = (_climbIndex + 1) % _ladderClips.Length;
        }

        #endregion

        #region Jumping & Landing
        [Header("JUMPING")]
        [SerializeField] private float _minImpactForce = 20;

        private bool _jumpTriggered;
        private bool _landed;
        private bool _grounded;
        private bool _wallJumped;

        private void OnJumped(bool wallJumped)
        {
            _jumpTriggered = true;
            _wallJumped = wallJumped;
        }

        private void OnAirJumped()
        {
            _jumpTriggered = true;
            _wallJumped = false;
        }

        private void OnGroundedChanged(bool grounded, float impactForce)
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
        private float _lockedTill;

        private void HandleAnimations()
        {
            var state = GetState();
            ResetFlags();
            if (state == _currentState) return;

            int GetState()
            {
                return 0;
            }

            void ResetFlags()
            {
                _jumpTriggered = false;
                _landed = false;
                _hitWall = false;
                _dismountedWall = false;
            }
        }
        #endregion

        #region Cached Properties
        private int _currentState;

        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Run = Animator.StringToHash("Run");

        private static readonly int Jump = Animator.StringToHash("Jump");
        private static readonly int Fall = Animator.StringToHash("Fall");
        #endregion
        
        #region Audio
        private void PlaySound(AudioClip clip, float volume = 1, float pitch = 1)
        {
            _source.pitch = pitch;
            _source.PlayOneShot(clip, volume);
        }

        #endregion
    }
}
