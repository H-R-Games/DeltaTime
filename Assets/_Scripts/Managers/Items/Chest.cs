using UnityEngine;

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
        #endregion

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        private void Start()
        {
            _spriteRenderer.sprite = _closedChest;

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
            if (Vector2.Distance(transform.position, _player.position) < 1.5f && !_isOpened)
            {
                print("Press F to open chest");
                if ( _playerBase.Money >= 100) {
                    _isOpened = true;
                    _spriteRenderer.sprite = _openedChest;
                    OpenChest();

                    _player.GetComponent<PlayerBase>().Money -= 100;
                }
            }
        }

        private void OpenChest()
        {
            float random = Random.Range(0, 100);
            var item = null as Item;

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
        }
    }
}
