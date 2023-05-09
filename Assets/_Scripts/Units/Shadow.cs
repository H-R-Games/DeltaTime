using System.Collections;
using UnityEngine;

namespace rene_roid {    
    public class Shadow : MonoBehaviour
    {
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] SpriteRenderer _spriteRenderer;
        private void Start() {
            StartCoroutine(SpawnShadows());

            IEnumerator SpawnShadows() {
                // Fade out sprite
                float t = 0f;
                while (t < 1) {
                    t += Time.deltaTime / _fadeDuration;
                    _spriteRenderer.color = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
                    yield return null;
                }
            }

            Destroy(this.gameObject, _fadeDuration);
        }
    }
}
