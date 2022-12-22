using UnityEngine;

namespace rene_roid_player {
    public class PlayerInput : MonoBehaviour
    {
        public FrameInput FrameInput { get; private set; }

        void Update() => FrameInput = Gather();
        
        private FrameInput Gather()
        {
            return new FrameInput
            {
                Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),

                JumpDown = Input.GetButtonDown("Jump"),
                JumpHeld = Input.GetButton("Jump"),

                BasicAttackDown = Input.GetKeyDown(KeyCode.Mouse0),
                BasicAttackHeld = Input.GetKey(KeyCode.Mouse0),

                SpecialAttack1Down = Input.GetKeyDown(KeyCode.Mouse1),
                SpecialAttack1Held = Input.GetKey(KeyCode.Mouse1),

                SpecialAttack2Down = Input.GetKeyDown(KeyCode.Q),
                SpecialAttack2Held = Input.GetKey(KeyCode.Q),

                UltimateDown = Input.GetKeyDown(KeyCode.E),
                UltimateHeld = Input.GetKey(KeyCode.E),
            };
        }
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
