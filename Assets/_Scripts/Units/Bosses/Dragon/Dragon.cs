using UnityEngine;
using System.Collections;
using rene_roid;

namespace rene_roid_enemy
{
    public class Dragon : EnemyBase
    {
        public override void Awake()
        {
            base.Awake();
            _enemyState = EnemyStates.Attack;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void UpdateState()
        {
            FireballAttack();
        }

        #region Attacks
        [Header("Fireball!")]
        [SerializeField] private GameObject _fireballPrefab;
        [SerializeField] private Transform _fireballSpawnPoint;
        [SerializeField] private float _fireballSpeed;
        [SerializeField] private float _fireballDamage;
        [SerializeField] private float _fireballCooldown;
        private float _fireballCooldownTimer = 0;

        private void FireballAttack()
        {
            if (_fireballCooldownTimer <= 0)
            {
                // for (int i = 0; i < 3; i++)
                // {
                //     var fireball = Instantiate(_fireballPrefab, _fireballSpawnPoint.position, Quaternion.identity);
                //     fireball.GetComponent<Fireball>().FireballStats = new Fireball.FireballStatsStruct(_fireballSpeed, _fireballDamage);
                //     fireball.GetComponent<Fireball>().PlayerTransform = _targetPlayer.transform;
                //     _fireballCooldownTimer = _fireballCooldown;
                // }

                StartCoroutine(FireBreath());
                _fireballCooldownTimer = _fireballCooldown;
            }
            else
            {
                _fireballCooldownTimer -= Time.deltaTime;
            }
        }


        [Header("Firebreath!")]
        [SerializeField] private GameObject _firebreathPrefab;
        [SerializeField] private Transform _firebreathStartPosition;
        [SerializeField] private Transform _firebreathEndPosition;
        [SerializeField] private float _firebreathSpeed;
        [SerializeField] private float _firebreathDamage;
        [SerializeField] private float _firebreathCooldown;
        private float _firebreathCooldownTimer = 0;

        private IEnumerator FireBreath() {
            var firebreath = Instantiate(_firebreathPrefab, _firebreathStartPosition.position, Quaternion.identity);

            var dist = Vector3.Distance(_firebreathStartPosition.position, _firebreathEndPosition.position);
            var t = 0f;
            firebreath.transform.localScale = new Vector2(0, 1);
            while (t < 1) {
                t += Time.deltaTime / _firebreathSpeed;
                
                // Scale gameobject from start to end position
                firebreath.transform.localScale = Vector3.Lerp(new Vector2(0,1), new Vector2(dist, 1), t);

                yield return null;
            }

            yield return Helpers.GetWait(1);
            Destroy(firebreath);
        }

        #endregion

        #region Tail Slam!
        [Header("Tail Slam!")]
        [SerializeField] private GameObject _tailSlamPrefab;
        [SerializeField] private GameObject _tailSlamSprite;
        [SerializeField] private float _tailSlamTime;
        [SerializeField] private float _tailStartRot;

        private float _tailSlamCooldownTimer = 0;
        private float _tailSlamCooldown = 5;

        private void TailSlam() {
            if (_tailSlamCooldownTimer <= 0) {
                StartCoroutine(TailSlamAttack());
                _tailSlamCooldownTimer = _tailSlamCooldown;
            }
            else {
                _tailSlamCooldownTimer -= Time.deltaTime;
            }
        }

        private IEnumerator TailSlamAttack() {
            var t = .0f;

            var rotT = 90;
            var rot =_tailStartRot;
            while (t < _tailSlamTime) {
                t += Time.deltaTime;
                // Rotate tail
                rot = Mathf.Lerp(_tailStartRot, rotT, t / _tailSlamTime);
                // Move tail
                _tailSlamPrefab.transform.rotation = Quaternion.Euler(0, 0, rot);
                _tailSlamSprite.transform.rotation = Quaternion.Euler(0, 0, rot);
                yield return null;
            }

            yield return Helpers.GetWait(1);

            // Return tail to start position
            t = .0f;
            var rotS = 90;
            while (t < _tailSlamTime) {
                t += Time.deltaTime;
                // Rotate tail
                rot = Mathf.Lerp(rotS, _tailStartRot, t / _tailSlamTime);
                // Move tail
                _tailSlamPrefab.transform.rotation = Quaternion.Euler(0, 0, rot);
                _tailSlamSprite.transform.rotation = Quaternion.Euler(0, 0, rot);
                yield return null;
            }

            yield return null;
        }
        #endregion    
    }
}
