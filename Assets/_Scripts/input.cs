using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace rene_roid { public class input : MonoBehaviour { void Start() {  }  void Update() { InputSystem.onAnyButtonPress.CallOnce(ctrl => print(ctrl.name));  } } }