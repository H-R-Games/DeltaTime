using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rene_roid;

namespace rene_roid_enemy {
    public class FinalBoss : EnemyBase
    {
        private int _phase = 1;
        private Rigidbody2D _rb;

        public override void Start() {
            base.Start();
            _rb = GetComponent<Rigidbody2D>();
            _lastHP = _health;
            _animator = GetComponentInChildren<Animator>();
        }

        public override void Update() {
            base.Update();
            HandleAnimations();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            FinalBossAI();
        }


        private void FinalBossAI() {
            if (_phase < 3 && _health <= EnemyBaseStats.Health * 0.05f) {
                _health = EnemyBaseStats.Health;
                _phase++;
            }

            Movement();

            switch (_phase)
            {
                case 1:
                    break;
                case 2:
                    // Dissable skills
                    DissableSkills();
                    // Spawn bosses
                    break;
                case 3:
                    // ZA WARUDO
                    ZaWarudo();
                    break;
                default:
                    break;
            }

            if (ZaWarudoActive) return;
            Dodge();
            GroingStuff();
            PlayerTimeTravel();
            Knifes();
        }

        #region Movement
        [Header("Movement")]
        [SerializeField] private float _speed = 5f;
        
        private void Movement() {
            var dir = _targetPlayer.transform.position - transform.position;
            dir.Normalize();
            dir = new Vector2(dir.x, 0);

            if (dir.x > 0) {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (dir.x < 0) {
                transform.localScale = new Vector3(1, 1, 1);
            }

            if (Vector2.Distance(transform.position, _targetPlayer.transform.position) > 5f) {
                _rb.velocity = dir * _speed;
            }
            else if (Vector2.Distance(transform.position, _targetPlayer.transform.position) < 3f) {
                _rb.velocity = -dir * _speed * 0.75f;
            }
            else {
                _rb.velocity = Vector2.zero;
            }
        }
        #endregion

        #region Dodge
        [Header("Dodge")]
        [SerializeField] private float _dodgeDuration = 5f;
        [SerializeField] private float _dodgeCooldown = 10f;
        [SerializeField] private float _dodgeyDamage = 1f;

        private float _dodgeTimer = 0f;
        private float _dodgeCooldownTimer = 0f;
        private bool _dodgeActive = false;

        private void Dodge() {
            if (_dodgeActive) {
                _dodgeTimer += Time.deltaTime;
                if (_dodgeTimer >= _dodgeDuration) {
                    _dodgeActive = false;
                    _dodgeTimer = 0f;
                }
            }
            else {
                _dodgeCooldownTimer += Time.deltaTime;
                if (_dodgeCooldownTimer >= _dodgeCooldown) {
                    _dodgeActive = true;
                    _dodgeCooldownTimer = 0f;
                    _lastHP = _health;
                }
            }
        }

        private float _lastHP = 0f;
        private void InDodge() {
            if (_dodgeActive) {
                if (_lastHP > _health) {
                    // Get behind the player
                    var player = _targetPlayer.transform.position;
                    var dir = player - transform.position;
                    dir.Normalize();
                    var fPos = player + dir * 2f;
                    transform.position = new Vector2(fPos.x, transform.position.y);
                    _lastHP = _health;
                    
                }
            }
        }
        #endregion
    
        #region Growing Stuff
        [Header("Growing Stuff")]
        [SerializeField] private GameObject _groingStuff;
        private float _groingStuffDamage = 1f;

        [SerializeField] private float _groingStuffCooldown = 5f;
        private float _groingStuffCooldownTimer = 0f;

        [SerializeField] private int _groingStuffQuantity = 7;

        private void GroingStuff() {
            _groingStuffCooldownTimer += Time.deltaTime;
            if (_groingStuffCooldownTimer >= _groingStuffCooldown) {
                _groingStuffCooldownTimer = 0f;
                StartCoroutine(GroingStuffCoroutine());
            }


            IEnumerator GroingStuffCoroutine() {
                List<GameObject> stuffs = new List<GameObject>();
                for (int i = 0; i < _groingStuffQuantity; i++) {
                    var pos = transform.position;
                    pos.x += Random.Range(-1f, 1f);
                    pos.y += Random.Range(-1f, 1f);
                    var stuff = Instantiate(_groingStuff, pos, Quaternion.identity);
                    stuffs.Add(stuff);
                    
                    if (Random.Range(0, 2) == 0) {
                        stuff.GetComponent<BoxCollider2D>().isTrigger = true;
                        stuff.GetComponent<SpriteRenderer>().color = Color.blue;
                    }

                    yield return new WaitForSeconds(0.1f);
                }

                foreach (var stuff in stuffs) {
                    stuff.GetComponent<GrowingStuff>()._target = _targetPlayer.transform;
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
        #endregion

        #region Player Time Travel
        [Header("Player Time Travel")]
        [SerializeField] private float _playerTimeTravelCooldown = 40f;
        private float _playerTimeTravelCooldownTimer = 0f;

        [SerializeField] private float _playerTimeTravelDuration = 5f;
        [SerializeField] private GameObject _stompPrefab;
        private Transform _playerTimeTravelTarget = null;

        private void PlayerTimeTravel() {
            _playerTimeTravelCooldownTimer += Time.deltaTime;
            if (_playerTimeTravelCooldownTimer >= _playerTimeTravelCooldown) {
                _playerTimeTravelCooldownTimer = 0f;
                StartCoroutine(PlayerTimeTravelCoroutine());
            }

            IEnumerator PlayerTimeTravelCoroutine() {
                var playerPos = _targetPlayer.transform.position;
                var spriteRenderer = this.GetComponentInChildren<SpriteRenderer>();
                var srColor = spriteRenderer.color;
                
                // Make sprite flash red and then return to original color
                var t = 0f;
                var dur = 0.5f;
                while (t < 1) {
                    t += Time.deltaTime / dur;
                    spriteRenderer.color = Color.Lerp(srColor, Color.red, t);
                    yield return null;
                }

                playerPos = _targetPlayer.transform.position;
                t = 0f;
                while (t < 1) {
                    t += Time.deltaTime / dur;
                    spriteRenderer.color = Color.Lerp(Color.red, srColor, t);
                    yield return null;
                }

                yield return new WaitForSeconds(_playerTimeTravelDuration * 0.5f);
                var gameobj = Instantiate(_stompPrefab, playerPos + new Vector3(0, 5, 0), Quaternion.identity);
                gameobj.GetComponent<StompWall>().Cd = _playerTimeTravelDuration;
                gameobj.GetComponent<StompWall>().Damage = _damage;
                yield return new WaitForSeconds(_playerTimeTravelDuration * 0.5f);

                _targetPlayer.transform.position = playerPos;
            }
        }
        #endregion

        #region Dissable Skills
        [Header("Dissable Skills")]
        [SerializeField] private float _dissableSkillsCooldown = 30f;
        private float _dissableSkillsCooldownTimer = 0f;

        [SerializeField] private float _dissableSkillsDuration = 15f;

        private void DissableSkills() {
            _dissableSkillsCooldownTimer += Time.deltaTime;
            if (_dissableSkillsCooldownTimer >= _dissableSkillsCooldown) {
                _dissableSkillsCooldownTimer = 0f;
                _targetPlayer.AddSkillsCooldown(_dissableSkillsDuration);
            }
        }
        #endregion
    
        #region Za Warudo
        [Header("Za Warudo")]
        [SerializeField] private float _zaWarudoCooldown = 60f;
        private float _zaWarudoCooldownTimer = 0f;

        [SerializeField] private float _zaWarudoDuration = 10f;
        [SerializeField] private Shark _sharkPrefab;
        [SerializeField] private int _sharkCount = 5;
        public bool ZaWarudoActive = false;

        private void ZaWarudo() {
            _zaWarudoCooldownTimer += Time.deltaTime;
            if (_zaWarudoCooldownTimer >= _zaWarudoCooldown) {
                _zaWarudoCooldownTimer = 0f;
                StartCoroutine(ZaWarudoCoroutine());
            }
            
            IEnumerator ZaWarudoCoroutine() {
                _targetPlayer.TakeAwayControl();
                ZaWarudoActive = true;

                var dirPlayer = _targetPlayer.transform.position - transform.position;
                dirPlayer.Normalize();
                var pos = transform.position + -dirPlayer * 30f;

                // Spawn sharks
                for (int i = 0; i < _sharkCount; i++)
                {
                    var randx = Random.Range(-1f, 1f);
                    var randy = Random.Range(-4f, 4f);
                    pos.x += randx;
                    pos.y += randy;
                    Shark shark = Instantiate(_sharkPrefab, pos, Quaternion.identity);
                    shark.Damage = _damage;
                    shark._boss = this;
                    shark._target = _targetPlayer.transform;

                }

                yield return Helpers.GetWait(_zaWarudoDuration);
                _targetPlayer.ReturnControl();
                ZaWarudoActive = false;
            }
        }
        #endregion

        #region Time Travel
        [Header("Time Travel")]
        [SerializeField] private float _timeTravelCooldown = 60f;
        private float _timeTravelCooldownTimer = 0f;

        [SerializeField] private float _timeTravelDuration = 10f;
        [SerializeField] private SpriteRenderer _timeTravelPrefab;
        private void TimeTravel() {
            // When activated save the position and after the duration return to that position
            _timeTravelCooldownTimer += Time.deltaTime;
            if (_timeTravelCooldownTimer >= _timeTravelCooldown) {
                _timeTravelCooldownTimer = 0f;
                StartCoroutine(TimeTravelCoroutine());
            }

            IEnumerator TimeTravelCoroutine() {
                var sr = this.GetComponentInChildren<SpriteRenderer>();
                var pos = transform.position;
                var t = 0f;
                var dur = _timeTravelDuration;

                StartCoroutine(ShadowCoroutine(dur));

                var hp = _health;

                while (t < 1) {
                    t += Time.deltaTime / dur;
                    yield return null;
                }
                //yield return Helpers.GetWait(_timeTravelDuration);
                transform.position = pos;


                IEnumerator ShadowCoroutine(float dur) {
                    var count = 0;
                    var t2 = dur / 20f;

                    while (count < 20) {
                        count++;
                        var shadow = Instantiate(_timeTravelPrefab, transform.position, Quaternion.identity);
                        shadow.sprite = sr.sprite;
                        yield return Helpers.GetWait(t2);
                    }
                }

                var hplost = hp - _health;
                _health = hplost * 0.75f;
            }
        }
        #endregion

        #region Knifes
        [Header("Knifes")]
        [SerializeField] private float _knifesCooldown = 30f;
        private float _knifesCooldownTimer = 0f;

        [SerializeField] private float _knifesDuration = 5f;
        [SerializeField] private GameObject _knifePrefab;
        [SerializeField] private int _knifeCount = 5;

        private void Knifes() {
            _knifesCooldownTimer += Time.deltaTime;
            if (_knifesCooldownTimer >= _knifesCooldown) {
                _knifesCooldownTimer = 0f;
                if (Random.Range(0, 2) == 0) StartCoroutine(KnifesCoroutine());
                else StartCoroutine(KnifesDelayCoroutine());
            }

            IEnumerator KnifesCoroutine() {
                var dirPlayer = _targetPlayer.transform.position - transform.position;
                dirPlayer.Normalize();
                var pos = transform.position + -dirPlayer * 10f;

                // Spawn knifes
                for (int i = 0; i < _knifeCount; i++)
                {                    
                    var knife = Instantiate(_knifePrefab, GetRandomPosition(), Quaternion.identity);
                    knife.GetComponent<Knife>().Damage = _damage;
                    knife.GetComponent<Knife>()._target = _targetPlayer.transform;

                    Destroy(knife, _knifesDuration);
                }

                Vector2 GetRandomPosition()
                {
                    Vector2 randomPoint = Random.insideUnitCircle.normalized * 20;
                    Vector2 offset = new Vector2(randomPoint.x, randomPoint.y);
                    return _targetPlayer.transform.position + (Vector3)offset;
                }

                yield return null;
            }

            IEnumerator KnifesDelayCoroutine() {
                var dirPlayer = _targetPlayer.transform.position - transform.position;
                dirPlayer.Normalize();
                var pos = transform.position + -dirPlayer * 10f;

                // Spawn knifes
                for (int i = 0; i < _knifeCount; i++)
                {                    
                    var knife = Instantiate(_knifePrefab, GetRandomPosition(), Quaternion.identity);
                    knife.GetComponent<Knife>().Damage = _damage;
                    knife.GetComponent<Knife>()._target = _targetPlayer.transform;

                    Destroy(knife, _knifesDuration);
                    yield return Helpers.GetWait(0.2f);
                }

                Vector2 GetRandomPosition()
                {
                    Vector2 randomPoint = Random.insideUnitCircle.normalized * 20;
                    Vector2 offset = new Vector2(randomPoint.x, randomPoint.y);
                    return _targetPlayer.transform.position + (Vector3)offset;
                }

                yield return null;
            }
        }
        #endregion

        #region Animations
        private Animator _animator;
        private int _currentAnimation = 0;
        private float _lockedTill;

        private static readonly int IdleInt = Animator.StringToHash("Idle");
        private static readonly int WalkInt = Animator.StringToHash("Walk");

        
        private void HandleAnimations() {
            var state = GetState();
            ResetFlags();
            if (state == _currentAnimation) return;

            _animator.Play(state, 0); //_anim.CrossFade(state, 0, 0);
            _currentAnimation = state;

            int GetState()
            {
                if (Time.time < _lockedTill) return _currentAnimation;

                if (_rb.velocity != Vector2.zero) return WalkInt;

                // NO SKILL PRESSED
                return IdleInt;

                // State and time to lock
                int LockState(int s, float t)
                {
                    _lockedTill = Time.time + t;
                    return s;
                }

            }

            void ResetFlags() {
                
            }
        }
        #endregion
    }
}
