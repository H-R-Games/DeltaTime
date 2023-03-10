using rene_roid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
        private Camera _camera;
        private float _length, _startPos;
        [SerializeField] private float _parallaxFactor;
        public bool Child = false;

        void Start()
        {
            _camera = Helpers.Camera;

            // Set sprite height to camera height
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;
            float cameraHeight = _camera.orthographicSize * 2;
            float scale = cameraHeight / spriteHeight;

            if (!Child) transform.localScale = new Vector3(scale, scale, 1);

            _startPos = transform.position.x;
            _length = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        void Update()
        {
            float temp = (_camera.transform.position.x * (1 - _parallaxFactor));
            float dist = (_camera.transform.position.x * _parallaxFactor);

            transform.position = new Vector3(_startPos + dist, _camera.transform.position.y, transform.position.z);

            if (temp > _startPos + _length) _startPos += _length;
            else if (temp < _startPos - _length) _startPos -= _length;
        }
    
}

