using System.Collections;
using UnityEngine;

namespace rene_roid {
    public class CloneFade : MonoBehaviour
    {
        #region Internal
        private SpriteRenderer _renderer;
        private float _fadeTime = 1f;
        private Sprite _sprite;
        #endregion

        public void Fade()
        {
            StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float startAlpha = _renderer.color.a;
            float rate = 1.0f / _fadeTime;
            float progress = 0.0f;
            while (progress < 1.0)
            {
                _renderer.color = new Color(_renderer.color.r, _renderer.color.g, _renderer.color.b, Mathf.Lerp(startAlpha, 0, progress));
                progress += rate * Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        public void SetFadeTime(float fadeTime)
        {
            _fadeTime = fadeTime;
        }

        public void SetSprite(Sprite sprite)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _sprite = sprite;
            _renderer.sprite = _sprite;
        }

        public void SetSprite(Sprite sprite, bool flipX)
        {
            _renderer = GetComponent<SpriteRenderer>();
            _sprite = sprite;
            _renderer.sprite = _sprite;
            _renderer.flipX = flipX;
        }

    }
}
