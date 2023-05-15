using rene_roid;
using rene_roid_enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_player
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerBase : MonoBehaviour, IPlayerController
    {
        [SerializeField] protected PlayerBaseStats _baseStats;

        #region Internal Variables
        [Header("Internal Variables")]
        [SerializeField] protected CapsuleCollider2D _col;
        [SerializeField] protected LayerMask _playerLayer; // Layer mask for the player
        [HideInInspector] protected Rigidbody2D _rb;
        protected bool _cachedTriggerSetting; // Used to cache the trigger setting of the collider

        protected PlayerInput _input;
        protected FrameInput _frameInput;
        protected int _fixedFrame;
        private Director _director;

        private float _luck = 0;
        #endregion

        #region External Variables
        // [Header("External Variables")]
        public event Action<bool, float> GroundedChanged; // Event for when the player's grounded state changes
        public event Action<bool> WallGrabChanged;
        public event Action<bool> Jumped;
        public event Action AirJumped;

        public event Action BasicAttack1;
        public event Action SpecialAttack1;
        public event Action SpecialAttack2;
        public event Action UltimateAttack;

        public PlayerBaseStats PlayerStats => _baseStats;
        public FrameInput FrameInput => _input.FrameInput;
        public Vector2 Input => _frameInput.Move;
        public Vector2 Speed => _speed;
        public Vector2 GroundNormal => _groundNormal;
        public int WallDirection => _wallDir;
        public bool ClimbingLadder => _onLadder;


        public float Luck { get; set; }

        public virtual void ApplyVelocity(Vector2 vel, PlayerForce forceType)
        {
            if (forceType == PlayerForce.Burst) _speed += vel;
            else _currentExternalVelocity += vel;
        }

        public virtual void TakeAwayControl(bool resetVelocity = true)
        {
            if (resetVelocity) _currentExternalVelocity = Vector2.zero;
            _hasControl = false;
        }

        public virtual void ReturnControl()
        {
            _speed = Vector2.zero;
            _hasControl = true;
        }
        #endregion

        private void OnEnable() {
            // EnemyBase.OnHit += OnEnemyHit;
            // EnemyBase.OnDeath += OnEnemyDeath;
        }

        public virtual void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _input = GetComponent<PlayerInput>();

            AwakePlayerStats();
            AwakeSkillFrameSetup();
        }

        public virtual void Start()
        {
            _itemManager = GetComponent<ItemManager>();
        }


        public virtual void Update()
        {
            GatherInput();
            UpdatePlayerStats();
        }

        public virtual void FixedUpdate()
        {
            FixedMovement();
        }

        protected virtual void GatherInput()
        {
            _frameInput = _input.FrameInput;

            if (_frameInput.JumpDown)
            {
                _jumpToConsume = true;
                _frameJumpWasPressed = _fixedFrame;
            }

            GatherSkillInput();
        }

        #region Player Stats
        [Header("Player Stats")]
        [SerializeField] protected int _level = 1;
        public int Level => _level;

        [SerializeField] protected float _currentHealth;
        [SerializeField] protected float _currentHealthRegen;
        [SerializeField] protected float _currentDamage;
        [SerializeField] protected float _currentArmor;
        [SerializeField] protected float _currentMovementSpeed;

        protected float _extraHealthPercentage = 0f;
        protected float _extraFlatHealth = 0f;
        protected float _extraHealthRegenPercentage = 0f;
        protected float _extraFlatHealthRegen = 0f;
        protected float _extraArmorPercentage = 0f;
        protected float _extraFlatArmor = 0f;
        protected float _extraMovementSpeedPercentage = 0f;
        protected float _extraFlatMovementSpeed = 0f;

        protected PlayerBaseStats _maxStats;


        protected void AwakePlayerStats()
        {
            _maxStats = Instantiate(_baseStats);

            SetPlayerStats();
        }

        protected void UpdatePlayerStats()
        {
            ConstantHealing();
            FallDamage();
        }

        public void SetPlayerStats()
        {
            _currentHealth = _maxStats.Health;
            _currentHealthRegen = _maxStats.HealthRegen;
            _currentDamage = _maxStats.Damage;
            _currentArmor = _maxStats.Armor;
            _currentMovementSpeed = _maxStats.MovementSpeed;
        }

        public void LevelUp()
        {
            _level++;
            LevelUpPlayerStats();
        }

        protected void LevelUpPlayerStats()
        {
            UpdateMaxPlayerStats();
            SetPlayerStats();
        }

        protected void UpdateMaxPlayerStats()
        {
            _maxStats.Health = (_baseStats.Health + ((_level - 1) * _baseStats.HealthPerLevel)) * (1 + _extraHealthPercentage) + _extraFlatHealth;
            _maxStats.HealthRegen = (_baseStats.HealthRegen + ((_level - 1) * _baseStats.HealthRegenPerLevel)) * (1 + _extraHealthRegenPercentage) + _extraFlatHealthRegen;
            _maxStats.Damage = (_baseStats.Damage + ((_level - 1) * _baseStats.DamagePerLevel));
            _maxStats.Armor = (_baseStats.Armor + ((_level - 1) * _baseStats.ArmorPerLevel)) * (1 + _extraArmorPercentage) + _extraFlatArmor;
            _maxStats.MovementSpeed = (_baseStats.MovementSpeed + ((_level - 1) * _baseStats.MovementSpeedPerLevel)) * (1 + _extraMovementSpeedPercentage) + _extraFlatMovementSpeed;

            UpdateCurrentStats();
        }

        /// <summary>
        /// Updates current stats (cleanse any buffs/debuffs)
        /// </summary>
        protected void UpdateCurrentStats() {
            _currentHealthRegen = _maxStats.HealthRegen;
            _currentDamage = _maxStats.Damage;
            _currentArmor = _maxStats.Armor;
            _currentMovementSpeed = _maxStats.MovementSpeed;
        }

        #region Fall Damage
        [Header("Fall Damage")]
        [SerializeField] protected float _fallDamageMultiplier = 5f;
        private float _maxFallDamagePercentage = 0.95f;
        private float _fallDamage = 0;
        private float _fallTime = 0f;

        protected void FallDamage()
        {
            if (_speed.y < 0 && !_grounded && !_onLadder && !_isOnWall)
            {
                _fallTime += Time.deltaTime;

                if (_fallTime > 1f)
                {
                    _fallDamage = Mathf.Clamp(_fallTime * _fallDamageMultiplier, 0, _maxFallDamagePercentage * _maxStats.Health);
                }
            }
            else
            {
                if (_onLadder) _fallDamage = 0;
                if (_grounded && _fallDamage > 0)
                {
                    print("Fall Damage: " + _fallDamage);
                    _fallDamage = Mathf.Clamp(_fallDamage, 0, 10);
                    _fallDamage = Helpers.FromRangeToRange(_fallDamage, 0, 10, 0, _currentHealth * _maxFallDamagePercentage);
                    TakeDamage(_fallDamage);
                    _fallDamage = 0;
                }

                _fallTime = 0f;
            }
        }
        #endregion


        #region Add Stats

        public void AddHealthPercentage(float percentage)
        {
            _extraHealthPercentage += percentage;
            UpdateMaxPlayerStats();
        }

        public void AddHealthFlat(float flat)
        {
            _extraFlatHealth += flat;
            UpdateMaxPlayerStats();
        }

        public void AddHealthRegenPercentage(float percentage)
        {
            _extraHealthRegenPercentage += percentage;
            UpdateMaxPlayerStats();
        }

        public void AddHealthRegenFlat(float flat)
        {
            _extraFlatHealthRegen += flat;
            UpdateMaxPlayerStats();
        }

        public void AddArmorPercentage(float percentage)
        {
            _extraArmorPercentage += percentage;
            UpdateMaxPlayerStats();
        }

        public void AddArmorFlat(float flat)
        {
            _extraFlatArmor += flat;
            UpdateMaxPlayerStats();
        }

        public void AddMovementSpeedPercentage(float percentage)
        {
            _extraMovementSpeedPercentage += percentage;
            UpdateMaxPlayerStats();
        }

        public void AddMovementSpeedFlat(float flat)
        {
            _extraFlatMovementSpeed += flat;
            UpdateMaxPlayerStats();
        }

        public void AddSpecialMultiplier(float multiplier)
        {
            _specialMultiplier += multiplier;
        }

        public void AddFlatDamageBonus(float flat)
        {
            _flatDmgBonus += flat;
        }

        public void AddPercentageDamageBonus(float percentage)
        {
            _percentageDmgBonus += percentage;
        }
        #endregion

        #region Remove Stats

        public void RemoveHealthPercentage(float percentage)
        {
            _extraHealthPercentage -= percentage;
            UpdateMaxPlayerStats();
        }

        public void RemoveHealthFlat(float flat)
        {
            _extraFlatHealth -= flat;
            UpdateMaxPlayerStats();
        }

        public void RemoveHealthRegenPercentage(float percentage)
        {
            _extraHealthRegenPercentage -= percentage;
            UpdateMaxPlayerStats();
        }

        public void RemoveHealthRegenFlat(float flat)
        {
            _extraFlatHealthRegen -= flat;
            UpdateMaxPlayerStats();
        }

        public void RemoveArmorPercentage(float percentage)
        {
            _extraArmorPercentage -= percentage;
            UpdateMaxPlayerStats();
        }

        public void RemoveArmorFlat(float flat)
        {
            _extraFlatArmor -= flat;
            UpdateMaxPlayerStats();
        }

        public void RemoveMovementSpeedPercentage(float percentage)
        {
            _extraMovementSpeedPercentage -= percentage;
            UpdateMaxPlayerStats();
        }

        public void RemoveMovementSpeedFlat(float flat)
        {
            _extraFlatMovementSpeed -= flat;
            UpdateMaxPlayerStats();
        }

        public void RemoveSpecialMultiplier(float multiplier)
        {
            _specialMultiplier -= multiplier;
        }

        public void RemoveFlatDamageBonus(float flat)
        {
            _flatDmgBonus -= flat;
        }

        public void RemovePercentageDamageBonus(float percentage)
        {
            _percentageDmgBonus -= percentage;
        }
        #endregion

        #region Health
        private bool _inCombat = false;
        public bool InCombat => _inCombat;
        private float _lastDamageTaken = 0f;

        protected void ConstantHealing()
        {
            // If has not taken damage in 5 seconds, _inCombat = false
            if (_inCombat && Time.time - _lastDamageTaken > 5f)
            {
                _inCombat = false;
            }

            // Heal the player every second
            if (_currentHealth >= _maxStats.Health) return;
            HealAmmount(_currentHealthRegen * Time.deltaTime);
        }

        /// <summary>
        /// Heals the player by a ammount
        /// </summary>
        /// <param name="ammount"></param>
        public void HealAmmount(float ammount)
        {
            _currentHealth += ammount;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxStats.Health);
        }

        /// <summary>
        /// Heals the player by a percentage of their max health
        /// </summary>
        /// <param name="percentage">The percentage of max health to heal (0-1)</param>
        public void HealPercentage(float percentage)
        {
            _currentHealth += _maxStats.Health * percentage;
            _currentHealth = Mathf.Clamp(_currentHealth, 0, _maxStats.Health);
        }
        #endregion

        #region Damage
        protected float _specialMultiplier = 1;
        protected float _flatDmgBonus = 0;
        protected float _percentageDmgBonus = 1;

        public void TakeDamage(float damage)
        {
            _inCombat = true;
            _lastDamageTaken = Time.time;

            _itemManager.OnDamageTaken(damage);

            if (_currentArmor > 0)
            {
                damage *= 100 / (100 + _currentArmor);
            }

            _currentHealth -= damage;

            if (_currentHealth <= 0)
            {
                Die();
            }

            void Die() {
                var death = FindObjectOfType<Death>();
                death.OnDeath();
            }
        }

        public float DealDamage(float percentage, float proc)
        {
            float basedmg = percentage * _maxStats.Damage;
            float damage = ((basedmg * _specialMultiplier) + _flatDmgBonus) * _percentageDmgBonus;

            print(damage);

            return damage;
        }

        public void KILLME(float dmg)
        {
            TakeDamage(DealDamage(dmg, 1));
        }
        #endregion

        #region Getters and Setters
        public PlayerBaseStats MaxStats => _maxStats;

        // Get only current stats
        public float CurrentHealth => _currentHealth;
        public float CurrentHealthRegen => _currentHealthRegen;
        public float CurrentArmor => _currentArmor;
        public float CurrentMovementSpeed => _currentMovementSpeed;
        #endregion

        #region Experience
        [Header("Experience")]
        private float _currentExperience = 0;
        public  float CurrentExperience => _currentExperience;
        private float _experienceToNextLevel = 100;
        public  float ExperienceToNextLevel => _experienceToNextLevel;
        private float _experienceMultiplier = 1.37f;
        private float _extraExp = 1;
        public void AddExperienceMultiplier(float multiplier)
        {
            _extraExp = multiplier;
        }

        public void AddExperience(float experience)
        {
            _currentExperience += experience * _extraExp;
            if (_currentExperience >= _experienceToNextLevel)
            {
                LevelUp();

                // Remove the experience that was used to level up
                _currentExperience -= _experienceToNextLevel;

                // Increase the experience needed to level up
                _experienceToNextLevel *= _experienceMultiplier;
            }
        }

        #endregion

        #endregion

        #region Player Skills
        [Header("Player Skills")]
        public float LastSkillProcCoefficient = 1;
        [Header("Cooldowns")]
        public float BasicAttackCooldown = 0.5f;
        public float Skill1Cooldown = 1;
        public float Skill2Cooldown = 2;
        public float UltimateCooldown = 5;

        protected float _basicAttackTimer;
        protected float _skill1Timer;
        protected float _skill2Timer;
        protected float _ultimateTimer;

        protected bool _basicAttackReady = true;
        protected bool _skill1Ready = true;
        protected bool _skill2Ready = true;
        protected bool _ultimateReady = true;

        protected int _basicAttackFrames = 8;
        protected int _skill1Frames = 8;
        protected int _skill2Frames = 8;
        protected int _ultimateFrames = 8;

        protected int _basicFrameWasPressed;
        protected int _skill1FrameWasPressed;
        protected int _skill2FrameWasPressed;
        protected int _ultimateFrameWasPressed;

        // Getts
        public float BasicAttackTimer => _basicAttackTimer;
        public float Skill1Timer => _skill1Timer;
        public float Skill2Timer => _skill2Timer;
        public float UltimateTimer => _ultimateTimer;

        [Header("Time Between Skills")]
        public float _basicAttackTimeLock = 0.5f;
        public float _skill1TimeLock = 0.5f;
        public float _skill2TimeLock = 0.5f;
        public float _ultimateTimeLock = 0.5f;

        protected float _basicAttackTimeLockTimer;
        protected float _skill1TimeLockTimer;
        protected float _skill2TimeLockTimer;
        protected float _ultimateTimeLockTimer;

        protected bool _locked = false;

        protected void AwakeSkillFrameSetup()
        {
            _basicFrameWasPressed = -_basicAttackFrames;
            _skill1FrameWasPressed = -_skill1Frames;
            _skill2FrameWasPressed = -_skill2Frames;
            _ultimateFrameWasPressed = -_ultimateFrames;
        }

        protected void GatherSkillInput() // TODO: Get proc coeficient
        {
            if ((_frameInput.BasicAttackDown || _frameInput.BasicAttackHeld) && _basicAttackReady)
            {
                _basicFrameWasPressed = 0;
                BasicAttack();
            }

            if ((_frameInput.SpecialAttack1Down || _frameInput.SpecialAttack1Held || _skill1FrameWasPressed + _skill1Frames > _fixedFrame) && _skill1Ready)
            {
                _skill1FrameWasPressed = 0;
                Skill1();
            }

            if ((_frameInput.SpecialAttack2Down || _frameInput.SpecialAttack2Held || _skill2FrameWasPressed + _skill2Frames > _fixedFrame) && _skill2Ready)
            {
                _skill2FrameWasPressed = 0;
                Skill2();
            }

            if ((_frameInput.UltimateDown || _frameInput.UltimateHeld || _ultimateFrameWasPressed + _ultimateFrames > _fixedFrame) && _ultimateReady)
            {
                _ultimateFrameWasPressed = 0;
                Ultimate();
            }

            SkillCooldowns();
        }

        public void AddSkillsCooldown(float time) {
            //_basicAttackTimer -= time;
            _skill1Timer = Skill1Cooldown - time;
            _skill2Timer = Skill2Cooldown - time;
            _ultimateTimer = UltimateCooldown - time;

            //_basicAttackReady = false;
            _skill1Ready = false;
            _skill2Ready = false;
            _ultimateReady = false;
        }

        protected void SkillCooldowns()
        {
            if (_locked)
            {
                // If any skill lock is active, count until timer has reached 0

            }

            // If _basic attack not ready count until timer is over cooldown
            if (!_basicAttackReady)
            {
                _basicAttackTimer += Time.deltaTime;
                if (_basicAttackTimer >= BasicAttackCooldown)
                {
                    _basicAttackReady = true;
                    _basicAttackTimer = 0;
                }

                if (_frameInput.BasicAttackDown || _frameInput.BasicAttackHeld)
                {
                    _basicFrameWasPressed = _fixedFrame;
                }
            }

            // If _skill1 not ready count until timer is over cooldown
            if (!_skill1Ready)
            {
                _skill1Timer += Time.deltaTime;
                if (_skill1Timer >= Skill1Cooldown)
                {
                    _skill1Ready = true;
                    _skill1Timer = 0;
                }

                if (_frameInput.SpecialAttack1Down || _frameInput.SpecialAttack1Held)
                {
                    _skill1FrameWasPressed = _fixedFrame;
                }
            }

            // If _skill2 not ready count until timer is over cooldown
            if (!_skill2Ready)
            {
                _skill2Timer += Time.deltaTime;
                if (_skill2Timer >= Skill2Cooldown)
                {
                    _skill2Ready = true;
                    _skill2Timer = 0;
                }

                if (_frameInput.SpecialAttack2Down || _frameInput.SpecialAttack2Held)
                {
                    _skill2FrameWasPressed = _fixedFrame;
                }
            }

            // If _ultimate attack not ready count until timer is over cooldown
            if (!_ultimateReady)
            {
                _ultimateTimer += Time.deltaTime;
                if (_ultimateTimer >= UltimateCooldown)
                {
                    _ultimateReady = true;
                    _ultimateTimer = 0;
                }

                if (_frameInput.UltimateDown || _frameInput.UltimateHeld)
                {
                    _ultimateFrameWasPressed = _fixedFrame;
                }
            }
        }

        public virtual void BasicAttack()
        {
            print("Basic attack!");
            _basicAttackReady = false;
            BasicAttack1.Invoke();
        }

        public virtual void Skill1()
        {
            print("Skill 1!");
            _skill1Ready = false;
            SpecialAttack1.Invoke();
        }

        public virtual void Skill2()
        {
            print("Skill 2!");
            _skill2Ready = false;
            SpecialAttack2.Invoke();
        }

        public virtual void Ultimate()
        {
            print("ULTIMATE!");
            _ultimateReady = false;
            UltimateAttack.Invoke();
        }
        #endregion

        #region Item Management
        [Header("Item Management")]
        public float Money = 0;
        public ItemManager _itemManager;
        public List<Item> Items = new List<Item>();
        [SerializeField] private GameObject _hitPrefab;

        public void AddMoney(float amount) {
            // Round ammount to integer
            amount = Mathf.Round(amount);
            Money += amount;
        }

        public void RemoveMoney(float amount) => Money -= amount;

        public void AddItem(Item item)
        {
            Items.Add(item);
            item.Items.ForEach(i => i.OnGet(this, _itemManager));
            _itemManager.OnPickUp();
        }

        // private void UpdateItems() => _items.ForEach(i => i.Items.ForEach(i => i.OnUpdate(this)));

        public void RemoveItem(Item item)
        {
            Items.Remove(item);
            item.Items.ForEach(i => i.OnRemove(this, _itemManager));
        }

        public virtual void OnEnemyHit(float damage, EnemyBase enemy) 
        {
            var hit = Instantiate(_hitPrefab, enemy.transform.position, Quaternion.identity);
            Destroy(hit, 1f);
            print("Hit enemy for " + damage + " damage!");
            _itemManager.OnHit(damage, enemy);
        }

        public float MoneyMultiplier = 1;
        public virtual void OnEnemyDeath(float damage, EnemyBase enemy)
        {
            print("Killed enemy  " + enemy.name + " for " + damage + " damage!");
            _itemManager.OnKill(damage, enemy);

            // ? Chance to get experience
            AddMoney(enemy.EnemyBaseStats.MoneyReward * MoneyMultiplier);

            // * Add experience
            AddExperience(enemy.EnemyBaseStats.ExperienceReward + (enemy.EnemyBaseStats.ExperienceRewardPerLevel * enemy.Level));

            if (_director == null) {
                _director = FindObjectOfType<Director>();

                if (_director == null) {
                    Debug.LogError("No director found in scene!");
                    return;
                } else {
                    _director.AddExp(enemy.EnemyBaseStats.ExperienceReward * 0.5f);
                }
            } else {
                _director.AddExp(enemy.EnemyBaseStats.ExperienceReward * 0.5f);
            }
        }
        #endregion

        #region Movement
        [Header("Movement")]
        protected Vector2 _speed;
        protected Vector2 _currentExternalVelocity;

        protected bool _hasControl = true;

        protected void FixedMovement()
        {
            _fixedFrame++;
            if (_fixedFrame > int.MaxValue - 100) _fixedFrame = 0;

            CheckCollisions();
            HandleCollisions();
            HandleWalls();
            HandleLadders();

            HandleJump();

            HorizontalMovement();
            HandleVertical();
            ApplyVelocity();
        }

        #region Collisions
        protected readonly RaycastHit2D[] _groundHits = new RaycastHit2D[2];
        protected readonly RaycastHit2D[] _ceilingHits = new RaycastHit2D[2];
        protected readonly Collider2D[] _wallHits = new Collider2D[5];
        protected readonly Collider2D[] _ladderHits = new Collider2D[1];
        protected Vector2 _groundNormal;
        protected int _groundHitCount;
        protected int _ceilingHitCount;
        protected int _wallHitCount;
        protected int _ladderHitCount;
        protected int _frameLeftGrounded = int.MinValue;
        protected bool _grounded;

        protected virtual void CheckCollisions()
        {
            Physics2D.queriesHitTriggers = false;

            // Ground and Ceiling
            _groundHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.down, _groundHits, _grounderDistance, (~_playerLayer & ~_oneWayFloorNoCol & ~_enemyLayer));
            _ceilingHitCount = Physics2D.CapsuleCastNonAlloc(_col.bounds.center, _col.size, _col.direction, 0, Vector2.up, _ceilingHits, _grounderDistance, (~_playerLayer & ~_oneWayFloor & ~_oneWayFloorNoCol & ~_enemyLayer));

            // Walls and Ladders
            var bounds = GetWallDetectionBounds();
            _wallHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _wallHits, _wallLayerMask);

            Physics2D.queriesHitTriggers = true; // Ladders are set to Trigger
            _ladderHitCount = Physics2D.OverlapBoxNonAlloc(bounds.center, bounds.size, 0, _ladderHits, _ladderLayerMask);
            Physics2D.queriesHitTriggers = _cachedTriggerSetting;


            if (!_hasControl) return;
            FallThroughFloor();
            JumpThroughFloor();
        }

        protected virtual bool TryGetGroundNormal(out Vector2 groundNormal)
        {
            Physics2D.queriesHitTriggers = false;
            var hit = Physics2D.Raycast(_rb.position, Vector2.down, _grounderDistance * 2 + 0.5f, ~_playerLayer);
            Physics2D.queriesHitTriggers = _cachedTriggerSetting;
            groundNormal = hit.normal;
            return hit.collider != null;
        }

        protected Bounds GetWallDetectionBounds()
        {
            var colliderOrigin = _rb.position + _col.offset;
            return new Bounds(colliderOrigin, _wallDetectorSize);
        }

        protected virtual void HandleCollisions()
        {
            // Hit a Ceiling
            if (_ceilingHitCount > 0) _speed.y = Mathf.Min(0, _speed.y);

            // Landed on the ground
            if (!_grounded && _groundHitCount > 0)
            {
                _grounded = true;
                ResetJump();
                GroundedChanged?.Invoke(true, Mathf.Abs(_speed.y));
            }

            // Left the ground
            else if (_grounded && _groundHitCount == 0)
            {
                _grounded = false;
                _frameLeftGrounded = _fixedFrame;
                GroundedChanged?.Invoke(false, 0);
            }
        }


        private Coroutine _c;
        // TODO: Make this more efficient
        protected virtual void FallThroughFloor()
        {
            if (_frameInput.Move.y <= -0.65f)
            {
                // Detect if player is on top of _oneWayFloor layer
                var hit = Physics2D.Raycast(_col.bounds.center, Vector2.down, /*_grounderDistance * 2*/ 2, _oneWayFloor);
                if (hit.collider != null)
                {
                    // From layermask to int
                    var ignoreLayer = LayerMask.NameToLayer("OneWayFloorNoCol");

                    // Change layer of hit
                    hit.collider.gameObject.layer = ignoreLayer;

                    // If player is on top of _oneWayFloor layer then ignore floor collision and fall through
                    // Activate the collision again after a short delay
                    _c = StartCoroutine(ChangeLayerAfterDelay(hit.collider, 0.1f));
                }
            }
        }

        protected virtual void JumpThroughFloor()
        {
            if (_speed.y <= 0) return;
            // Detect if player is on top of _oneWayFloor layer
            var hit = Physics2D.Raycast(_col.bounds.center, Vector2.up, /*_grounderDistance * 2*/ 2, _oneWayFloor);
            if (hit.collider != null)
            {
                // From layermask to int
                var ignoreLayer = LayerMask.NameToLayer("OneWayFloorNoCol");

                // Change layer of hit
                hit.collider.gameObject.layer = ignoreLayer;

                // If player is on top of _oneWayFloor layer then ignore floor collision and fall through
                // Activate the collision again after a short delay
                _c = StartCoroutine(ChangeLayerAfterDelay(hit.collider, 0.1f));
            }
        }

        // TODO: Fix bug where player can get stuck on a one way platform
        protected IEnumerator ChangeLayerAfterDelay(Collider2D collider, float delay)
        {
            yield return Helpers.GetWait(delay);
            var bound = new Bounds(_rb.position, _col.size); // Player bounds
            var hit = Physics2D.OverlapBox(bound.center, bound.size, 0, _oneWayFloor); // Check if player is on top of _oneWayFloor layer
            var hit2 = Physics2D.OverlapBox(bound.center, bound.size, 0, _oneWayFloorNoCol); // Check if player is on top of _oneWayFloorNoCol layer

            if (hit != null || hit2 != null)
            {
                StartCoroutine(ChangeLayerAfterDelay(collider, 0.05f));
            }
            else
            {
                // From string to layermask int
                var layer = LayerMask.NameToLayer("OneWayFloor");
                // Change layer of hit
                collider.gameObject.layer = layer;
            }

            
        }
        #endregion

        #region Walls
        protected float _currentWallJumpMoveMultiplier = 1f; // aka "Horizontal input influence"
        protected int _wallDir;
        protected bool _isOnWall;
        protected bool _isLeavingWall; // prevents immediate re-sticking to wall

        protected virtual void HandleWalls()
        {
            if (!_allowWalls) return;

            _currentWallJumpMoveMultiplier = Mathf.MoveTowards(_currentWallJumpMoveMultiplier, 1f, 1f / _wallJumpInputLossFrames);

            _wallDir = _wallHitCount > 0 ? (int)Mathf.Sign(_wallHits[0].transform.position.x - transform.position.x) : 0;

            if (!_isOnWall && ShouldStickWall()) SetOnWall(true);
            else if (_isOnWall && !ShouldStickWall()) SetOnWall(false);

            bool ShouldStickWall()
            {
                if (_wallDir == 0 || _grounded) return false;
                return _requireInputPush ? Mathf.Sign(_frameInput.Move.x > 0 ? 1 : -1) == _wallDir : true;
            }
        }

        protected void SetOnWall(bool on)
        {
            _isOnWall = on;
            if (on) _speed = Vector2.zero;
            else
            {
                _isLeavingWall = false;
                ResetAirJumps();
            }
            WallGrabChanged?.Invoke(on);
        }

        #endregion

        #region Ladders
        protected Vector2 _ladderSnapVel; // TODO: determine if we need to reset this when leaving a ladder, or use a different kind of Lerp/MoveTowards
        protected int _frameLeftLadder = int.MinValue;
        protected bool _onLadder;

        protected bool CanEnterLadder => _ladderHitCount > 0 && _fixedFrame > _frameLeftLadder + _ladderCooldownFrames;
        protected bool MountLadderInputReached => _frameInput.Move.y > _verticalDeadzoneThreshold || (!_grounded && _frameInput.Move.y < -_verticalDeadzoneThreshold);
        protected bool DismountLadderInputReached => _grounded && _frameInput.Move.y < -_verticalDeadzoneThreshold;

        protected virtual void HandleLadders()
        {
            if (!_allowLadders) return;

            if (!_onLadder && CanEnterLadder && MountLadderInputReached) ToggleClimbingLadder(true);
            else if (_onLadder && (_ladderHitCount == 0 || DismountLadderInputReached)) ToggleClimbingLadder(false);

            // Snap to center of ladder
            if (_onLadder && _frameInput.Move.x == 0 && _snapToLadders && _hasControl)
            {
                var pos = _rb.position;
                _rb.position = Vector2.SmoothDamp(pos, new Vector2(_ladderHits[0].transform.position.x, pos.y), ref _ladderSnapVel, _ladderSnapSpeed);
            }
        }

        protected void ToggleClimbingLadder(bool on)
        {
            if (_onLadder == on) return;
            if (on) _speed = Vector2.zero;
            else if (_ladderHitCount > 0) _frameLeftLadder = _fixedFrame;
            _onLadder = on;
            ResetAirJumps();
        }
        #endregion

        #region Jumping 
        protected bool _jumpToConsume;
        protected bool _endedJumpEarly;
        protected bool _coyoteUsable;
        protected bool _bufferedJumpUsable;
        protected int _frameJumpWasPressed = int.MinValue;
        protected int _airJumpsRemaining;

        protected bool CanUseCoyote => _coyoteUsable && !_grounded && _fixedFrame < _frameLeftGrounded + _coyoteFrames;
        protected bool HasBufferedJump => _bufferedJumpUsable && _fixedFrame < _frameJumpWasPressed + _jumpBufferFrames;
        protected bool CanAirJump => _airJumpsRemaining > 0;

        protected virtual void HandleJump()
        {
            if (_jumpToConsume || HasBufferedJump)
            {
                if (_isOnWall && !_isLeavingWall) WallJump();
                else if (_grounded || _onLadder || CanUseCoyote) NormalJump();
                else if (_jumpToConsume && CanAirJump) AirJump();
            }

            _jumpToConsume = false; // Allways consume the flag

            if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;
        }

        protected virtual void NormalJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _coyoteUsable = false;
            ToggleClimbingLadder(false);
            _speed.y = _jumpForce;
            // Jumped Invoke
            Jumped?.Invoke(false);
        }

        protected virtual void WallJump()
        {
            _endedJumpEarly = false;
            _bufferedJumpUsable = false;
            _isLeavingWall = true;
            _currentWallJumpMoveMultiplier = 0;
            _speed = Vector2.Scale(_wallJumpPower, new Vector2(-_wallDir, 1));
            // Jumped Invoke
            Jumped?.Invoke(true);
        }

        protected virtual void AirJump()
        {
            _endedJumpEarly = false;
            _airJumpsRemaining--;
            _speed.y = _jumpForce;
            // Air Jumped Invoke
            AirJumped?.Invoke();
        }

        protected virtual void ResetJump()
        {
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            ResetAirJumps();
        }

        public void AddAirJump() => _maxAirJumps++;
        protected virtual void ResetAirJumps() => _airJumpsRemaining = _maxAirJumps;
        #endregion

        #region Horizontal
        protected virtual void HorizontalMovement()
        {
            // Deceleracion
            if (_frameInput.Move.x == 0)
                _speed.x = Mathf.MoveTowards(_speed.x, 0, (_grounded ? _groundDeceleration : _airDeceleration) * Time.fixedDeltaTime);

            // Regular Horizontal Movement
            else
            {
                if (_wallHitCount > 0 && Mathf.Approximately(_rb.velocity.x, 0) && Mathf.Sign(_frameInput.Move.x) == Mathf.Sign(_speed.x)) // Prevents gaining speed when moving against a wall
                    _speed.x = 0;

                var inputX = _frameInput.Move.x * (_onLadder ? _ladderShimmySpeedMultiplier : 1);
                _speed.x = Mathf.MoveTowards(_speed.x, inputX * _currentMovementSpeed, _currentWallJumpMoveMultiplier * _acceleration * Time.fixedDeltaTime);
            }
        }
        #endregion

        #region Vertical
        protected virtual void HandleVertical()
        {
            // Ladder
            if (_onLadder)
            {
                var inputY = _frameInput.Move.y;
                _speed.y = inputY * (inputY > 0 ? _ladderClimbSpeed : _ladderSlideSpeed);
            }
            // Grounded & Slopes
            else if (_grounded && _speed.y <= 0f)
            {
                _speed.y = _groundingForce;

                if (TryGetGroundNormal(out _groundNormal))
                {
                    if (!Mathf.Approximately(_groundNormal.y, 1f))
                    {
                        _speed.y = _speed.x * -_groundNormal.x / _groundNormal.y;
                        if (_speed.x != 0) _speed.y += _groundingForce;
                    }
                }
            }
            // Wall Climbing & Sliding
            else if (_isOnWall && !_isLeavingWall)
            {
                if (_frameInput.Move.y > 0) _speed.y = _wallClimbSpeed;
                else if (_frameInput.Move.y < 0) _speed.y = -_maxWallFallSpeed;
                else _speed.y = Mathf.MoveTowards(Mathf.Min(_speed.y, 0), -_maxFallSpeed, _wallFallAcceleration * Time.deltaTime);
            }

            // In Air
            else
            {
                var inAirGravity = _fallAcceleration;
                if (_endedJumpEarly && _speed.y > 0) inAirGravity *= _jumpEndEarlyGravityModifier;
                _speed.y = Mathf.MoveTowards(_speed.y, -_maxFallSpeed, inAirGravity * Time.fixedDeltaTime);
            }
        }
        #endregion

        protected virtual void ApplyVelocity()
        {
            if (!_hasControl) return; // If the player doesn't have control, don't move
            _rb.velocity = _speed + _currentExternalVelocity;

            _currentExternalVelocity = Vector2.MoveTowards(_currentExternalVelocity, Vector2.zero, _externalVelocityDecay * Time.deltaTime);
        }

        #region Movement Stats
        [Header("Movement Stats")]
        // Movement
        protected float _acceleration = 120; // Capacity to gain horizontal speed
        protected float _groundDeceleration = 60; // Pace at which the player comes to a stop
        protected float _airDeceleration = 30; // Deceleration in air only after stopping input mid-air
        protected float _groundingForce = -1.5f; // Constant force applied to the player when grounded (Slopes)

        protected float _verticalDeadzoneThreshold = 0.3f; // Min input required to mount ladders
        protected float _horizontalDeadzoneThreshold = 0.1f; // Min input required to move


        // Jump
        protected int _maxAirJumps = 1; // Max amount of jumps the player can do in the air. 0 = No air jumps
        protected float _jumpForce = 36; // Inmediate force applied to the player when jumping
        public float JumpForce => _jumpForce;
        public void SetJumpForce(float jumpForce) => _jumpForce = jumpForce;
        protected float _maxFallSpeed = 40; // Max speed the player can fall at
        protected float _fallAcceleration = 100; // Acceleration applied to the player when falling
        protected float _jumpEndEarlyGravityModifier = 3; // Gravity modifier applied to the player when ending a jump early
        protected int _coyoteFrames = 7; // Amount of frames the player can jump after leaving the ground
        protected int _jumpBufferFrames = 7; // Amount of fixed frames we buffer a jump. This allows jump input before actually hitting the ground


        // Walls
        protected bool _allowWalls = true; // Allows wall slide / Jump
        [Header("Layers")]
        [SerializeField] protected LayerMask _wallLayerMask; // Layer mask for climbable walls are on
        protected bool _requireInputPush = false; // If true, the player must push against the wall to climb it

        protected float _wallClimbSpeed => _currentMovementSpeed / 2; // Speed at which the player climbs walls -------------- NEEDS TO BE HALF OF PLAYER SPEED
        protected float _wallFallAcceleration = 8; // Acceleration applied to the player when falling off a wall
        protected float _maxWallFallSpeed = 16; // Max speed the player can fall off a wall at
        protected Vector2 _wallJumpPower = new Vector2(30, 25); // Power applied to the player when jumping off a wall
        protected int _wallJumpInputLossFrames = 18; // The frames before full horizontal movement is returned after a wall jump


        // Ladders
        protected bool _allowLadders = true; // Allows ladder climbing
        protected bool _snapToLadders = true; // If true, the player will snap to the ladder when climbing it
        [SerializeField] protected LayerMask _ladderLayerMask; // Layer mask for climbable ladders are on
        protected float _ladderSnapSpeed = 0.05f;
        protected float _ladderShimmySpeedMultiplier = 0.5f; // Horizontal speed multiplier while attached to a ladder

        protected float _ladderClimbSpeed = 8; // Speed at which the player climbs ladders
        protected float _ladderSlideSpeed = 12;
        protected int _ladderCooldownFrames = 8; // How many frames can pass between ladder interactions


        // Collisions
        protected float _grounderDistance = 0.1f; // Distance from the player's feet to the ground
        protected Vector2 _wallDetectorSize = new Vector2(0.75f, 1.25f); // Size of the wall detector box
        [SerializeField] protected LayerMask _oneWayFloor; // Layer mask for ground is on
        [SerializeField] protected LayerMask _oneWayFloorNoCol; // Layer mask for ground is on
        [SerializeField] protected LayerMask _enemyLayer; // Layer mask for enemies are on


        // External
        protected int _externalVelocityDecay = 100;
        #endregion
        #endregion

        #region Getters & Setters
        public FrameInput GetFrameInput() => _frameInput;

        public bool IsGrounded() => _grounded;

        public bool IsFalling() => _speed.y < 0;

        public bool IsClimbing() => _onLadder || _isOnWall;
        #endregion

#if UNITY_EDITOR
        protected void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            var bounds = GetWallDetectionBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            var bound = new Bounds(_rb.position, _col.size);
            Gizmos.DrawWireCube(bound.center, bound.size);

            Gizmos.color = Color.red;
            var down = new Vector2(_col.bounds.center.x, -_col.bounds.center.y + 1);
            Gizmos.DrawLine(_col.bounds.center, down);

            Gizmos.color = Color.blue;
            var boundF = new Bounds(_rb.position, _col.size / 0.9f); // Player bounds
            Gizmos.DrawWireCube(boundF.center, boundF.size);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_col.bounds.center, _col.bounds.center + Vector3.up);

            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(_col.bounds.center, _col.bounds.center + Vector3.down);
        }

        protected void OnValidate()
        {
            if (_baseStats == null) Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
            if (_col == null) Debug.LogWarning("Please assign a Capsule Collider to the collider slot", this);
            if (_rb == null) _rb = GetComponent<Rigidbody2D>(); // serialized but hidden in the inspector
        }
#endif
    }

    public interface IPlayerController
    {
        public event Action<bool, float> GroundedChanged;
        public event Action<bool> WallGrabChanged;
        public event Action<bool> Jumped;
        public event Action AirJumped;

        public event Action BasicAttack1;
        public event Action SpecialAttack1;
        public event Action SpecialAttack2;
        public event Action UltimateAttack;

        public PlayerBaseStats PlayerStats { get; }
        public Vector2 Input { get; }
        public Vector2 Speed { get; }
        public Vector2 GroundNormal { get; }
        public int WallDirection { get; }
        public bool ClimbingLadder { get; }
        public void ApplyVelocity(Vector2 vel, PlayerForce force);
    }

    public enum PlayerForce
    {
        /// <summary>
        /// Added directly to the players movement speed, to be controlled by the standard deceleration
        /// </summary>
        Burst,

        /// <summary>
        /// An additive force handled by the decay system
        /// </summary>
        Decay
    }
}
