using UnityEngine;
using System.Collections.Generic;
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
            var procCo = _player.LastSkillProcCoefficient;
            List<ItemBase> items = new List<ItemBase>();
            print("On Hit");
            ProccItems(items, damage, enemy, procCo);

        }

        public void OnKill(float damage, EnemyBase enemy) {
            print("Heal On Kill");
            HealOnKill();
        }  
        
        public void OnDeath() {

        }

        private void ProccItems(List<ItemBase> items, float damage, EnemyBase enemy, float procCo = 1f) {
            print("Procc Items");
            ChanceToDealExtraHit(items, damage, enemy, procCo);
        }

        #region Heal On Kill
        [Header("Heal On Kill")]
        public int HealOnKillAmount = 0; // Amount of items
        private float _healOnKill = 0f; // Heal on kill (Stack with other items)
        private HealOnKill _healOnKillItem;

        private void HealOnKill() {
            // If HealOnKillAmount is 0, return
            if (HealOnKillAmount == 0) return;
            // If _healOnKillItem is null, create a new instance of HealOnKill
            if (_healOnKillItem == null) _healOnKillItem = new HealOnKill();
            // Calculate heal amount from HealOnKillAmount and _healOnKillItem
            _healOnKill = _healOnKillItem.HealAmount * HealOnKillAmount;
            // Add health to player
            _player.HealAmmount(_healOnKill);
        }
        #endregion

        #region Move fast when low health
        [Header("Move fast when low health")]
        public int MoveFastWhenLowHealthAmount = 0; // Amount of items
        private float _moveFastWhenLowHealth = 0f; // Move fast when low health (Stack with other items)
        private MoveFastWhenLowHealth _moveFastWhenLowHealthItem;
        private bool _inEffect = false;

        private void MoveFastWhenLowHealth() {
            // If the MoveFastWhenLowHealthAmount is 0, then we don't want to do anything
            if (MoveFastWhenLowHealthAmount == 0) return;
            if (_moveFastWhenLowHealthItem == null) _moveFastWhenLowHealthItem = new MoveFastWhenLowHealth();

            // Calculate the amount of health that the player needs to be at to move faster
            _moveFastWhenLowHealth = _moveFastWhenLowHealthItem.SpeedBoost * MoveFastWhenLowHealthAmount;

            // Check if the player's health is less than or equal to the calculated health
            if (_player.CurrentHealth <= _player.MaxStats.Health * 0.25f) {
                // If the player's health is low and the effect is not in effect, then add the effect
                if (!_inEffect) {
                    _inEffect = true;
                    _player.AddMovementSpeedFlat(_moveFastWhenLowHealth);
                }
            } else {
                // If the player's health is not low and the effect is in effect, then remove the effect
                if (_inEffect) {
                    _inEffect = false;
                    _player.RemoveMovementSpeedFlat(_moveFastWhenLowHealth);
                }
            }
        }
        #endregion

        #region Chance To Deal Extra Hit
        [Header("Chance To Deal Extra Hit")]
        public int ChanceToDealExtraHitAmount = 0; // Amount of items
        private float _chanceToDealExtraHit = 0f; // Chance to deal extra hit (Stack with other items)
        private ChanceToDealExtraHit _chanceToDealExtraHitItem;

        private void ChanceToDealExtraHit(List<ItemBase> items, float damage, EnemyBase enemy, float procCo = 1f) {
            // If the ChanceToDealExtraHitAmount is 0, then we don't want to do anything
            if (ChanceToDealExtraHitAmount == 0) return; 

            // If the items list is not empty, then check if it contains the class
            if (items.Count > 0) {
                // If the items list contains the class, then we don't want to do anything
                for (int i = 0; i < items.Count; i++) {
                    if (items[i].GetType() == typeof(ChanceToDealExtraHit)) return; 
                }
            }

            // If the _chanceToDealExtraHitItem is null, then create a new instance of the class
            if (_chanceToDealExtraHitItem == null) _chanceToDealExtraHitItem = new ChanceToDealExtraHit(); 

            // Get the chance to deal extra hit
            _chanceToDealExtraHit = _chanceToDealExtraHitItem.Chance * ChanceToDealExtraHitAmount;

            // Print out the chance to deal extra hit
            print("Chance To Deal Extra Hit: " + _chanceToDealExtraHit);
            
            // Chance to deal extra hit
            // If the chance to deal extra hit is less than the random range, then we don't want to do anything
            if (Random.Range(0f, 1f) > _chanceToDealExtraHit * procCo) return;
            print("YAAAAY: " + _chanceToDealExtraHit);

            // Create a new instance of the class
            ChanceToDealExtraHit chanceToDealExtraHitItem = new ChanceToDealExtraHit();
            // Take damage from enemy
            enemy.TakeDamage(damage * chanceToDealExtraHitItem.DamageMultiplier);
            // Add the class to the items list
            items.Add(chanceToDealExtraHitItem);
            // Call the ProccItems method
            ProccItems(items, damage, enemy);
        }
        #endregion
    }
}
