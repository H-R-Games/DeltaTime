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
            ItemFloat();
            ItemGravity();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                var player = other.GetComponent<PlayerBase>();
                player.AddItem(Item);
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
    }
}
