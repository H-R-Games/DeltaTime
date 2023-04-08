using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid;

namespace rene_roid_enemy
{
    public class ChronoWeaver : EnemyBase
    {
        public override void Awake()
        {
            base.Awake();
            _enemyState = EnemyStates.Attack;
        }

        public override void Start()
        {
            base.Start();

            StartAttacks();
        }

        public override void Update()
        {
            base.Update();
            Attack1();
        }

        public override void UpdateState()
        {
        }

        #region Attacks
        private void StartAttacks()
        {
            StartAttack1();
        }

        [Header("Attack 1")]
        [SerializeField] private List<GameObject> _props = new List<GameObject>();

        private void StartAttack1()
        {
            var propParent = GameObject.Find("Props").gameObject.transform;

            for (int i = 0; i < propParent.childCount; i++)
            {
                GameObject prop = propParent.GetChild(i).gameObject;
                _props.Add(prop);
            }
        }

        [SerializeField] private float _attack1DamagePercentage = 1f;
        [SerializeField] private int _attack1Count = 3;
        [SerializeField] private float _attack1Cooldown = 5f;
        private float _attack1Timer = 0f;
        private bool _doingAtatck1 = false;

        private void Attack1()
        {
            if (_doingAtatck1) return;

            print("Attack 1");

            if (_attack1Timer > 0f)
            {
                _attack1Timer -= Time.deltaTime;
                return;
            }

            if (_attack1Timer <= 0f)
            {
                _attack1Timer = _attack1Cooldown;

                print("Attack 1 cooldown");
                // TODO: Start next attack
                StartCoroutine(ThrowProps());
                return;
            }

            IEnumerator ThrowProps()
            {
                _doingAtatck1 = true;
                yield return Helpers.GetWait(1f);

                var playerPos = _targetPlayer.transform.position;
                var playerDir = playerPos - transform.position;

                var props = new List<GameObject>();

                // Select props
                for (int i = 0; i < _attack1Count; i++)
                {
                    var prop = _props[Random.Range(0, _props.Count)];
                    props.Add(prop);
                    _props.Remove(prop);

                    prop.GetComponent<SpriteRenderer>().color = Color.red;

                    var time = 0f;
                    var propInitPos = prop.transform.position;
                    while (time < .4f)
                    {
                        time += Time.deltaTime;
                        prop.transform.position = Vector3.Lerp(propInitPos, propInitPos + Vector3.up, time);
                        yield return null;
                    }

                    // yield return Helpers.GetWait(0.5f);
                }

                yield return Helpers.GetWait(1f);

                // Throw props
                for (int i = 0; i < props.Count; i++)
                {
                    playerPos = _targetPlayer.transform.position;
                    playerDir = playerPos - transform.position;

                    var prop = props[i];
                    var propInitPos = prop.transform.position;
                    var propDir = playerPos - propInitPos;
                    var propSpeed = 100f;

                    var propRb = prop.GetComponent<Rigidbody2D>();
                    propRb.isKinematic = false;

                    yield return Helpers.GetWait(0.1f);

                    propRb.AddForce(propDir.normalized * propSpeed, ForceMode2D.Impulse);

                    // var time = 0f;
                    // while (time < 1f)
                    // {
                    //     time += Time.deltaTime;
                    //     yield return null;
                    // }

                    yield return Helpers.GetWait(0.4f);
                }

                _doingAtatck1 = false;
            }

        }
        #endregion

    }
}
