using UnityEngine;
using System.Collections;

namespace rene_roid_player {    
    public class ShadowClone : MonoBehaviour
    {
        #region Internal
        private SpriteRenderer _renderer;
        private Sprite _sprite;
        private Vector2 _direction;
        #endregion

        #region External
        [Header("External")]
        public Sprite _shadowCloneSprite1;
        public Sprite _shadowCloneSprite2;
        #endregion

        public void SetFlipX(bool flipX)
        {
            var renderer = GetComponent<SpriteRenderer>();
            renderer.flipX = flipX;
            _direction = flipX ? Vector2.left : Vector2.right;
        }

        // Dash fast in the direction the player is facing and then fade out
        public void Dash(float speed, float fadeTime)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _renderer.sprite = _shadowCloneSprite1;
            StartCoroutine(DashAndFade(speed, fadeTime));
        }

        private IEnumerator DashAndFade(float speed, float fadeTime)
        {
            float startAlpha = _renderer.color.a;
            float rate = 1.0f / fadeTime;
            float progress = 0.0f;
            while (progress < 1.0)
            {
                transform.Translate(_direction * speed * Time.deltaTime);
                // _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(startAlpha, 0, progress));
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, .7f);
                progress += rate * Time.deltaTime;


                yield return null;
            }

            // In the end of the dash, change the sprite to the fade out sprite2
            _renderer.sprite = _shadowCloneSprite2;
            var t = 1f;
            var fadet = 0.3f;
            while (t > 0)
            {
                t -= Time.deltaTime / fadet;
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, t);
                yield return null;
            }

            Destroy(gameObject);
        }
    }
}
