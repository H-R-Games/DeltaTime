using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace rene_roid
{
    public class InputManager : MonoBehaviour
    {
        public static PlayerInputActions InputActions { get; private set; }

        public static event Action OnRebindComplete;
        public static event Action OnRebindCancelled;
        public static event Action<InputAction, int> OnRebindStarted;

        private void Awake()
        {
            if (InputActions == null) InputActions = new PlayerInputActions();
        }

        // Load all bindings on start
        private void Start()
        {
            foreach (var action in InputActions.asset.actionMaps)
            {
                for (int i = 0; i < action.actions.Count; i++)
                {
                    LoadBindingOverride(action.actions[i].name);
                }
            }
        }

        public static void StartRebind(string actionName, int bindingIndex, TMP_Text statusText, bool excludeMouse)
        {
            InputAction action = InputActions.asset.FindAction(actionName);
            if (action == null || action.bindings.Count <= bindingIndex) return;

            if (action.bindings[bindingIndex].isComposite)
            {
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isComposite)
                    DoRebind(action, bindingIndex, statusText, true, excludeMouse);
            }
            else DoRebind(action, bindingIndex, statusText, false, excludeMouse);
        }

        private static void DoRebind(InputAction actionToRebind, int bindingIndex, TMP_Text statusText, bool allCompositeParts, bool excludeMouse)
        {
            if (actionToRebind == null || bindingIndex < 0) return;

            statusText.text = $"Press a key to rebind {actionToRebind.name}...";
            actionToRebind.Disable();

            var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);
            rebind.OnComplete(operation =>
            {
                actionToRebind.Enable();
                operation.Dispose();

                if (allCompositeParts)
                {
                    var nextBindingIndex = bindingIndex + 1;
                    if (nextBindingIndex < actionToRebind.bindings.Count && actionToRebind.bindings[nextBindingIndex].isComposite)
                        DoRebind(actionToRebind, nextBindingIndex, statusText, allCompositeParts, excludeMouse);
                    else statusText.text = $"Rebinded {actionToRebind.name} to {actionToRebind.GetBindingDisplayString(bindingIndex)}";
                }

                SaveBindingOverride(actionToRebind);
                OnRebindComplete?.Invoke();
            });

            rebind.OnCancel(operation =>
            {
                actionToRebind.Enable();
                operation.Dispose();

                OnRebindCancelled?.Invoke();
            });

            rebind.WithCancelingThrough("<Keyboard>/escape");

            if (excludeMouse)
                rebind.WithControlsExcluding("Mouse");

            OnRebindStarted?.Invoke(actionToRebind, bindingIndex);
            rebind.Start();
        }

        public static string GetBindingName(string actionName, int bindingIndex)
        {
            if (InputActions == null) InputActions = new PlayerInputActions();

            InputAction action = InputActions.asset.FindAction(actionName);
            return action.GetBindingDisplayString(bindingIndex);
        }

        private static void SaveBindingOverride(InputAction action)
        {
            if (action == null) return;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                PlayerPrefs.SetString(action.actionMap + action.name + i, action.bindings[i].overridePath);
            }
        }

        public static void LoadBindingOverride(string actionName)
        {
            if (InputActions == null) InputActions = new PlayerInputActions();

            InputAction action = InputActions.asset.FindAction(actionName);
            if (action == null) return;

            for (int i = 0; i < action.bindings.Count; i++)
            {
                string bindingOverridePath = PlayerPrefs.GetString(action.actionMap + action.name + i);
                if (!string.IsNullOrEmpty(bindingOverridePath))
                    action.ApplyBindingOverride(i, bindingOverridePath);
            }
        }

        public static void ResetBinding(string actionName, int bindingIndex)
        {
            if (InputActions == null) InputActions = new PlayerInputActions();

            InputAction action = InputActions.asset.FindAction(actionName);
            if (action == null || action.bindings.Count <= bindingIndex) return;

            if (action.bindings[bindingIndex].isComposite)
            {
                for (int i = bindingIndex; i < action.bindings.Count && action.bindings[i].isComposite; i++)
                    action.RemoveBindingOverride(i);
            }
            else action.RemoveBindingOverride(bindingIndex);

            SaveBindingOverride(action);
        }
    }
}
