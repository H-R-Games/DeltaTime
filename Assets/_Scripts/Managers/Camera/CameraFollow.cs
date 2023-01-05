using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace rene_roid {
    public class CameraFollow : MonoBehaviour
    {
        #region Internal
        [Header("Camera Settings")]
        [SerializeField] private Transform _target;
        [SerializeField] [Range(0, 1)] private float _smoothSpeed = 0.125f;
        [SerializeField] private float _yOffset;

        #endregion

        private void Start()
        {
            
        }

        private void Update()
        {
            CameraMovement();
        }

        private void CameraMovement()
        {
            if (_target == null) return;
            var pos = Vector3.Lerp(transform.position, new Vector3(_target.position.x, _target.position.y, transform.position.z), _smoothSpeed);
            transform.position = pos;
        }
    }
}
