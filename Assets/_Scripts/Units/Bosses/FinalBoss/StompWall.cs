using System.Collections;
using UnityEngine;
using rene_roid;

namespace rene_roid_enemy {    
    public class StompWall : MonoBehaviour
    {
        public float Speed = 5f;
        public float Cd = 5f;
        private bool _go = false;

        void Start()
        {
            StartCoroutine(Stomp());

            IEnumerator Stomp() {
                yield return null;
                var z = Random.Range(-1f, 1f);
                z = z > 0 ? 1 : -1;

                var fScale = new Vector3(1, 1, z);
                
                var t = 0f;
                var cd = 0.2f;
                while (t < 1) {
                    t += Time.deltaTime / cd;
                    transform.localScale = Vector3.Lerp(new Vector3(0, 0, 0), fScale, t);
                    yield return null;
                }

                yield return Helpers.GetWait(Cd * 0.5f - cd);
                _go = true;

                Destroy(gameObject, 5f);
            }   
        }

        private void Update() {
            if (_go) {
                transform.position += Vector3.down * Time.deltaTime * Speed;
            }
        }
    }
}
