using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerBase : MonoBehaviour
    {
        [SerializeField] private PlayerBaseStats _baseStats;

        #region Internal Variables
        [HideInInspector] private Rigidbody2D _rb;
        [SerializeField] private CapsuleCollider2D _col;
        [SerializeField] private LayerMask _playerLayer; // Layer mask for the player
        private bool _cachedTriggerSetting; // Used to cache the trigger setting of the collider

        private PlayerInput _input;
        private FrameInput _frameInput;
        private int _fixedFrame;
        #endregion

        public virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInput>();
        }

        public virtual void Start()
        {

        }

        public virtual void Update()
        {
            GatherInput();
        }

        public virtual void FixedUpdate()
        {
            FixedMovement();
        }

        protected virtual void GatherInput()
        {
            _frameInput = _input.FrameInput;

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _frameJumpWasPressed = _fixedFrame;
            }
        }

        #region Movement
        private Vector2 _speed;
        private Vector2 _currentExternalVelocity;

        private bool _hasControl = true;

        private void FixedMovement()
        {
            _fixedFrame++;

            CheckCollisions();
            HandleCollisions();
            HandleWalls();
            HandleLadders();

            HandleJump();

            HorizontalMovement();
            HandleVertical();
            ApplyVelocity();
        }

        #region Collisions
        private readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
        private readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[2];
        private readonly Collider2D[] _wallHits = new Collider2D[5];
        private readonly Collider2D[] _ladderHits = new Collider2D[1];
        private Vector2 _groundNormal;
        private int _groundHitCount;
        private int _ceilingHitCount;
        private int _wallHitCount;
        private int _ladderHitCount;
        private int _frameLeftGrounded = int.MinValue;
        private bool _grounded;

        protected virtual void CheckCollisions()
        {
            Physics2D.queriesHitTriggers = false;

            // Ground and Ceiling
            _groundHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _groundHits, _grounderDistance, ~_playerLayer);
            _ceilingHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _ceilingHits, _grounderDistance, ~_playerLayer);

            // Walls and Ladders
            var bounds = GetWallDetectionBounds();
            _wallHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _wallHits,  _wallLayerMask);

            Physics2D.queriesHitTriggers = true; // Ladders are set to Trigger
            _ladderHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _ladderHits, _ladderLayerMask);
            Physics2D.queriesHitTriggers = _cachedTriggerSetting;
        }

        protected virtual bool TryGetGroundNormal(out Vector2 groundNormal)
        {
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(_rb.position, Vector2.down, _grounderDistance * 2, ~_playerLayer);
            Physics2D.queriesHitTriggers = _cachedTriggerSetting;
            groundNormal = hit.normal;
            return hit.collider != null;
        }

        private Bounds GetWallDetectionBounds()
        {
            var colliderOrigin = _rb.position + _col.offset;
            return new Bounds(colliderOrigin, _wallDetectorSize);
        }

        protected virtual void HandleCollisions()
        {
            // Hit a Ceiling
            if (_ceilingHitCount > 0) _speed.y = Mathf.Min(0, _speed.y);

            // Landed on the ground
            if (!_grounded && _groundHitCount > 0)
            {
                _grounded = true;
                ResetJump();
                // Grounded changed Invoke
            }

            // Left the ground
            else if (_grounded && _groundHitCount == 0)
            {
                _grounded = false;
                _frameLeftGrounded = _fixedFrame;
                // Grounded changed Invoke
            }
        }
        #endregion

        #region Walls
        private float _currentWallJumpMoveMultiplier = 1f; // aka "Horizontal input influence"
        private int _wallDir;
        private bool _isOnWall;
        private bool _isLeavingWall; // prevents immediate re-sticking to wall

        protected virtual void HandleWalls()
        {
            if (!_allowWalls) return;

            _currentWallJumpMoveMultiplier = Mathf.MoveTowards(_currentWallJumpMoveMultiplier, 1f, 1f / _wallJumpInputLossFrames);

            _wallDir = _wallHitCount > 0 ? (int)Mathf.Sign(_wallHits[0].transform.position.x - transform.position.x) : 0;

            if (!_isOnWall && ShouldStickWall()) SetOnWall(true);
            else if (_isOnWall && !ShouldStickWall()) SetOnWall(false);
            
            bool ShouldStickWall() {
                if (_wallDir == 0 || _grounded) return false;
                return _requireInputPush ? Mathf.Sign(_frameInput.Move.x) == _wallDir : true;
            }
        }

        private void SetOnWall(bool on)
        {
            _isOnWall = on;
            if (on) _speed = Vector2.zero;
            else
            {
                _isLeavingWall = false;
                ResetAirJumps();
            }
            // WallGrabbedChanged Invoke "on"
        }

        #endregion
        #region Ladders
        private Vector2 _ladderSnapVel; // TODO: determine if we need to reset this when leaving a ladder, or use a different kind of Lerp/MoveTowards
        private int _frameLeftLadder = int.MinValue;
        private bool _onLadder;

        private bool CanEnterLadder => _ladderHitCount > 0 && _fixedFrame > _frameLeftLadder + _ladderCooldownFrames;
        private bool MountLadderInputReached => _frameInput.Move.y > _verticalDeadzoneThreshold || (!_grounded && _frameInput.Move.y < -_verticalDeadzoneThreshold);
        private bool DismountLadderInputReached => _grounded && _frameInput.Move.y < -_verticalDeadzoneThreshold;

        protected virtual void HandleLadders()
        {
            if (!_allowLadders) return;

            if (!_onLadder && CanEnterLadder && MountLadderInputReached) ToggleClimbingLadder(true);
            else if (_onLadder && (_ladderHitCount == 0 || DismountLadderInputReached )) ToggleClimbingLadder(false);

            // Snap to center of ladder
            if (_onLadder && _frameInput.Move.x == 0 && _snapToLadders && _hasControl)
            {
                var pos = _rb.position;
                _rb.position = Vector2.SmoothDamp(pos, new Vector2(_ladderHits[0].transform.position.x, pos.y), ref _ladderSnapVel, _ladderSnapSpeed);
            }
        }

        private void ToggleClimbingLadder(bool on)
        {
            if (_onLadder == on) return;
            if (on) _speed = Vector2.zero;
            else if (_ladderHitCount > 0) _frameLeftLadder = _fixedFrame;
            _onLadder = on;
            ResetAirJumps();
        }
        #endregion

        #region Jumping 
        private bool _jumpToConsume;
        private bool _endedJumpEarly;
        private bool _coyoteUsable;
        private bool _bufferedJumpUsable;
        private int _frameJumpWasPressed = int.MinValue;
        private int _airJumpsRemaining;

        private bool CanUseCoyote => _coyoteUsable && !_grounded && _fixedFrame < _frameLeftGrounded + _coyoteFrames;
        private bool HasBufferedJump => _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _jumpBufferFrames;
        private bool CanAirJump => _airJumpsRemaining > 0;

        protected virtual void HandleJump()
        {
            if (_jumpToConsume || HasBufferedJump)
            {
                if (_isOnWall && !_isLeavingWall) WallJump();
                else if (_grounded || _onLadder || CanUseCoyote) NormalJump();
                else if (_jumpToConsume && CanAirJump) AirJump();
            }

            _jumpToConsume = false; // Allways consume the flag

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
        }

        protected virtual void NormalJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            ToggleClimbingLadder(false);
            _speed.y = _jumpForce;
            // Jumped Invoke
        }

        protected virtual void WallJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _isLeavingWall = true;
            _currentWallJumpMoveMultiplier = 0;
            _speed = Vector2.Scale(_wallJumpPower, new Vector2(-_wallDir, 1));
            // Jumped Invoke
        }

        protected virtual void AirJump()
        {
            _endedJumpEarly = false;
            _airJumpsRemaining--;
            _speed.y = _jumpForce;
            // Air Jumped Invoke
        }

        protected virtual void ResetJump()
        {
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            ResetAirJumps();
        }

        protected virtual void ResetAirJumps() => _airJumpsRemaining = _maxAirJumps;
        #endregion

        #region Horizontal
        protected virtual void HorizontalMovement()
        {
            // Deceleracion
            if (_frameInput.Move.x == 0)
                _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _groundDeceleration : _airDeceleration) * Time.fixedDeltaTime);

            // Regular Horizontal Movement
            else
            {
                if (_wallHitCount > 0 && Mathf.Approximately(_rb.velocity.x, 0) && Mathf.Sign(_frameInput.Move.x) == Mathf.Sign(_speed.x)) // Prevents gaining speed when moving against a wall
                    _speed.x = 0;

                var inputX = _frameInput.Move.x * (_onLadder ? _ladderShimmySpeedMultiplier : 1);
                _speed.x = Mathf.MoveTowards(_speed.x, inputX * _baseStats.MovementSpeed, _currentWallJumpMoveMultiplier * _acceleration * Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Vertical
        protected virtual void HandleVertical()
        {
            // Ladder
            if (_onLadder)
            {
                var inputY = _frameInput.Move.y;
                _speed.y = inputY * (inputY > 0 ? _ladderClimbSpeed : _ladderSlideSpeed);
            }
            // Grounded & Slopes
            else if (_grounded && _speed.y <= 0f)
            {
                _speed.y = _groundingForce;

                if (TryGetGroundNormal(out _groundNormal))
                {
                    if (!Mathf.Approximately(_groundNormal.y, 1f))
                    {
                        _speed.y = _speed.x * -_groundNormal.x / _groundNormal.y;
                        if (_speed.x != 0) _speed.y += _groundingForce;
                    }
                }
            }
            // Wall Climbing & Sliding
            else if (_isOnWall && !_isLeavingWall)
            {
                if (_frameInput.Move.y > 0) _speed.y = _wallClimbSpeed;
                else if (_frameInput.Move.y < 0) _speed.y = -_maxWallFallSpeed;
                else _speed.y = Mathf.MoveTowards(Mathf.Min(_speed.y, 0), -_maxFallSpeed, _wallFallAcceleration * Time.deltaTime);
            }

            // In Air
            else
            {
                var inAirGravity = _fallAcceleration;
                if (_endedJumpEarly && _speed.y > 0) inAirGravity *= _jumpEndEarlyGravityModifier;
                _speed.y = Mathf.MoveTowards(_speed.y, -_maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }
        #endregion

        protected virtual void ApplyVelocity()
        {
            if (!_hasControl) return; // If the player doesn't have control, don't move
            _rb.velocity = _speed + _currentExternalVelocity;

            _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _externalVelocityDecay * Time.deltaTime);
        }

        #region Movement Stats
        // Movement
        private float _acceleration = 120; // Capacity to gain horizontal speed
        private float _groundDeceleration = 60; // Pace at which the player comes to a stop
        private float _airDeceleration = 30; // Deceleration in air only after stopping input mid-air
        private float _groundingForce = -1.5f; // Constant force applied to the player when grounded (Slopes)

        private float _verticalDeadzoneThreshold = 0.3f; // Min input required to mount ladders
        private float _horizontalDeadzoneThreshold = 0.1f; // Min input required to move


        // Jump
        private int _maxAirJumps = 0; // Max amount of jumps the player can do in the air. 0 = No air jumps
        private float _jumpForce = 36; // Inmediate force applied to the player when jumping
        private float _maxFallSpeed = 40; // Max speed the player can fall at
        private float _fallAcceleration = 100; // Acceleration applied to the player when falling
        private float _jumpEndEarlyGravityModifier = 3; // Gravity modifier applied to the player when ending a jump early
        private int _coyoteFrames = 7; // Amount of frames the player can jump after leaving the ground
        private int _jumpBufferFrames = 7; // Amount of fixed frames we buffer a jump. This allows jump input before actually hitting the ground


        // Walls
        private bool _allowWalls = true; // Allows wall slide / Jump
        [SerializeField] private LayerMask _wallLayerMask; // Layer mask for climbable walls are on
        private bool _requireInputPush = false; // If true, the player must push against the wall to climb it

        private float _wallClimbSpeed = 7; // Speed at which the player climbs walls -------------- NEEDS TO BE HALF OF PLAYER SPEED
        private float _wallFallAcceleration = 8; // Acceleration applied to the player when falling off a wall
        private float _maxWallFallSpeed = 16; // Max speed the player can fall off a wall at
        private Vector2 _wallJumpPower = new Vector2(30, 25); // Power applied to the player when jumping off a wall
        private int _wallJumpInputLossFrames = 18; // The frames before full horizontal movement is returned after a wall jump


        // Ladders
        private bool _allowLadders = true; // Allows ladder climbing
        private bool _snapToLadders = true; // If true, the player will snap to the ladder when climbing it
        [SerializeField] private LayerMask _ladderLayerMask; // Layer mask for climbable ladders are on
        private float _ladderSnapSpeed = 0.05f;
        private float _ladderShimmySpeedMultiplier = 0.5f; // Horizontal speed multiplier while attached to a ladder

        private float _ladderClimbSpeed = 8; // Speed at which the player climbs ladders
        private float _ladderSlideSpeed = 12;
        private int _ladderCooldownFrames = 8; // How many frames can pass between ladder interactions


        // Collisions
        private float _grounderDistance = 0.1f; // Distance from the player's feet to the ground
        private Vector2 _wallDetectorSize = new Vector2(0.75f, 1.25f); // Size of the wall detector box


        // External
        private int _externalVelocityDecay = 100;
        #endregion

        public enum PlayerForce
        {
            /// <summary>
            /// Added directly to the players movement speed, to be controlled by the standard deceleration
            /// </summary>
            Burst,

            /// <summary>
            /// An additive force handled by the decay system
            /// </summary>
            Decay
        }
        #endregion

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var bounds = GetWallDetectionBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        private void OnValidate()
        {
            if (_baseStats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
            if (_col == null) Debug.LogWarning("Please assign a Capsule Collider to the collider slot", this);
            if (_rb == null) _rb = GetComponent<Rigidbody2D>(); // serialized but hidden in the inspector
        }
#endif
    }
}
