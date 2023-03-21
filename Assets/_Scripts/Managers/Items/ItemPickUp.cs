using Unity.VisualScripting;
using UnityEngine;

namespace rene_roid_player {
    public class ItemPickUp : MonoBehaviour
    {
        #region Internal
        private Rigidbody2D _rb;
        private Collider2D _collider;
        private SpriteRenderer _spriteRenderer;

        private Vector2 _initPos;

        private bool _launched = false;
        private float _timeLaunched = 0;
        #endregion

        #region External
        public Item Item;
        #endregion

        void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<Collider2D>();
        }

        void Update()
        {
            if (!_launched)
            {
                _rb.AddForce(new Vector2(Random.Range(-.5f, .5f), Random.Range(.5f, 1f)), ForceMode2D.Impulse);
                
                if (_timeLaunched == 0) _timeLaunched = Time.time + 0.075f;
                if (Time.time > _timeLaunched) _launched = true;
            } else {
                ItemFloat();
                ItemGravity();
            }

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player" && _launched)
            {
                //ItemManager.Instance.AddItem(Item);
                other.GetComponent<PlayerBase>().AddItem(Item);
                Destroy(gameObject);
            }
        }

        private void ItemGravity()
        {
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, Vector2.down, .7f);

            if (hit2D.collider != null) _rb.bodyType = RigidbodyType2D.Static;
        }

        private void ItemFloat()
        {
            if (_rb.bodyType != RigidbodyType2D.Static) return;
            if (_initPos == Vector2.zero) _initPos = transform.position;

            transform.position = new Vector2(_initPos.x, _initPos.y + Mathf.Sin(Time.time * 2f) * 0.1f);
        }

        public void SetItem(Item item)
        {
            Item = item;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer != null && item.Icon != null) _spriteRenderer.sprite = item.Icon;
        }
    }
}
