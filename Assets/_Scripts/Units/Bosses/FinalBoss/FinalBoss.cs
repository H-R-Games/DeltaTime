using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_enemy {
    public class FinalBoss : EnemyBase
    {
        public override void Start() {
            base.Start();
            _lastHP = _health;
        }

        public override void Update() {
            base.Update();

            InDodge();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            FinalBossAI();
        }


        private void FinalBossAI() {
            Dodge();
            //GroingStuff();
            PlayerTimeTravel();
        }

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
                yield return new WaitForSeconds(_playerTimeTravelDuration * 0.5f);

                _targetPlayer.transform.position = playerPos;
            }
        }
        #endregion
    }
}
