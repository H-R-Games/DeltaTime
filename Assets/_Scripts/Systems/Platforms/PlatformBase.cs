using UnityEngine;
using rene_roid_player;

namespace rene_roid {    
    [RequireComponent(typeof(Rigidbody2D))]
    public abstract class PlatformBase : MonoBehaviour
    {
        private Rigidbody2D _player;
        protected Rigidbody2D _rb;
        protected Vector2 _startPos;
        protected Vector2 _lastPos;

        protected virtual void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _startPos = _rb.position;
        }

        protected private void FixedUpdate() {
            var newPos = _rb.position;
            var change = newPos - _lastPos;
            _lastPos = newPos;

            //MovePlayer(change);
        }

        protected virtual void OnCollisionEnter2D(Collision2D other) {
            if (other.transform.TryGetComponent<PlayerBase>(out var player)) {
                _player = player.GetComponent<Rigidbody2D>();
                // MovePlayer(Vector2.zero);
            }
        }
    }
}
