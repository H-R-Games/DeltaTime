using System.Collections.Generic;
using UnityEngine;

namespace rene_roid {    
    public class MainMenuUI : MonoBehaviour
    {
        void Start()
        {
            SelectCharacterStart();
        }
        
        void Update()
        {
            
        }

        #region Select Character
        [Header("Select Character")]
        public GameObject _selectCharacterParent;
        private List<GameObject> _selectCharacterButtons = new List<GameObject>();

        private void SelectCharacterStart() {
            _selectCharacterButtons = new List<GameObject>();
            foreach (Transform child in _selectCharacterParent.transform) {
                _selectCharacterButtons.Add(child.gameObject);
            }
        }

        public void SelectCharacter(int character) {
            var selecCol = Color.red;
            var unselecCol = Color.black;

            for (int i = 0; i < _selectCharacterButtons.Count; i++) {
                var button = _selectCharacterButtons[i];
                var buttonImage = button.GetComponent<UnityEngine.UI.Image>();
                var buttonColor = buttonImage.color;

                if (i == character) {
                    buttonColor = selecCol;
                } else {
                    buttonColor = unselecCol;
                }

                buttonImage.color = buttonColor;
            }
        }
        #endregion
    }
}
