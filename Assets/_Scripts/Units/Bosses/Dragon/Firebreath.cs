using UnityEngine;
using rene_roid_player;

namespace rene_roid_enemy {    
    public class Firebreath : MonoBehaviour
    {
        public float Damage = 10;

        void Start()
        {
            Destroy(gameObject, 5);
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.tag == "Player") {
                other.GetComponent<PlayerBase>().TakeDamage(Damage);
                // Destroy(gameObject);
            }
        }
    }
}
