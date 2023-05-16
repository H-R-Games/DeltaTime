using UnityEngine;
using TMPro;

namespace rene_roid_player
{
    public class Chest : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        private SpriteRenderer _spriteRenderer;
        [SerializeField] private Sprite _closedChest;
        [SerializeField] private Sprite _openedChest;
        private Transform _player;
        private PlayerBase _playerBase;
        private bool _isOpened = false;

        [SerializeField] private TMP_Text _moneyCostText;
        [SerializeField] private float _moneyCost = 100;
        public float SetMoneyCost { set { _moneyCost = value; } }
        #endregion

        #region External
        [Header("External")]
        public GameObject Item;
        public Item[] CommonItems;
        public Item[] RareItems;
        public Item[] MythicItems;
        public Item[] LegendaryItems;

        public float CommonChance;
        public float RareChance;
        public float MythicChance;
        public float LegendaryChance;

        public float LuckChance;
        #endregion

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            _spriteRenderer.sprite = _closedChest;

            _moneyCostText.text = _moneyCost.ToString() + " $";

            // Set the chance to 100% if the sum of all chances is less than 100%
            float sum = CommonChance + RareChance + MythicChance + LegendaryChance;
            if (sum < 100)
            {
                float diff = 100 - sum;
                CommonChance += diff;
            }

            _playerBase = _player.GetComponent<PlayerBase>();
        }

        private void Update()
        {
            if (_player == null) _player = GameObject.FindGameObjectWithTag("Player").transform;
            if (_player != null && _playerBase == null) _playerBase = _player.GetComponent<PlayerBase>();

            if (Vector2.Distance(transform.position, _player.position) < 1.5f && !_isOpened)
            {

                print("Press F to open chest");
                if ( _playerBase.Money >= _moneyCost) {
                    _isOpened = true;
                    _spriteRenderer.sprite = _openedChest;
                    LuckChance = _playerBase.Luck;
                    OpenChest();

                    _player.GetComponent<PlayerBase>().Money -= _moneyCost;
                }
            }
        }

        private void OpenChest()
        {
            float random = Random.Range(0, 100);
            var item = null as Item;

            random += LuckChance;

            if (random < CommonChance) item = CommonItems[Random.Range(0, CommonItems.Length)];
            else if (random < CommonChance + RareChance) item = RareItems[Random.Range(0, RareItems.Length)];
            else if (random < CommonChance + RareChance + MythicChance) item = MythicItems[Random.Range(0, MythicItems.Length)];
            else if (random < CommonChance + RareChance + MythicChance + LegendaryChance) item = LegendaryItems[Random.Range(0, LegendaryItems.Length)];

            if (item != null)
            {
                var itemPickUp = Instantiate(Item, transform.position, Quaternion.identity).GetComponent<ItemPickUp>();
                itemPickUp.Item = item;
            } else {
                var itemPickUp = Instantiate(Item, transform.position, Quaternion.identity).GetComponent<ItemPickUp>();
                itemPickUp.Item = CommonItems[0];
            }

            Destroy(gameObject, 0.5f);
        }
    }
}
