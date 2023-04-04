using UnityEngine;
using rene_roid_player;

namespace rene_roid {    
    public class DamageZone : MonoBehaviour
    {
        public float Damage = 10f;
        private Transform _respawn;

        private void Start() {
            _respawn = transform.GetChild(0);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.CompareTag("Player")) {
                other.gameObject.transform.position = _respawn.position;

                var player = other.gameObject.GetComponent<PlayerBase>();
                player.TakeDamage(Damage);
                player.TakeAwayControl(true);
                player.ReturnControl();
            }
        }
    }
}
