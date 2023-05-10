using UnityEngine;

namespace rene_roid {    
    public class Parallax : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private float _parallaxFactor = 0.5f;

        void Start()
        {
            _camera = Helpers.Camera;

            // Set sprite height to camera height
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            float spriteHeight = spriteRenderer.sprite.bounds.size.y;
            float cameraHeight = _camera.orthographicSize * 2;
            float scale = cameraHeight / spriteHeight;
            transform.localScale = new Vector3(scale, scale, 1);
        }
        
        void Update()
        {
            
        }

        void LateUpdate()
        {
            Vector3 cameraPosition = _camera.transform.position;
            Vector3 parallaxPosition = new Vector3(cameraPosition.x * _parallaxFactor, cameraPosition.y, transform.position.z);
            transform.position = parallaxPosition;
        }
    }
}
