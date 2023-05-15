using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace rene_roid {
    public class CreditsScene : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _creditsText;
        [SerializeField] private float _creditsSpeed = 1f;
        [SerializeField] private float _targetY = 10000f;
        
        private void Update() {
            _creditsText.transform.Translate(Vector3.up * _creditsSpeed * Time.deltaTime);
            if (_creditsText.transform.position.y > _targetY) {
                SceneManager.LoadScene(0);
            }
        }
    }
}
