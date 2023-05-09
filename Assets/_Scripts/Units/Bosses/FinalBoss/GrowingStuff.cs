using System.Collections;
using UnityEngine;
using rene_roid;

namespace rene_roid_enemy {    
    public class GrowingStuff : MonoBehaviour
    {
        public Transform _target;
        private bool _shot = false;

        void Start()
        {
            
        }
        
        void Update()
        {
            if (_target == null) return;

            if (_target != null && !_shot) {
                _shot = true;
                var rb = GetComponent<Rigidbody2D>();
                var dir = _target.position - transform.position;
                rb.AddForce(dir.normalized * 50000f, ForceMode2D.Impulse);
                StartCoroutine(Grow());
            }
        }

        private IEnumerator Grow() {
            yield return Helpers.GetWait(0.5f);
            while (transform.localScale.x < 1f) {
                transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return Helpers.GetWait(0.1f);
            }
            
            yield return Helpers.GetWait(7f);
            while (transform.localScale.x > 0f) {
                transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                yield return Helpers.GetWait(0.1f);
            }
            Destroy(gameObject);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit) {
            if (hit.gameObject.CompareTag("Player")) {
                print("hit");
            }
        }
    }
}
