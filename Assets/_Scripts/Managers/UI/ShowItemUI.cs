using UnityEngine;
using UnityEngine.UI;
using TMPro;
using rene_roid_player;

namespace rene_roid {    
    public class ShowItemUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private TextMeshProUGUI _itemName;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [SerializeField] private GameObject _panel;

        [SerializeField] private GameObject _image;
        [SerializeField] private GameObject _images;

        private float _timeToHide = 3f;

        private void Start() {
            HideItem();
        }

        public void DisplayItem(Item item) {
            _itemImage.enabled = true;
            _itemName.enabled = true;
            _itemDescription.enabled = true;

            _panel.SetActive(true);

            _itemImage.sprite = item.Icon;
            _itemName.text = item.Name;
            _itemDescription.text = item.Description;

            _timeToHide = 3f;

            if (_images == null) return;

            var img = Instantiate(_image, _images.transform);
            img.GetComponent<Image>().sprite = item.Icon;
        }

        public void HideItem() {
            _itemImage.sprite = null;
            _itemName.text = "";
            _itemDescription.text = "";

            _panel.SetActive(false);

            _itemImage.enabled = false;
            _itemName.enabled = false;
            _itemDescription.enabled = false;
        }

        private void Update() {
            if (_itemName.text != "") {
                _timeToHide -= Time.deltaTime;
                if (_timeToHide <= 0) {
                    HideItem();
                    _timeToHide = 3f;
                }
            }
        }
    }
}
