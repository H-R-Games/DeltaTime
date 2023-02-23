using rene_roid_player;
using UnityEngine;
using System.Collections;

namespace rene_roid {
    public class CameraFollow : MonoBehaviour
    {
        #region Internal
        [Header("Camera Settings")]
        [SerializeField] private Transform _target;
        [SerializeField] [Range(0, 1)] private float _smoothSpeed = 0.125f;
        [SerializeField] private float _currentYoffset;
        [SerializeField] private float _currentXoffset;

        private bool _isFollowing = true;

        private PlayerBase _player;
        #endregion

        private void Start()
        {
            if (_target != null) _player = _target.GetComponent<PlayerBase>();
            _xOffset = 1;
        }

        private void Update() {
            AdaptiveCamera();
            CameraMovement();
        }

        private void LateUpdate()
        {
            UpdateShake();
        }

        private void CameraMovement()
        {
            if (!_isFollowing) return;
            if (_target == null) return;
            Vector3 desiredPosition = new Vector3(_target.position.x + _currentXoffset, _target.position.y + _currentYoffset , transform.position.z);

            if (desiredPosition.y <= 3f) desiredPosition.y = 3f;

            var pos = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
            transform.position = pos;
        }

        #region Cool Camera Stuff
        [Header("Camera Shake")]
        [SerializeField] private float _shakeDuration = 0.5f;
        [SerializeField] private float _shakeMagnitude = 0.5f;
        [SerializeField] private float _dampingSpeed = 1.0f;
        
        private Vector3 _initialPosition;
        private bool _isShaking;
        private float _elapsedShake = 0.0f;

        public void ShakeCamera()
        {
            if (_isShaking) return;
            _initialPosition = transform.localPosition;
            _elapsedShake = _shakeDuration;
            _isShaking = true;
        }

        private void UpdateShake()
        {
            if (!_isShaking) return;
            _initialPosition = transform.localPosition;
            if (_elapsedShake > 0)
            {
                transform.localPosition = _initialPosition + Random.insideUnitSphere * _shakeMagnitude;
                _elapsedShake -= Time.deltaTime * _dampingSpeed;
            }
            else
            {
                _elapsedShake = 0f;
                _isShaking = false;
                transform.localPosition = _initialPosition;
            }
        }

        [Header("Adaptive camera")]
        [SerializeField] private float _adaptiveSpeed;

        [Header("Adaptive camera Y settings")]
        [SerializeField] private float _groundLevelYoffset = 3f;
        [SerializeField] private float _climbingYoffset = 0f;
        [SerializeField] private float _airLevelYoffset = 1f;
        [SerializeField] private float _fallingYoffset = -3f;

        [Header("Adaptive camera X settings")]
        [SerializeField] private float _xOffset = 1f;

        private float _fallingTimer;
        private float _fallingTimerMax = 0.6f;

        private void AdaptiveCamera()
        {
            if (!_isFollowing) return;
            if (_player == null) return;
            if (_player.IsGrounded() && _target.transform.position.y > 2f) _currentYoffset = Mathf.Lerp(_currentYoffset, _airLevelYoffset, _adaptiveSpeed * Time.deltaTime);
            else if (_player.IsGrounded()) _currentYoffset = Mathf.Lerp(_currentYoffset, _groundLevelYoffset, _adaptiveSpeed * Time.deltaTime);
            else
            {
                if (_player.IsClimbing() && _target.transform.position.y < 4f) _currentYoffset = Mathf.Lerp(_currentYoffset, _groundLevelYoffset, _adaptiveSpeed * Time.deltaTime);
                if (_player.IsClimbing()) _currentYoffset = Mathf.Lerp(_currentYoffset, _climbingYoffset, _adaptiveSpeed * Time.deltaTime);
                else if (isFalling()) _currentYoffset = Mathf.Lerp(_currentYoffset, _fallingYoffset, _adaptiveSpeed * Time.deltaTime);
            }

            if (_player.IsGrounded() || _player.IsClimbing()) _fallingTimer = _fallingTimerMax;

            if (Mathf.Abs(_player.Input.x) > 0.1f) _xOffset = Mathf.Abs(_xOffset) * Mathf.CeilToInt(_player.Input.x);
            _currentXoffset = Mathf.Lerp(_currentXoffset, _xOffset, _adaptiveSpeed * Time.deltaTime);

            bool isFalling() {
                if (_player.IsGrounded())
                {
                    _fallingTimer = _fallingTimerMax;
                    return false;
                }

                if (_fallingTimer > 0)
                {
                    _fallingTimer -= Time.deltaTime;
                    return false;
                }
                else return true;
            }
        }
        

        public void MoveCameraToPosition(Vector3 position, float time)
        {
            _isFollowing = false;
            StartCoroutine(MoveCamera(position, time));
        }

        private IEnumerator MoveCamera(Vector3 position, float time)
        {
            var t = 0f;
            var startPos = transform.position;
            while (t < 1)
            {
                t += Time.deltaTime / time;
                transform.position = Vector3.Lerp(startPos, position + new Vector3(0, 0, transform.position.z), t);
                yield return null;
            }
        }

        public void ReturnToPlayer()
        {
            _isFollowing = true;
        }
        #endregion
    }
}
