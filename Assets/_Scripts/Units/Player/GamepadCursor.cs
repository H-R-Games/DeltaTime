using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

namespace rene_roid {    
    public class GamepadCursor : MonoBehaviour
    {
        #region Internal
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private RectTransform _cursorTransform;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private RectTransform _canvasRectTransform;
        [SerializeField] private float _speed = 1000f;
        [SerializeField] private float _padding = 35f;
        private Mouse _virtualMouse;
        private Mouse _currentMouse;
        private bool _previousState;
        private Camera _mainCamera;

        private string _previousControlScheme = "";
        private const string gamepadScheme = "Gamepad";
        private const string keyboardScheme = "Keyboard&Mouse";
        #endregion

        private void OnEnable() {
            _mainCamera = Helpers.Camera;

            if (_virtualMouse == null) {
                _virtualMouse = (Mouse) InputSystem.AddDevice("VirtualMouse");
            } else if (!_virtualMouse.added) {
                InputSystem.AddDevice(_virtualMouse);
            }
            
            // Pair the virtual mouse with the player input
            InputUser.PerformPairingWithDevice(_virtualMouse, _playerInput.user);
            _currentMouse = Mouse.current;
            
            if (_cursorTransform != null) {
                Vector2 pos = _cursorTransform.anchoredPosition;
                InputState.Change(_virtualMouse.position, pos);
            }

            InputSystem.onAfterUpdate += UpdateMotion;
            //_playerInput.onControlsChanged += OnControlChanged;
        }

        private void OnDisable() {
            if (_virtualMouse != null && _virtualMouse.added) InputSystem.RemoveDevice(_virtualMouse);

            InputSystem.onAfterUpdate -= UpdateMotion;
            //_playerInput.onControlsChanged -= OnControlChanged;
        }

        private void Update() {
            if (_previousControlScheme != _playerInput.currentControlScheme) OnControlChanged(_playerInput);
            _previousControlScheme = _playerInput.currentControlScheme;
        }

        private void UpdateMotion() {
            if (_virtualMouse == null || Gamepad.current == null) return;

            // Delta position
            Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
            deltaValue *= _speed * Time.deltaTime;

            Vector2 currentPos = _virtualMouse.position.ReadValue();
            Vector2 newPos = currentPos + deltaValue;
        
            newPos.x = Mathf.Clamp(newPos.x, _padding, Screen.width - _padding);
            newPos.y = Mathf.Clamp(newPos.y, _padding, Screen.height - _padding);

            InputState.Change(_virtualMouse.position, newPos);
            InputState.Change(_virtualMouse.delta, deltaValue);

            bool aButtonIsPressed = Gamepad.current.aButton.isPressed;
            if (_previousState != aButtonIsPressed) {
                _virtualMouse.CopyState<MouseState>(out var mouseState); // Copy the current state of the virtual mouse
                mouseState.WithButton(MouseButton.Left, aButtonIsPressed); // Change the state of the left mouse button
                InputState.Change(_virtualMouse, mouseState); // Apply the new state to the virtual mouse
                _previousState = aButtonIsPressed;
            }

            AnchorCursor(newPos);
        }

        private void AnchorCursor(Vector2 pos) {
            if (_cursorTransform == null) return;
            
            Vector2 anchorPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, pos, _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCamera, out anchorPos);
            _cursorTransform.anchoredPosition = anchorPos;
        }

        private void OnControlChanged(PlayerInput playerInput) {
            if (playerInput.currentControlScheme == keyboardScheme && _previousControlScheme != keyboardScheme) {
                _cursorTransform.gameObject.SetActive(false);
                Cursor.visible = true;
                _currentMouse.WarpCursorPosition(_virtualMouse.position.ReadValue());
                _previousControlScheme = keyboardScheme;
            } else if (playerInput.currentControlScheme == gamepadScheme && _previousControlScheme != gamepadScheme) {
                _cursorTransform.gameObject.SetActive(true);
                Cursor.visible = false;
                InputState.Change(_virtualMouse.position, _currentMouse.position.ReadValue());
                AnchorCursor(_currentMouse.position.ReadValue());
                _previousControlScheme = gamepadScheme;
            }
        }
    }
}
