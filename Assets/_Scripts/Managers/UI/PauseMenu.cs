using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
#endif

namespace rene_roid {    
    public class PauseMenu : MonoBehaviour
    {
        private PlayerInputActions _inputActions;
        private InputAction _esc;
        private GameObject _pauseMenu;

        private void Start() {
            _inputActions = InputManager.InputActions;
            _esc = _inputActions.UI.Escape;

            _pauseMenu = this.transform.Find("Pause Menu").gameObject;

            Time.timeScale = 1;
        }

        private void Update() {
            if (_esc.triggered) {
                PauseGame();
            }
        }

        public void PauseGame() {
            Time.timeScale = 0;
            _pauseMenu.SetActive(true);
        }

        public void ResumeGame() {
            Time.timeScale = 1;
            _pauseMenu.SetActive(false);
        }
    }
}
