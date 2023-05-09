using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
            GetFashionEars();
        }
        
        public void OnPickUp() {
            // Game Items
            GetDopamine();
            GetMarcaShoes();
            GetRock();
            GetWingedShoes();
            GetSpringShoes();
            OnGetCachinkGlasses();
        }

        public void OnRemove(float damage, EnemyBase enemy) {

        }

        public void OnHit(float damage, EnemyBase enemy) {
            var procCo = _player.LastSkillProcCoefficient;
            List<ItemBase> items = new List<ItemBase>();
            print("On Hit");
            ProccItems(items, damage, enemy, procCo);

            // Game Items 100% proc
            DealMonsterWheights(damage, enemy);
            GetLaParca(enemy);
        }

        public void OnKill(float damage, EnemyBase enemy) {
            print("Heal On Kill");
            HealOnKill();
        }  
        
        public void OnDeath() {

        }

        private bool _hasRecievedDamage = false;
        public void OnDamageTaken(float damage) {
            _hasRecievedDamage = true;
            print("On Damage Taken");
        }

        private void ProccItems(List<ItemBase> items, float damage, EnemyBase enemy, float procCo = 1f) {
            print("Procc Items");
            ChanceToDealExtraHit(items, damage, enemy, procCo);
            GetDecisionArrow(items, enemy, procCo);
            GetGeniusComet(items, enemy, procCo);
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
            if (Random.Range(0f, 1f) > _chanceToDealExtraHit * procCo + _player.Luck) return;
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
        private int _lastMarcaShoesAmmount = 0;

        private void GetMarcaShoes() {
            if (MarcaShoesAmount == 0) return;
            if (_lastMarcaShoesAmmount == MarcaShoesAmount) return;
            if (_marcaShoesItem == null) _marcaShoesItem = new MarcaShoes();
            _lastMarcaShoesAmmount = MarcaShoesAmount;
            _marcaShoes = _marcaShoesItem.MovementSpeedIncrease * MarcaShoesAmount;

            _player.AddMovementSpeedPercentage(_marcaShoes);
        }
        #endregion
        #region Rock
        [Header("Rock")]
        public int RockAmount = 0; // Amount of items
        private float _rock = 0f; // Rock (Stack with other items)
        private Rock _rockItem;
        private int _lastRockAmmount = 0;

        private void GetRock() {
            if (RockAmount == 0) return;
            if (_lastRockAmmount == RockAmount) return;
            if (_rockItem == null) _rockItem = new Rock();
            _lastRockAmmount = RockAmount;
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
            if (_player.InCombat) return;

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
        private int _lastSpringShoesAmmount = 0;

        private void GetSpringShoes() {
            if (SpringShoesAmount == 0) return;
            if (_lastSpringShoesAmmount == SpringShoesAmount) return;
            if (_springShoesItem == null) _springShoesItem = new SpringShoes();
            _lastSpringShoesAmmount = SpringShoesAmount;
            _springShoes = _player.JumpForce + _springShoesItem.JumpHeightIncrease * SpringShoesAmount;

            _player.SetJumpForce(_springShoes);
        }
        #endregion
        #region Immovable sword
        [Header("Immovable sword")]
        public int ImmovableSwordAmount = 0; // Amount of items
        private ImmovableSword _immovableSwordItem;
        float _timerToAplicImmovableSword = 0;
        float _timerToEndImmovableSword = 0;
        bool _immovableSwordInEffect = false;

        private void GetImmovableSword() {
            if (ImmovableSwordAmount == 0) return;
            if (_immovableSwordItem == null) _immovableSwordItem = new ImmovableSword();
            if (ImmovableSwordAmount >= 5) ImmovableSwordAmount = 5;

            // TODO: Detect if player velocity is 0

            if (_player.GetComponent<Rigidbody2D>().velocity == Vector2.zero) {
                _timerToAplicImmovableSword += Time.deltaTime;

                if (_timerToAplicImmovableSword >= _immovableSwordItem.TimeToActive) {
                    if (!_immovableSwordInEffect) {
                        _immovableSwordInEffect = true;
                        _player.AddPercentageDamageBonus(_immovableSwordItem.DamagePorcen * ImmovableSwordAmount);
                        _timerToEndImmovableSword = 0;
                    }
                }
            } else {
                _timerToAplicImmovableSword = 0;
            }

            if (_immovableSwordInEffect) {
                _timerToEndImmovableSword += Time.deltaTime;
                if (_timerToEndImmovableSword >= _immovableSwordItem.TimeToDesactive) {
                    _immovableSwordInEffect = false;
                    _player.RemovePercentageDamageBonus(_immovableSwordItem.DamagePorcen * ImmovableSwordAmount);
                    _timerToAplicImmovableSword = 0;
                }
            }
        }
        #endregion
        #region Winged Shoes
        [Header("Winged Shoes")]
        public int WingedShoesAmount = 0; // Amount of items
        private WingedShoes _wingedShoesItem;
        private int _lastWingedShoesAmmount = 0;

        private void GetWingedShoes() {
            if (WingedShoesAmount == 0) return;
            if (_lastWingedShoesAmmount == WingedShoesAmount) return;
            if (_wingedShoesItem == null) _wingedShoesItem = new WingedShoes();
            _lastWingedShoesAmmount = WingedShoesAmount;

            // TODO: We add one more jump to the player depending on the number of items

            _player.AddAirJump();
        }
        #endregion
        #region Fashion Ears
        [Header("Fashion Ears")]
        public int FashionEarsAmount = 0; // Amount of items
        private FashionEars _FashionEarsItem;
        bool _FashionEarsInEffect = false;
        
        private void GetFashionEars() {
            if (FashionEarsAmount == 0) return;
            if (_FashionEarsItem == null) _FashionEarsItem = new FashionEars();

            // TODO: When the player is hit, he runs faster for 3 seconds

            if (_hasRecievedDamage) {
                if (!_FashionEarsInEffect) {
                    _FashionEarsInEffect = true;
                    StartCoroutine(RunFast());
                    _hasRecievedDamage = false;
                }
            }

            IEnumerator RunFast() {
                var f = _FashionEarsItem.SpeedIncrease * FashionEarsAmount;
                var t = _FashionEarsItem.TimeToDesactive;
                _player.AddMovementSpeedPercentage(f);
                yield return new WaitForSeconds(t);
                _player.RemoveMovementSpeedPercentage(f);
                _FashionEarsInEffect = false;
            }

        }
        #endregion
        #region Monster Wheights
        [Header("Monster Wheights")]
        public int MonsterWheightsAmount = 0; // Amount of items
        private MonsterWheights _monsterWheightsItem;
        private float _movementRedux = 0f;

        private void DealMonsterWheights(float damage, EnemyBase enemy) {
            if (MonsterWheightsAmount == 0) return;
            if (_monsterWheightsItem == null) _monsterWheightsItem = new MonsterWheights();

            _movementRedux = _monsterWheightsItem.SpeedRedux * MonsterWheightsAmount;

            var removeSpeed = enemy.GetMoveSpeed() * _movementRedux;
            enemy.SetMoveSpeed(enemy.GetMoveSpeed() - removeSpeed);
        }
        #endregion
        #region La Parca
        [Header("La Parca")]
        public int LaParcaAmount = 0; // Amount of items
        private LaParca _laParcaItem;

        private void GetLaParca(EnemyBase enemy) {
            if (LaParcaAmount == 0) return;
            if (_laParcaItem == null) _laParcaItem = new LaParca();

            // TODO: If the enemy has less than 15% life, kill him instantly
            var f = _laParcaItem.EnemyMaxLife * LaParcaAmount;
            f = Mathf.Clamp(f, 0, _laParcaItem.MaxPorcen);

            if (enemy.GetHealthPercentage() <= f) {
                enemy.DestroyEnemy();
            }
        }
        #endregion
        #region Book of Knowledge
        [Header("Book of Knowledge")]
        public int BookOfKnowledgeAmount = 0; // Amount of items
        private BookOfKnowledge _bookOfKnowledgeItem;
        private int _lastBookOfKnowledgeAmmount = 0;

        private void GetBookOfKnowledge() {
            if (BookOfKnowledgeAmount == 0) return;
            if (_lastBookOfKnowledgeAmmount == BookOfKnowledgeAmount) return;
            if (_bookOfKnowledgeItem == null) _bookOfKnowledgeItem = new BookOfKnowledge();
            _lastBookOfKnowledgeAmmount = BookOfKnowledgeAmount;

            _player.AddExperienceMultiplier(_bookOfKnowledgeItem.ExpMultiplier * BookOfKnowledgeAmount);
        }
        #endregion
        #region Lucky Glasses
        [Header("Lucky Glasses")]
        public int LuckyGlassesAmount = 0; // Amount of items
        private LuckyGlasses _luckyGlassesItem;
        private int _lastLuckyGlassesAmmount = 0;

        private void GetLuckyGlasses() {
            if (LuckyGlassesAmount == 0) return;
            if (_lastLuckyGlassesAmmount == LuckyGlassesAmount) return;
            if (_luckyGlassesItem == null) _luckyGlassesItem = new LuckyGlasses();
            _lastLuckyGlassesAmmount = LuckyGlassesAmount;

            _player.Luck = _luckyGlassesItem.Luck * LuckyGlassesAmount;
        }
        #endregion
        #region Teeth Of The Fearful
        [Header("Teeth Of The Fearful")]
        public int TeethOfTheFearfulAmount = 0; // Amount of items
        private TeethOfTheFearful _teethOfTheFearfulItem;

        private void GetTeethOfTheFearful(float damage) {
            if (TeethOfTheFearfulAmount == 0) return;
            if (_teethOfTheFearfulItem == null) _teethOfTheFearfulItem = new TeethOfTheFearful();

            _player.HealAmmount(damage * (TeethOfTheFearfulAmount * _teethOfTheFearfulItem.HealthSteal));
        }
        #endregion
        #region Decision Arrow
        [Header("Decision Arrow")]
        public int DecisionArrowAmount = 0; // Amount of items
        private DecisionArrow _decisionArrowItem;
        Arrow ArrowPrefab;

        private void GetDecisionArrow(List<ItemBase> items, EnemyBase enemy, float procCo = 1f) {
            if (DecisionArrowAmount == 0) return;

            // If the items list is not empty, then check if it contains the class
            if (items.Count > 0) {
                // If the items list contains the class, then we don't want to do anything
                for (int i = 0; i < items.Count; i++) {
                    if (items[i].GetType() == typeof(DecisionArrow)) return; 
                }
            }

            if (_decisionArrowItem == null) _decisionArrowItem = new DecisionArrow();
            if (ArrowPrefab == null) ArrowPrefab = Resources.Load<Arrow>("Arrow");

            // TODO: When we are attacking an enemy there is a probability that we will shoot an arrow

            // Create a new instance of the class
            DecisionArrow DecisionArrowItem = new DecisionArrow();
            
            if (Random.Range(0, 1f) > _decisionArrowItem.Probability * procCo + _player.Luck) return;

            for (int i = 0; i < DecisionArrowAmount; i++) {
                var f = Random.Range(-2, 2);
                Arrow arrow = Instantiate(ArrowPrefab, enemy.transform.position + new Vector3(Random.Range(0f, 1f) > 0.5f ? 20 + f : -20 + f, 20, 0), Quaternion.identity);
                arrow.targetPosition = enemy.transform.position;
                arrow.direction = (enemy.transform.position - arrow.transform.position).normalized;
            }

            // Add the class to the items list
            items.Add(DecisionArrowItem);
            // Call the ProccItems method
            ProccItems(items, 10, enemy);
        }
        #endregion
        #region Berserk Beer
        [Header("Berserk Beer")]
        public int BerserkBeerAmount = 0; // Amount of items
        private BerserkBeer _berserkBeerItem;

        private bool _inBerserkBeer = false;
        private void InBerserkBeer() {
            if (BerserkBeerAmount == 0) return;
            if (_berserkBeerItem == null) _berserkBeerItem = new BerserkBeer();

            if (_player.CurrentHealth <= _player.MaxStats.Health * 0.3f) {
                if (!_inBerserkBeer) {
                    _inBerserkBeer = true;
                    _player.AddPercentageDamageBonus(_berserkBeerItem.DamageBoost * BerserkBeerAmount);
                }
            } else {
                if (_inBerserkBeer) {
                    _inBerserkBeer = false;
                    _player.AddPercentageDamageBonus(-_berserkBeerItem.DamageBoost * BerserkBeerAmount);
                }
            }
        }
        #endregion
        #region GeniusComet
        [Header("Genius Comet")]
        public int GeniusCometAmount = 0; // Amount of items
        private GeniusComet _geniusCometItem;
        Comet CometPrefab;

        private void GetGeniusComet(List<ItemBase> items, EnemyBase enemy, float procCo = 1f) {
            if (GeniusCometAmount == 0) return;

            // If the items list is not empty, then check if it contains the class
            if (items.Count > 0) {
                // If the items list contains the class, then we don't want to do anything
                for (int i = 0; i < items.Count; i++) {
                    if (items[i].GetType() == typeof(GeniusComet)) return; 
                }
            }

            if (_geniusCometItem == null) _geniusCometItem = new GeniusComet();
            if (CometPrefab == null) CometPrefab = Resources.Load<Comet>("Comet");

            // TODO: When we are attacking an enemy there is a probability that we will shoot an comet

            // Create a new instance of the class
            GeniusComet GeniusCometItem = new GeniusComet();

            if (Random.Range(0, 1f) > _geniusCometItem.Probability * procCo + _player.Luck) return;

            var damage = GeniusCometItem.Damage * GeniusCometAmount;

            Comet comet = Instantiate(CometPrefab, enemy.transform.position, Quaternion.identity);
            comet.targetPosition = enemy.transform.position;
            comet.direction = (enemy.transform.position - comet.transform.position).normalized;
            comet.damage = damage;

            // Add the class to the items list
            items.Add(GeniusCometItem);
            // Call the ProccItems method
            ProccItems(items, damage, enemy);
        }
        #endregion
        #region Cachink Glasses
        [Header("Cachink Glasses")]
        public int CachinkGlassesAmount = 0; // Amount of items
        private CachinkGlasses _cachinkGlassesItem;
        private int _lastCachinkGlassesAmmount = 0;

        private void OnGetCachinkGlasses() {
            if (CachinkGlassesAmount == 0) return;
            if (_cachinkGlassesItem == null) _cachinkGlassesItem = new CachinkGlasses();
            if (_lastCachinkGlassesAmmount == CachinkGlassesAmount) return;
            _lastCachinkGlassesAmmount = CachinkGlassesAmount;

            _player.MoneyMultiplier = 1 + _cachinkGlassesItem.MoneyMultiplier * CachinkGlassesAmount;
        }
        #endregion
    }
}
