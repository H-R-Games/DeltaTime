using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid_player;

namespace rene_roid_enemy
{
    public class Knife : MonoBehaviour
    {
        private Vector2 _targetPos;
        private bool _onGoing = false;
        public Transform _target;
        public float Damage = 1f;
        [SerializeField] private GameObject _meterBar;

        private void Start() {
            _targetPos = _target.position;

            // Move the z rotation to the target
            Vector3 targetDirZ = _targetPos;
            targetDirZ.z = 0;

            Vector3 objectPosZ = transform.position;
            targetDirZ.x = targetDirZ.x - objectPosZ.x;
            targetDirZ.y = targetDirZ.y - objectPosZ.y;

            float angle = Mathf.Atan2(targetDirZ.y, targetDirZ.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            StartCoroutine(AttackTarget());
        }


        private IEnumerator AttackTarget() {
            var meter = Instantiate(_meterBar, transform.position, Quaternion.identity, transform);
            meter.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, -90);

            var t = 0f;
            var dur = 1f;
            while (t < 1) {
                t += Time.deltaTime / dur;
                meter.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1, 30, 1), t);
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            // Throw the knife
            t = 0f;
            dur = 1f;
            var direction = (_targetPos - (Vector2)transform.position).normalized;
            while (t < 1) {
                t += Time.deltaTime / dur;
                transform.position += (Vector3)direction * Time.deltaTime * 100f;
                yield return null;
            }
        }

        private void OnTriggerEnter2D(Collider2D other) {
            if (other.CompareTag("Player")) {
                other.GetComponent<PlayerBase>().TakeDamage(Damage);
                Destroy(gameObject);
            }
        }
    }
}
