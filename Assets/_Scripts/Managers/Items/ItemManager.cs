using UnityEngine;
using rene_roid_enemy;

namespace rene_roid_player {    
    public class ItemManager : MonoBehaviour
    {
        #region Internal
        private PlayerBase _player;
        #endregion

        void Start()
        {
            _player = GetComponent<PlayerBase>();
        }
        
        void Update()
        {
            MoveFastWhenLowHealth();
        }
        
        public void OnPickUp() {

        }

        public void OnRemove(float damage, EnemyBase enemy) {

        }

        public void OnHit(float damage, EnemyBase enemy) {

        }

        public void OnKill(float damage, EnemyBase enemy) {
            print("Heal On Kill");
            HealOnKill();
        }  
        
        public void OnDeath() {

        }

        #region Heal On Kill
        [Header("Heal On Kill")]
        public int HealOnKillAmount = 0;
        [SerializeField] private float _healOnKill = 0f;
        private float c_healOnKill = 0f;

        private void HealOnKill() {
            if (HealOnKillAmount == 0) return;
            c_healOnKill = _healOnKill * HealOnKillAmount;
            // Add health to player
            _player.HealAmmount(c_healOnKill);
        }
        #endregion

        #region Move fast when low health
        [Header("Move fast when low health")]
        public int MoveFastWhenLowHealthAmount = 0;
        [SerializeField] private float _moveFastWhenLowHealth = 0f;
        private float c_moveFastWhenLowHealth = 0f;
        private bool _inEffect = false;

        private void MoveFastWhenLowHealth() {
            // If the MoveFastWhenLowHealthAmount is 0, then we don't want to do anything
            if (MoveFastWhenLowHealthAmount == 0) return;

            // Calculate the amount of health that the player needs to be at to move faster
            c_moveFastWhenLowHealth = _moveFastWhenLowHealth * MoveFastWhenLowHealthAmount;

            // Check if the player's health is less than or equal to the calculated health
            if (_player.CurrentHealth <= _player.MaxStats.Health * 0.25f) {
                // If the player's health is low and the effect is not in effect, then add the effect
                if (!_inEffect) {
                    _inEffect = true;
                    _player.AddMovementSpeedFlat(c_moveFastWhenLowHealth);
                }
            } else {
                // If the player's health is not low and the effect is in effect, then remove the effect
                if (_inEffect) {
                    _inEffect = false;
                    _player.RemoveMovementSpeedFlat(c_moveFastWhenLowHealth);
                }
            }
        }
        #endregion
    }
}
