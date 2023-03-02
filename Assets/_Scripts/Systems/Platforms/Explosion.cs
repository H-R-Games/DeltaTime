using UnityEngine;
using rene_roid_player;

namespace rene_roid
{
    public class Explosion : MonoBehaviour
    {
        [SerializeField] private float _explosionForce = 50;
        [SerializeField] private bool _pushOffeset = false;

        [SerializeField] private bool _growing = false;
        [SerializeField] private float _growSpeed = 1;
        [SerializeField] private float _growSize = 1;

        private void Update()
        {
            if (!_growing) return;
            var scale = (Mathf.Sin(Time.time * _growSpeed) + 2) * _growSize;
            transform.localScale = new Vector3(scale, scale, scale);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.TryGetComponent<PlayerBase>(out var player)) {
                if (_pushOffeset) {
                    var dir = (Vector2) other.transform.position + other.offset - (Vector2) transform.position;
                    player.ApplyVelocity(dir.normalized * _explosionForce, PlayerForce.Decay);
                } else {
                    player.ApplyVelocity(Vector2.up * _explosionForce, PlayerForce.Decay);
                }
            }
        }
    }
}
