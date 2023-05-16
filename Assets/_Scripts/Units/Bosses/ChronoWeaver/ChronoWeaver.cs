using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid;

namespace rene_roid_enemy
{
    public class ChronoWeaver : EnemyBase
    {
        [Header("Chrono Weaver")]
        [SerializeField] private Sprite _idleSprite;
        [SerializeField] private Sprite _attackSprite;
        [SerializeField] private Sprite _moveSprite;

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private int _currentAttackCount = 0;
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
            PlayAnimations();
            base.Update();
            Move();

            Attack1();
            Attack2();

            Attack4();
        }

        public override void UpdateState()
        {
        }

        private void PlayAnimations() {
            if (_doingAtatck1 || _doingAtatck2) {
                _spriteRenderer.sprite = _attackSprite;
            } else if (_moving) {
                _spriteRenderer.sprite = _moveSprite;
            } else {
                _spriteRenderer.sprite = _idleSprite;
            }
        }

        #region Moving
        [Header("Move")]
        [SerializeField] private List<Transform> _movePoints = new List<Transform>();
        [SerializeField] private float _moveSpeed = 5f;

        private int _currentMovePoint = 0;
        private bool _moving = false;

        private void Move()
        {
            if (_moving) return;
            if (_doingAtatck1 || _doingAtatck2) return;

            if (_currentMovePoint >= _movePoints.Count)
            {
                _currentMovePoint = 0;
            }

            var movePoint = _movePoints[_currentMovePoint];
            var moveDir = movePoint.position - transform.position;

            StartCoroutine(MoveTo(movePoint.position));

            IEnumerator MoveTo(Vector3 targetPos)
            {
                _moving = true;

                var time = 0f;
                var startPos = transform.position;
                var finalPos = targetPos;
                var midPoint = (startPos + finalPos) / 2f + (Vector3.up * 5f);

                while (time < 1f)
                {
                    var pos = CalculateBezierCuadraticPoint(startPos, midPoint, finalPos, time);

                    transform.position = pos;

                    time += Time.deltaTime * _moveSpeed;

                    yield return null;
                }

                _currentMovePoint++;

                yield return Helpers.GetWait(1f);
                _moving = false;
            }

            Vector2 CalculateBezierCuadraticPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t) {
                var u = 1 - t;
                var tt = t * t;
                var uu = u * u;
                var uuu = uu * u;
                var ttt = tt * t;

                var p = uuu * p0; //first term
                p += 3 * uu * t * p1; //second term
                p += 3 * u * tt * p2; //third term
                p += ttt * p2; //fourth term

                return p;
            }
        }
        #endregion

        #region Attacks
        private void StartAttacks()
        {
            StartAttack1();
            StartAttack2();
            StartAttack4();
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
            if (_props.Count == 0) return;
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
                _currentAttackCount++;
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
        

        [Header("Attack 2")]
        private Transform _spawnPropsYPos;
        private void StartAttack2()
        {
            _spawnPropsYPos = GameObject.Find("Trash").transform;
        }

        [SerializeField] private float _attack2DamagePercentage = 1f;
        [SerializeField] private int _attack2Count = 10;
        [SerializeField] private float _attack2Cooldown = 10f;
        private float _attack2Timer = 0f;
        private bool _doingAtatck2 = false;

        private void Attack2()
        {
            if (_doingAtatck2) return;

            print("Attack 2");

            if (_attack2Timer > 0f)
            {
                _attack2Timer -= Time.deltaTime;
                return;
            }

            if (_attack2Timer <= 0f)
            {
                _attack2Timer = _attack2Cooldown;

                print("Attack 2 cooldown");
                _currentAttackCount++;
                // TODO: Start next attack
                StartCoroutine(RoundProps());
                return;
            }

            IEnumerator RoundProps() {
                _doingAtatck2 = true;
                yield return Helpers.GetWait(1f);

                var playerPos = _targetPlayer.transform.position;
                
                var props = new List<GameObject>();
                for (int i = 0; i < _attack2Count; i++)
                {
                    var posProp = new Vector3(playerPos.x + Random.Range(-5f, 5f), _spawnPropsYPos.position.y, 0f);
                    var prop = Instantiate(_props[Random.Range(0, _props.Count)], posProp, Quaternion.identity);
                    props.Add(prop);
                }

                // yield return Helpers.GetWait(1f);

                // Place props in circle around player smoothly
                var playerDir = playerPos - transform.position;
                var angle = 0f;
                var angleStep = 360f / _attack2Count;
                var radius = 5f;

                for (int i = 0; i < props.Count; i++)
                {
                    // Change sprite color
                    props[i].GetComponent<SpriteRenderer>().color = Color.red;

                    var prop = props[i];
                    var propInitPos = prop.transform.position;
                    var propDir = playerPos - propInitPos;

                    var propRb = prop.GetComponent<Rigidbody2D>();

                    var time = 0f;
                    while (time < 1f)
                    {
                        time += Time.deltaTime * 10f;
                        var pos = new Vector3(playerPos.x + Mathf.Cos(angle) * radius, playerPos.y + Mathf.Sin(angle) * radius, 0f);
                        prop.transform.position = Vector3.Lerp(propInitPos, pos, time);
                        yield return null;
                    }

                    angle += angleStep;
                }

                yield return Helpers.GetWait(0.5f);

                // Throw props to the center at the same time
                var t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 10f;
                    for (int i = 0; i < props.Count; i++)
                    {
                        var prop = props[i];
                        var propInitPos = prop.transform.position;
                        var propDir = playerPos - propInitPos;
                        var propSpeed = 10f;

                        var propRb = prop.GetComponent<Rigidbody2D>();
                        propRb.isKinematic = false;

                        propRb.AddForce(propDir.normalized * propSpeed, ForceMode2D.Impulse);
                    }

                    yield return null;
                }

                _doingAtatck2 = false;
            }

        }


        [Header("Attack 4")]
        [SerializeField ] private GameObject _spikes;
        private int _roundsCD = 3;
        private int _roundsDuration = 1;
        private int _roundCount = 0;
        private bool _spikesOn = false;
        private int _lastRound = 0;

        private void StartAttack4()
        {
            _roundCount = _roundsCD;
            _lastRound = _currentAttackCount;
        }

        private void Attack4() {
            if (!_spikesOn) { // In cooldown
                if (_lastRound != _currentAttackCount) {
                    _lastRound = _currentAttackCount;
                    _roundCount--;
                }

                if (_roundCount <= 0) {
                    _roundCount = _roundsDuration;
                    _spikesOn = true;
                    _spikes.SetActive(true);
                }
            } else {
                if (_lastRound != _currentAttackCount) {
                    _lastRound = _currentAttackCount;
                    _roundCount--;
                }

                if (_roundCount <= 0) {
                    _roundCount = _roundsCD;
                    _spikesOn = false;
                    _spikes.SetActive(false);
                }
            }
        }
        #endregion

    }
}
