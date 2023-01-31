using System;
using UnityEngine;
using rene_roid;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace rene_roid_player
{
    public class PlayerInput : MonoBehaviour
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        [Header("Player Input")]
        [Tooltip("The player's basic attack")]
        [SerializeField] private KeyCode _basicAttack = KeyCode.Mouse0;

        [Tooltip("The player's ability 1")]
        [SerializeField] private KeyCode _ability1 = KeyCode.Mouse1;

        [Tooltip("The player's ability 2")]
        [SerializeField] private KeyCode _ability2 = KeyCode.Q;

        [Tooltip("The player's ultimate")]
        [SerializeField] private KeyCode _ultimate = KeyCode.E;
#endif

        public FrameInput FrameInput { get; private set; }

        void Update() => FrameInput = Gather();

#if ENABLE_INPUT_SYSTEM
        private PlayerInputActions _inputActions;
        private InputAction _move, _jump, _basicAttack, _ability1, _ability2, _ultimate;

        private void Awake()
        {
            //if (_inputActions == null) _inputActions = new PlayerInputActions();
            _inputActions = InputManager.InputActions;
            
            _move = _inputActions.Player.Move;
            _jump = _inputActions.Player.Jump;
            _basicAttack = _inputActions.Player.BasicAttack;
            _ability1 = _inputActions.Player.Ability1;
            _ability2 = _inputActions.Player.Ability2;
            _ultimate = _inputActions.Player.Ultimate;
        }

        private void OnEnable()
        {
            //if (_inputActions == null) _inputActions = new PlayerInputActions();
            _inputActions = InputManager.InputActions;
            _inputActions.Enable();
        }

        private void OnDisable() => _inputActions.Disable();

        private FrameInput Gather()
        {
            return new FrameInput
            {
                Move = _move.ReadValue<Vector2>(),

                JumpDown = _jump.WasPressedThisFrame(),
                JumpHeld = _jump.IsPressed(),

                BasicAttackDown = _basicAttack.WasPressedThisFrame(),
                BasicAttackHeld = _basicAttack.IsPressed(),

                SpecialAttack1Down = _ability1.WasPressedThisFrame(),
                SpecialAttack1Held = _ability1.IsPressed(),

                SpecialAttack2Down = _ability2.WasPressedThisFrame(),
                SpecialAttack2Held = _ability2.IsPressed(),

                UltimateDown = _ultimate.WasPressedThisFrame(),
                UltimateHeld = _ultimate.IsPressed(),
            };
        }

#elif ENABLE_LEGACY_INPUT_MANAGER
        private FrameInput Gather()
        {
            return new FrameInput
            {
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),

                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),

                BasicAttackDown = Input.GetKeyDown(_basicAttack),
                BasicAttackHeld = Input.GetKey(_basicAttack),

                SpecialAttack1Down = Input.GetKeyDown(_ability1),
                SpecialAttack1Held = Input.GetKey(_ability1),

                SpecialAttack2Down = Input.GetKeyDown(_ability2),
                SpecialAttack2Held = Input.GetKey(_ability2),

                UltimateDown = Input.GetKeyDown(_ultimate),
                UltimateHeld = Input.GetKey(_ultimate),
            };
        }
#endif
    }

    public struct FrameInput
    {
        public Vector2 Move;

        public bool JumpDown;
        public bool JumpHeld;

        public bool BasicAttackDown;
        public bool BasicAttackHeld;

        public bool SpecialAttack1Down;
        public bool SpecialAttack1Held;

        public bool SpecialAttack2Down;
        public bool SpecialAttack2Held;

        public bool UltimateDown;
        public bool UltimateHeld;
    }
}
