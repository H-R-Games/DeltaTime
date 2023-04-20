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

            // Game Items
            LionEmblem();
            Aspirin();
            GetImmovableSword();
        }
        
        public void OnPickUp() {
            // Game Items
            GetDopamine();
            GetMarcaShoes();
            GetRock();
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
    
        /// GAME ITEMS

        #region Lion Emblem
        [Header("Lion Emblem")]
        public int LionEmblemAmount = 0; // Amount of items
        private float _lionEmblem = 0f; // Lion Emblem (Stack with other items)
        private LionEmblem _lionEmblemItem;
        private bool _lionEmblemInEffect = false;

        private void LionEmblem() {
            if (LionEmblemAmount == 0) return;
            if (_lionEmblemItem == null) _lionEmblemItem = new LionEmblem();

            _lionEmblem = _lionEmblemItem.DefenceIncrease * LionEmblemAmount;

            if (_player.CurrentHealth <= _player.MaxStats.Health * 0.30f) {
                if (!_lionEmblemInEffect) {
                    _lionEmblemInEffect = true;
                    _player.AddArmorFlat(_lionEmblem);
                }
            } else {
                if (_lionEmblemInEffect) {
                    _lionEmblemInEffect = false;
                    _player.AddArmorFlat(_lionEmblem);
                }
            }
        }
        #endregion
        #region Dopamine
        [Header("Dopamine")]
        public int DopamineAmount = 0; // Amount of items
        private float _dopamine = 0f; // Dopamine (Stack with other items)
        private Dopamine _dopamineItem;
        private int _lastDopamineAmmount = 0;

        private void GetDopamine() {
            if (DopamineAmount == 0) return;
            if (_lastDopamineAmmount == DopamineAmount) return;
            if (_dopamineItem == null) _dopamineItem = new Dopamine();
            _lastDopamineAmmount = DopamineAmount;

            _dopamine = _dopamineItem.AttackMoveSpeedIncrease * DopamineAmount;

            _player.AddMovementSpeedPercentage(_dopamine);
            _player.AddPercentageDamageBonus(_dopamine);
        }
        #endregion    
        #region Marca Shoes
        [Header("Marca Shoes")]
        public int MarcaShoesAmount = 0; // Amount of items
        private float _marcaShoes = 0f; // Marca Shoes (Stack with other items)
        private MarcaShoes _marcaShoesItem;

        private void GetMarcaShoes() {
            if (MarcaShoesAmount == 0) return;
            if (_marcaShoesItem == null) _marcaShoesItem = new MarcaShoes();

            _marcaShoes = _marcaShoesItem.MovementSpeedIncrease * MarcaShoesAmount;

            _player.AddMovementSpeedPercentage(_marcaShoes);
        }
        #endregion
        #region Rock
        [Header("Rock")]
        public int RockAmount = 0; // Amount of items
        private float _rock = 0f; // Rock (Stack with other items)
        private Rock _rockItem;

        private void GetRock() {
            if (RockAmount == 0) return;
            if (_rockItem == null) _rockItem = new Rock();

            _rock = _rockItem.Armor * RockAmount;

            _player.AddArmorFlat(_rock);
        }
        #endregion
        #region Aspirin
        [Header("Aspirin")]
        public int AspirinAmount = 0; // Amount of items
        private float _aspirin = 0f; // Aspirin (Stack with other items)
        private Aspirin _aspirinItem;

        private void Aspirin() {
            if (AspirinAmount == 0) return;
            if (_aspirinItem == null) _aspirinItem = new Aspirin();

            _aspirin = _aspirinItem.HealthRegen * AspirinAmount;

            // TODO: Detect if player is in combat

            if (_player.CurrentHealth <= _player.MaxStats.Health * 0.30f) {
                _player.AddHealthRegenPercentage(_aspirin);
            } else {
                _player.AddHealthRegenPercentage(_aspirin);
            }
        }
        #endregion
        #region Spring Shoes
        [Header("Spring Shoes")]
        public int SpringShoesAmount = 0; // Amount of items
        private float _springShoes = 0f; // Spring Shoes (Stack with other items)
        private SpringShoes _springShoesItem;

        private void GetSpringShoes() {
            if (SpringShoesAmount == 0) return;
            if (_springShoesItem == null) _springShoesItem = new SpringShoes();

            _springShoes = _player.JumpForce + _springShoesItem.JumpHeightIncrease * SpringShoesAmount;

            // TODO: Set the jump force to the new value
            // _player.JumpForce = _springShoes;
        }
        #endregion
        #region Immovable sword
        [Header("Immovable sword")]
        public int ImmovableSwordAmount = 0; // Amount of items
        private ImmovableSword _immovableSwordItem;
        float timerToAplic = 0;
        float timerToEnd = 0;
        bool _immovableSwordInEffect = false;

        private void GetImmovableSword() {
            if (ImmovableSwordAmount == 0) return;
            if (_immovableSwordItem == null) _immovableSwordItem = new ImmovableSword();
            if (ImmovableSwordAmount >= 5) ImmovableSwordAmount = 5;

            // TODO: Detect if player velocity is 0

            if (_player.GetComponent<Rigidbody2D>().velocity == Vector2.zero) {
                timerToAplic += Time.deltaTime * 0.5f;

                if (timerToAplic >= _immovableSwordItem.TimeToActive) {
                    if (!_immovableSwordInEffect) {
                        _immovableSwordInEffect = true;
                        _player.AddPercentageDamageBonus(_immovableSwordItem.DamagePorcen);
                        timerToEnd = 0;
                    }
                }
            } else {
                timerToAplic = 0;
            }

            if (_immovableSwordInEffect) {
                timerToEnd += Time.deltaTime * 0.5f;
                if (timerToEnd >= _immovableSwordItem.TimeToDesactive) {
                    _immovableSwordInEffect = false;
                    _player.RemovePercentageDamageBonus(_immovableSwordItem.DamagePorcen);
                    timerToAplic = 0;
                }
            }
        }
        #endregion
    }
}
