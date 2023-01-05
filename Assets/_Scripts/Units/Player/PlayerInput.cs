using System;
using UnityEngine;

namespace rene_roid_player {
    public class PlayerInput : MonoBehaviour
    {
        [Header("Player Input")]
        [Tooltip("The player's basic attack")]
        [SerializeField] private KeyCode _basicAttack = KeyCode.Mouse0;

        [Tooltip("The player's ability 1")]
        [SerializeField] private KeyCode _ability1 = KeyCode.Mouse1;

        [Tooltip("The player's ability 2")]
        [SerializeField] private KeyCode _ability2 = KeyCode.Q;

        [Tooltip("The player's ultimate")]
        [SerializeField] private KeyCode _ultimate = KeyCode.E;

        public FrameInput FrameInput { get; private set; }

        void Update() => FrameInput = Gather();

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
