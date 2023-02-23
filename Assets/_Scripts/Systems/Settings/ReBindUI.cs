using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace rene_roid
{
    public class ReBindUI : MonoBehaviour
    {
        [Header("Keybinds")]
        [SerializeField] private InputActionReference _inputActionReference;
        [SerializeField] private bool _excludeMouse = true;
        [SerializeField][Range(0, 10)] private int _selectedBinding;
        [SerializeField] private InputBinding.DisplayStringOptions _displayStringOptions;

        [Header("Binding Info - Do not change")]
        [SerializeField] private InputBinding _inputBinding;
        private int _bindingIndex;
        private string _actionName;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _actionText;
        [SerializeField] private TextMeshProUGUI _rebindingText;
        [SerializeField] private Button _rebindingButton;
        [SerializeField] private Button _resetButton;

        private void OnEnable() {
            _rebindingButton.onClick.AddListener(() => DoRebind());
            _resetButton.onClick.AddListener(() => ResetBinding());

            if (_inputActionReference != null)
            {
                InputManager.LoadBindingOverride(_actionName);
                GetBindingInfo();
                UpdateUI();
            }

            InputManager.OnRebindComplete += UpdateUI;
            InputManager.OnRebindCancelled += UpdateUI;
        }

        private void OnDisable() {
            _rebindingButton.onClick.RemoveListener(() => DoRebind());
            _resetButton.onClick.RemoveListener(() => ResetBinding());

            InputManager.OnRebindComplete -= UpdateUI;
            InputManager.OnRebindCancelled -= UpdateUI;
        }

        private void OnValidate()
        {
            if (_inputActionReference == null) return;
            GetBindingInfo();
            UpdateUI();
        }

        private void GetBindingInfo()
        {
            if (_inputActionReference.action != null)
                _actionName = _inputActionReference.action.name;

            if (_inputActionReference.action.bindings.Count > _selectedBinding)
            {
                _inputBinding = _inputActionReference.action.bindings[_selectedBinding];
                _bindingIndex = _selectedBinding;
            }
        }

        private void UpdateUI()
        {
            if (_actionText != null)
                _actionText.text = _actionName;

            if (_rebindingText != null)
            {
                if (Application.isPlaying)
                {
                    // Grab info from Input manager
                    _rebindingText.text = InputManager.GetBindingName(_actionName, _bindingIndex);
                }
                else
                    _rebindingText.text = _inputActionReference.action.GetBindingDisplayString(_bindingIndex);
            }
        }

        private void DoRebind() {
            InputManager.StartRebind(_actionName, _bindingIndex, _rebindingText, _excludeMouse);
        }

        private void ResetBinding() {
            InputManager.ResetBinding(_actionName, _bindingIndex);
            UpdateUI();
        }
    }
}
