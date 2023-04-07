using UnityEngine;
using System.Collections;
using rene_roid;
using rene_roid_player;
using System.Collections.Generic;

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
            DragonAI();
        }

        private void DragonAI() {
            var h = _health;
            int f = 0;

            if (h > 0.75f * EnemyBaseStats.Health) {
                FireballAttack();
            }
            else if (h > 0.5f * EnemyBaseStats.Health) {
                FireballAttack();
                TailSlam();
                f = 1;
            }
            else if (h > 0.25f * EnemyBaseStats.Health) {
                FireballAttack();
                TailSlam();
                TimedPetrifiedEnemy();
                f = 2;
            }
            else {
                FireballAttack();
                TailSlam();
                TimedPetrifiedEnemy();
                TimedFireBreath();
                f = 3;
            }

            print(f);
        }

        #region Attacks
        [Header("Fireball!")]
        [SerializeField] private GameObject _fireballPrefab;
        [SerializeField] private Transform _fireballSpawnPoint;
        [SerializeField] private float _fireballSpeed;
        [SerializeField] private float _fireballDamageMultiplier = 0.5f;
        [SerializeField] private float _fireballCooldown;
        private float _fireballCooldownTimer = 0;

        private void FireballAttack()
        {
            if (_fireballCooldownTimer <= 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    var fireball = Instantiate(_fireballPrefab, _fireballSpawnPoint.position, Quaternion.identity);
                    fireball.GetComponent<Fireball>().FireballStats = new Fireball.FireballStatsStruct(_fireballSpeed, _damage * _fireballDamageMultiplier);
                    fireball.GetComponent<Fireball>().PlayerTransform = _targetPlayer.transform;
                    _fireballCooldownTimer = _fireballCooldown;
                }
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
        [SerializeField] private float _firebreathDamageMultiplier = 0.8f;
        [SerializeField] private float _firebreathCooldown;
        private float _firebreathCooldownTimer = 0;

        private void TimedFireBreath() {
            if (_firebreathCooldownTimer <= 0) {
                StartCoroutine(FireBreath());
                _firebreathCooldownTimer = _firebreathCooldown;
            }
            else {
                _firebreathCooldownTimer -= Time.deltaTime;
            }
        }

        private IEnumerator FireBreath() {
            var firebreath = Instantiate(_firebreathPrefab, _firebreathStartPosition.position, Quaternion.identity);

            var script = firebreath.GetComponentInChildren<Firebreath>();
            script.Damage = _damage * _firebreathDamageMultiplier;

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
        [SerializeField] private float _tailDamageMultiplier = 2;

        private float _tailSlamCooldownTimer = 0;
        [SerializeField] private float _tailSlamCooldown = 5;

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

            var col = _tailSlamPrefab.transform.GetChild(0).GetComponent<Collider2D>();

            var rotT = 90;
            var rot =_tailStartRot;
            while (t < _tailSlamTime) {
                t += Time.deltaTime;
                // Rotate tail
                rot = Mathf.Lerp(_tailStartRot, rotT, t / _tailSlamTime);
                // Move tail
                _tailSlamPrefab.transform.rotation = Quaternion.Euler(0, 0, rot);
                _tailSlamSprite.transform.rotation = Quaternion.Euler(0, 0, rot);

                var player = Physics2D.OverlapBoxAll(col.transform.position, col.bounds.size, 0, _playerLayer);
                if (player.Length > 0) {
                    var p = player[0].GetComponent<PlayerBase>();

                    if (p != null) {
                        p.TakeDamage(_damage * _tailDamageMultiplier);
                    }
                }
                
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

        #region Petrified enemy
        [Header("Petrified enemy")]
        [SerializeField] private GameObject _petrifiedEnemyPrefab;
        [SerializeField] private float _petrifiedEnemyCooldown;
        private float _petrifiedEnemyCooldownTimer = 0;

        private List<GameObject> _petrifiedEnemies = new List<GameObject>();

        private void TimedPetrifiedEnemy() {
            if (_petrifiedEnemyCooldownTimer <= 0) {
                PetrifiedEnemy();
                _petrifiedEnemyCooldownTimer = _petrifiedEnemyCooldown;
            }
            else {
                _petrifiedEnemyCooldownTimer -= Time.deltaTime;
            }
        }

        private void PetrifiedEnemy() {
            var petrifiedEnemy = Instantiate(_petrifiedEnemyPrefab, _fireballSpawnPoint.position, Quaternion.identity);
            var rb = petrifiedEnemy.GetComponent<Rigidbody2D>();

            // Add force to petrified enemy up and to the right
            rb.AddForce(new Vector2(1, 1) * Random.Range(100, 200));
            _petrifiedEnemies.Add(petrifiedEnemy);

            StartCoroutine(DeactivateTrigger(petrifiedEnemy));
        }

        private IEnumerator DeactivateTrigger(GameObject petrifiedEnemy) {
            yield return Helpers.GetWait(0.3f);
            petrifiedEnemy.GetComponent<Collider2D>().isTrigger = false;
            
        }
        #endregion
    }
}
