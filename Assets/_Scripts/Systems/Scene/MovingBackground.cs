using UnityEngine;

namespace rene_roid {    
    public class MovingBackground : MonoBehaviour
    {
        public float speedX = 0.0f;
        public float speedY = 0.0f;

        private Camera _camera;

        private Vector3 _spriteSize;

        void Start()
        {
            _camera = Helpers.Camera;

            // Set sprite height to camera height
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;
            float cameraHeight = _camera.orthographicSize * 2;
            float scale = cameraHeight / spriteHeight;
            transform.localScale = new Vector3(scale, scale, 1);

            // Set sprite size
            _spriteSize = spriteRenderer.sprite.bounds.size;
        }

        void Update()
        {
            // Move the sprite, and reset it if it's out of the camera
            transform.position += new Vector3(speedX, speedY, 0) * Time.deltaTime;
            if (transform.position.x < -_spriteSize.x)
            {
                transform.position = new Vector3(_spriteSize.x, transform.position.y, transform.position.z);
            }

            if (transform.position.x > _spriteSize.x)
            {
                transform.position = new Vector3(-_spriteSize.x, transform.position.y, transform.position.z);
            }

            if (transform.position.y < -_spriteSize.y)
            {
                transform.position = new Vector3(transform.position.x, _spriteSize.y, transform.position.z);
            }

            if (transform.position.y > _spriteSize.y)
            {
                transform.position = new Vector3(transform.position.x, -_spriteSize.y, transform.position.z);
            }
        }
    }
}
