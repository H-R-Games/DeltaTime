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

            InParry();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            FinalBossAI();
        }


        private void FinalBossAI() {
            Parry();
        }

        #region Parry
        [Header("Parry")]
        [SerializeField] private float _parryDuration = 5f;
        [SerializeField] private float _parryCooldown = 10f;
        [SerializeField] private float _parryDamage = 1f;

        private float _parryTimer = 0f;
        private float _parryCooldownTimer = 0f;
        private bool _parryActive = false;

        private void Parry() {
            if (_parryActive) {
                _parryTimer += Time.deltaTime;
                if (_parryTimer >= _parryDuration) {
                    _parryActive = false;
                    _parryTimer = 0f;
                }
            }
            else {
                _parryCooldownTimer += Time.deltaTime;
                if (_parryCooldownTimer >= _parryCooldown) {
                    _parryActive = true;
                    _parryCooldownTimer = 0f;
                    _lastHP = _health;
                }
            }
        }

        private float _lastHP = 0f;
        private void InParry() {
            if (_parryActive) {
                if (_lastHP > _health) {
                    // Get behind the player
                    var player = _targetPlayer.transform.position;
                    var dir = player - transform.position;
                    dir.Normalize();
                    transform.position = player + dir * 2f;
                    _lastHP = _health;
                }
            }
        }
        #endregion
    }
}
