using rene_roid_enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using rene_roid_player;

namespace rene_roid
{
    public class Director : PersistentSingleton<Director>
    {
        #region Internal
        [Header("Internal")]
        [SerializeField] private int _directorsLevel = 1;
        [SerializeField] private float _currentStage = 1;
        private float _weightRange = .2f;

        private int _maxNumEnemiesInScene = 400;
        private PlayerBase _playerBase;
        #endregion
        
        #region External
        public List<EnemyBase> AllEnemies = new List<EnemyBase>();
        public List<EnemyBase> CurrentStageEnemies = new List<EnemyBase>();

        public int CurrentEnemiesInSceneCount = 0;
        #endregion

        void Start()
        {
            // Order Enemies by cost
            AllEnemies.Sort((x, y) => x.EnemyBaseStats.Cost.CompareTo(y.EnemyBaseStats.Cost));
            GetCurrentStageEnemies();        

            _playerBase = FindObjectOfType<PlayerBase>();
        }

        void Update()
        {
            ConstantLvlUp();
            LevelUpDirectors();
            if (CurrentStageEnemies.Count < 0) return;
            DirectorsUpdate();
        }

        #region Directors

        private void DirectorsUpdate()
        {
            PassiveDirectorUpdate();
            ActiveDirectorUpdate();
            TeleportEventUpdate();
        }

        private void DirectorLevelUp()
        {
            _directorsLevel++;
        }

        #region Passive Director
        private enum PassiveDirectorState
        {
            Innactive,
            Gathering,
            Spawning,
            Waiting
        }

        [Header("Passive Director")]
        [SerializeField] private PassiveDirectorState _passiveDirectorState = PassiveDirectorState.Innactive;
        
        [SerializeField] private float _creditsPD = 120f;
        [SerializeField] private float _creditsPerSecondPD = 1.2f;

        [SerializeField] private float _creditsPerSecondPerLevelPD = 2f;
        [SerializeField] [Range(0, 1)] private float _creditPercentagePerEnemyPD = .2f;
        [SerializeField] [Range(0, 100)] private float _creditPercentageToStartSpawningPD = 80f;
        [SerializeField] private float _multiplierToSpawnEnemies = 5;

        [SerializeField] private float _spawningDurationPD = 15f;
        [SerializeField] private float _timeBetweenEnemySpawnPD = .7f;
        [SerializeField] private float _timeBetweenWavesPD = 5f;

        [SerializeField] private float _maxEnemiesPerWavePD = 10f;
        [SerializeField] private float _maxEnemiesPerWaveLevelUpPD = 1f;

        // Smol vars
        // Gethering --
        private float _checkStartSpawningCooldownPD = 5f;
        private float _lastTimePD = 0;

        private int _minIndexPD = 0;

        // Waiting --
        private float _waitTimePD = 0;

        
        private void PassiveDirectorUpdate()
        {
            if (CurrentEnemiesInSceneCount >= _maxNumEnemiesInScene) return;

            switch (_passiveDirectorState)
            {
                case PassiveDirectorState.Innactive: // The director is innactive
                    break;
                case PassiveDirectorState.Gathering: // Needs credits to spawn enemies
                    // Update
                    PassiveDirectorGathering();
                    
                    // Change state conditions
                    if (_lastTimePD + _checkStartSpawningCooldownPD <= Time.time)
                    {
                        _lastTimePD = Time.time;
                        if (StartSpawning()) PassiveDirectorChangeState(PassiveDirectorState.Spawning);
                    }
                    
                    break;
                case PassiveDirectorState.Spawning: // Spawning enemies (Spending credits)
                    // Update
                    PassiveDirectorSpawning();

                    // Change state conditions
                    
                    break;
                case PassiveDirectorState.Waiting: // Check if the director can start spawning again or if it needs to gather more credits
                    // Update
                    PassiveDirectorGathering();
                    if (_waitTimePD + _timeBetweenWavesPD > Time.time) return; // If has not waited for the time between waves, return

                    // Change state conditions
                    if (StartSpawning()) PassiveDirectorChangeState(PassiveDirectorState.Spawning);
                    else PassiveDirectorChangeState(PassiveDirectorState.Gathering);
                    break;
                default:
                    break;
            }
        }

        private void PassiveDirectorChangeState(PassiveDirectorState newState)
        {
            // Exit current state
            switch (_passiveDirectorState)
            {
                case PassiveDirectorState.Innactive:
                    break;
                case PassiveDirectorState.Gathering:
                    break;
                case PassiveDirectorState.Spawning:
                    break;
                case PassiveDirectorState.Waiting:
                    break;
                default:
                    break;
            }

            // Enter new state
            switch (newState)
            {
                case PassiveDirectorState.Innactive:
                    break;
                case PassiveDirectorState.Gathering:
                    break;
                case PassiveDirectorState.Spawning:
                    _checkStartSpawningCooldownPD = 0;
                    _lastTimePD = 0;
                    _minIndexPD = 0;
                    _waitTimePD = 0;
                    _timeSpawningPD = 0;
                    _waitBetweenEnemySpawnTimePD = 0;
                    _enemiesSpawnedPD = 0;
                    break;
                case PassiveDirectorState.Waiting:
                    _waitTimePD = Time.time; // Set wait time when entering the state
                    break;
                default:
                    break;
            }

            _passiveDirectorState = newState;
        }


        // ----------- Passive Director States -----------

        // ----------- Gathering
        private void PassiveDirectorGathering()
        {
            _creditsPD += _creditsPerSecondPD * Time.deltaTime;
        }
        
        private bool StartSpawning()
        {
            var multiplier = Mathf.RoundToInt(Helpers.FromPercentageToRange(_creditPercentageToStartSpawningPD, 0, CurrentStageEnemies.Count));
            return CurrentStageEnemies[Mathf.RoundToInt(Helpers.FromPercentageToRange(_creditPercentageToStartSpawningPD, 0, CurrentStageEnemies.Count - 1))].EnemyBaseStats.Cost * multiplier <= _creditsPD;
        }


        // ----------- Spawning
        private float _timeSpawningPD = 0;
        private float _waitBetweenEnemySpawnTimePD = 0;
        private int _enemiesSpawnedPD = 0;
        private void PassiveDirectorSpawning()
        {
            // Spawn enemies every _timeBetweenEnemySpawnPD
            // Spawn for _spawningDurationPD seconds or until _maxEnemiesPerWavePD is reached or until _creditsPD is 0
            // Change state to waiting
            _timeSpawningPD += Time.deltaTime;
            if (_waitBetweenEnemySpawnTimePD + _timeBetweenEnemySpawnPD > Time.time) return; // Wait _timeBetweenEnemiesPD seconds between each enemy spawn

            //* Spawn enemy
            _waitBetweenEnemySpawnTimePD = Time.time;
            SpawnEnemy();

            // Check if end of wave
            if (CurrentStageEnemies[_minIndexPD].EnemyBaseStats.Cost > _creditsPD || _timeSpawningPD > _spawningDurationPD || _enemiesSpawnedPD >= _maxEnemiesPerWavePD)
            {
                // Reset
                _timeSpawningPD = 0;
                _waitBetweenEnemySpawnTimePD = Time.time;
                _enemiesSpawnedPD = 0;
                PassiveDirectorChangeState(PassiveDirectorState.Waiting);                
            }
        }


        private void SpawnEnemy()
        {
            // Instantiate enemy
            var enemy2spawn = ChooseEnemeyToSpawnPD();

            print("Enemy is null: " + (enemy2spawn == null));
            if (enemy2spawn == null) return;

            _enemiesSpawnedPD++;
            _creditsPD -= enemy2spawn.EnemyBaseStats.Cost;

            print($"Spawning enemy: {enemy2spawn.name} | Cost: {enemy2spawn.EnemyBaseStats.Cost} | Credits left: {_creditsPD}");

            var enemy = Instantiate(enemy2spawn, SpawnPosition(), Quaternion.identity);
        }

        private EnemyBase ChooseEnemeyToSpawnPD()
        {
            GetWheightRange(); // Update wheight range

            // var xpos = Helpers.FromRangeToPercentage(_directorsLevel, 0, 100, true) * 100;
            // var minIndex = Mathf.FloorToInt(xpos - (xpos * (_weightRange / 2)));
            // var maxIndex = Mathf.CeilToInt(xpos + (xpos * (_weightRange / 2)));
            // _minIndexPD = minIndex;
            // print($"minIndex: {minIndex} | maxIndex: {maxIndex} | xpos: {xpos}");

            // // Get enemies inside the weight range
            // List<EnemyBase> enemies = new List<EnemyBase>();
            // for (int i = 0; i < CurrentStageEnemies.Count; i++)
            // {
            //     var fatness = CurrentStageEnemies[i].EnemyBaseStats.Weight;
            //     print($"Enemy: {CurrentStageEnemies[i].name} | Fatness: {fatness} | minIndex: {minIndex} | maxIndex: {maxIndex}");
            //     if (fatness >= minIndex && fatness <= maxIndex) enemies.Add(CurrentStageEnemies[i]);
            // }

            // print($"Enemies inside range: {enemies.Count}");

            // // Get random enemy from enemies list
            // if (enemies.Count == 0) return null;
            return CurrentStageEnemies[Random.Range(0, CurrentStageEnemies.Count)];
        }

        // ----------- Waiting

        private Vector2 SpawnPosition() {
            // Spawn the enemy in a random position outside the camera view
            var camera = Helpers.Camera;
            var cameraPos = camera.transform.position;
            var cameraSize = camera.orthographicSize;
            var cameraWidth = cameraSize * camera.aspect;

            var spawnPos = new Vector2(0, _playerBase.transform.position.y + 2);
            
            // Get random position outside the camera view in the X axis
            var randomX = Random.Range(-cameraWidth, cameraWidth);
            if (randomX > 0) spawnPos.x = cameraPos.x + cameraWidth + 1;
            else spawnPos.x = cameraPos.x - cameraWidth - 1;

            // Raycast to the x position only detect layers 0 && 8
            // Direction is from player to spawn position
            var direction = spawnPos - (Vector2)_playerBase.transform.position;
            var hit = Physics2D.Raycast(_playerBase.transform.position, direction, direction.magnitude, 1 << 0 | 1 << 8);

            // If raycast hit something, spawn the enemy in the hit position and put it a little bit near the player direction
            if (hit.collider != null) {
                spawnPos = hit.point;
                spawnPos -= direction.normalized * 2;
            }
            
            return spawnPos;
        }
        #endregion

        #region Active Director
        private enum ActiveDirectorState
        {
            Innactive,
            Activate,
            Spawning,
            Deactivate
        }
        [Header("Active Director")]
        [SerializeField] private ActiveDirectorState _activeDirectorState = ActiveDirectorState.Innactive;
        [SerializeField] private float _creditsAC = 0;
        [SerializeField] private float _creditsOnActivateAC = 100;
        [SerializeField] private float _extraCreditsPerLevelAC = 25;

        [SerializeField] private float _timeBetweenEnemySpawnAC = 1;

        private int _minIndexAC = 0;


        /* --- Workflow ---
         * Innactive --> Activate
         * On activate --> Get initial credits
         * Change state to spawn
         * On enter spawn start spawn coroutine
         * End spawn --> Change to Deactivate
         * Send unused credits to passive director
         * Change to innactive
         */
        private void ActiveDirectorUpdate()
        {
            if (CurrentEnemiesInSceneCount >= _maxNumEnemiesInScene) return;

            switch (_activeDirectorState)
            {
                case ActiveDirectorState.Innactive:
                    break;
                case ActiveDirectorState.Activate:
                    _creditsAC += _creditsOnActivateAC;
                    ActiveDirectorChangeState(ActiveDirectorState.Spawning);
                    break;
                case ActiveDirectorState.Spawning:
                    break;
                case ActiveDirectorState.Deactivate:
                    ActiveDirectorChangeState(ActiveDirectorState.Innactive);
                    break;
                default:
                    break;
            }
        }

        private void ActiveDirectorChangeState(ActiveDirectorState newState)
        {
            // Exit current state
            switch (_activeDirectorState)
            {
                case ActiveDirectorState.Innactive:
                    break;
                case ActiveDirectorState.Activate:
                    break;
                case ActiveDirectorState.Spawning:
                    break;
                case ActiveDirectorState.Deactivate:
                    break;
                default:
                    break;
            }

            // Enter new state
            switch (newState)
            {
                case ActiveDirectorState.Innactive:
                    break;
                case ActiveDirectorState.Activate:
                    _creditsAC += _creditsOnActivateAC;
                    break;
                case ActiveDirectorState.Spawning:
                    StartCoroutine(SpawnEnemiesActiveDirector());
                    break;
                case ActiveDirectorState.Deactivate:
                    _creditsPD += _creditsAC;
                    _creditsAC = 0;
                    break;
                default:
                    break;
            }

            _activeDirectorState = newState;
        }

        private IEnumerator SpawnEnemiesActiveDirector()
        {
            GetWheightRange(); // Update wheight range

            var xpos = Helpers.FromRangeToPercentage(_directorsLevel, 0, 100, true) * 100;
            var minIndex = Mathf.FloorToInt(xpos - (xpos * (_weightRange / 2)));
            var maxIndex = Mathf.CeilToInt(xpos + (xpos * (_weightRange / 2)));
            _minIndexAC = minIndex;

            minIndex = Mathf.Clamp(minIndex, 0, 80);
            maxIndex = Mathf.Clamp(maxIndex, 20, 100);
            print($"minIndex: {minIndex} | maxIndex: {maxIndex} | xpos: {xpos}");

            // Get enemies inside the weight range
            List<EnemyBase> enemies = new List<EnemyBase>();
            for (int i = 0; i < CurrentStageEnemies.Count; i++)
            {
                var fatness = CurrentStageEnemies[i].EnemyBaseStats.Weight;
                if (fatness >= minIndex && fatness <= maxIndex) enemies.Add(CurrentStageEnemies[i]);
            }

            if (enemies == null) yield break;

            enemies.Sort((x, y) => x.EnemyBaseStats.Cost.CompareTo(x.EnemyBaseStats.Cost));
            var minCost = enemies[_minIndexAC].EnemyBaseStats.Cost;

            while (_creditsAC > minCost)
            {
                //* Spawn enemy
                var enemy = enemies[Random.Range(0, enemies.Count)];
                // Remove credits
                _creditsAC -= enemy.EnemyBaseStats.Cost;

                print($"Spawning enemy: {enemy.EnemyBaseStats.name} | Cost: {enemy.EnemyBaseStats.Cost} | Credits left: {_creditsAC}");

                yield return Helpers.GetWait(_timeBetweenEnemySpawnAC);
            }

            ActiveDirectorChangeState(ActiveDirectorState.Deactivate);
        }
        #endregion

        #region Teleport Event Director
        private enum TeleportEventDirectorState { Innactive, Activate, Gathering, Spawning, Waiting, Deactivate }
        [Header("Teleport Event Director")]
        [SerializeField] private TeleportEventDirectorState _teleportEventDirectorState = TeleportEventDirectorState.Innactive;
        
        [SerializeField] private float _creditsTED = 0;
        [SerializeField] private float _creditsOnActivateTED = 100;
        [SerializeField] private float _extraCreditsPerLevelTED = 25;
        [SerializeField] private float _creditsPerSecondTED = 2;

        [SerializeField] private float _timeBetweenEnemySpawnTED = 1;

        // Smol vars
        private float _checkGatherTEDFreq = 3;
        private float _timeGatherTEDFreq = 0;
        private int _minIndexTED = 0;
        private float _lastSpawnTimeTED = 0;
        
        private float _timeBetweenWavesTED = 10;
        private float _lastWaveTimeTED = 0;

        private void TeleportEventUpdate()
        {
            switch (_teleportEventDirectorState)
            {
                case TeleportEventDirectorState.Innactive:
                    break;
                case TeleportEventDirectorState.Activate:
                    GatherCreditsTED();
                    if (_timeGatherTEDFreq + _checkGatherTEDFreq > Time.time) ChangeStateTED(TeleportEventDirectorState.Spawning);
                    break;
                case TeleportEventDirectorState.Gathering:
                    GatherCreditsTED();

                    if (_timeGatherTEDFreq + _checkGatherTEDFreq > Time.time && CanSpawnPED()/* && Check if can spawn */) ChangeStateTED(TeleportEventDirectorState.Spawning); // Spawn();
                    break;
                case TeleportEventDirectorState.Spawning:
                    if (CurrentEnemiesInSceneCount >= _maxNumEnemiesInScene) return;

                    SpawnEnemiesTED();
                    break;
                case TeleportEventDirectorState.Waiting:
                    GatherCreditsTED();

                    if (_lastWaveTimeTED + _timeBetweenWavesTED < Time.time) ChangeStateTED(TeleportEventDirectorState.Gathering);
                    break;
                case TeleportEventDirectorState.Deactivate:
                    break;
                default:
                    break;
            }
        }

        private void ChangeStateTED(TeleportEventDirectorState newState)
        {
            // Exit current state
            switch (_teleportEventDirectorState)
            {
                case TeleportEventDirectorState.Innactive:
                    break;
                case TeleportEventDirectorState.Activate:
                    break;
                case TeleportEventDirectorState.Gathering:
                    break;
                case TeleportEventDirectorState.Spawning:
                    break;
                case TeleportEventDirectorState.Waiting:
                    break;
                case TeleportEventDirectorState.Deactivate:
                    break;
                default:
                    break;
            }

            // Enter new state
            switch (newState)
            {
                case TeleportEventDirectorState.Innactive:
                    break;
                case TeleportEventDirectorState.Activate:
                    _creditsTED += _creditsOnActivateTED;
                    _timeGatherTEDFreq = Time.time;
                    break;
                case TeleportEventDirectorState.Gathering:
                    _timeGatherTEDFreq = Time.time;
                    break;
                case TeleportEventDirectorState.Spawning:
                    _creditsTED *= 1.5f;
                    _lastSpawnTimeTED = Time.time;
                    break;
                case TeleportEventDirectorState.Waiting:
                    break;
                case TeleportEventDirectorState.Deactivate:
                    break;
                default:
                    break;
            }

            _teleportEventDirectorState = newState;
        }

        private void GatherCreditsTED()
        {
            if (_teleportEventDirectorState != TeleportEventDirectorState.Gathering) _creditsTED += _extraCreditsPerLevelTED * Time.deltaTime;
            else _creditsTED += (_creditsPerSecondTED * Time.deltaTime) * 2;
        }

        private bool CanSpawnPED()
        {
            GetWheightRange();
            var multiplier = Mathf.RoundToInt(Helpers.FromPercentageToRange(_creditPercentageToStartSpawningPD, 0, CurrentStageEnemies.Count));
            return CurrentStageEnemies[Mathf.RoundToInt(Helpers.FromPercentageToRange(_creditPercentageToStartSpawningPD, 0, CurrentStageEnemies.Count))].EnemyBaseStats.Cost * multiplier <= _creditsPD;
        }

        private void SpawnEnemiesTED()
        {
            if (_lastSpawnTimeTED + _timeBetweenEnemySpawnTED > Time.time) return;

            // Instantiate enemy
            var enemy2spawn = ChooseEnemeyToSpawnTED();
            if (enemy2spawn == null) return;
            print("Enemy is null: " + (enemy2spawn == null));
            print($"Spawning enemy: {enemy2spawn.name} | Cost: {enemy2spawn.EnemyBaseStats.Cost} | Credits left: {_creditsTED}");
            _enemiesSpawnedPD++;
            _creditsTED -= enemy2spawn.EnemyBaseStats.Cost;
            //* Spawn actual enemy

            print("Credits left: " + _creditsTED + " | Credits needed: " + CurrentStageEnemies[_minIndexTED].EnemyBaseStats.Cost);
            if (_creditsTED < CurrentStageEnemies[_minIndexTED].EnemyBaseStats.Cost)
            {
                ChangeStateTED(TeleportEventDirectorState.Waiting);
            }

            _lastSpawnTimeTED = Time.time;
        }

        private EnemyBase ChooseEnemeyToSpawnTED()
        {
            GetWheightRange(); // Update wheight range

            var xpos = Helpers.FromRangeToPercentage(_directorsLevel, 0, 100, true) * 100;
            var minIndex = Mathf.FloorToInt(xpos - (xpos * (_weightRange / 2)));
            var maxIndex = Mathf.CeilToInt(xpos + (xpos * (_weightRange / 2)));
            _minIndexTED = minIndex;
            print($"minIndex: {minIndex} | maxIndex: {maxIndex} | xpos: {xpos}");

            // Get enemies inside the weight range
            List<EnemyBase> enemies = new List<EnemyBase>();
            for (int i = 0; i < CurrentStageEnemies.Count; i++)
            {
                var fatness = CurrentStageEnemies[i].EnemyBaseStats.Weight;
                if (fatness >= minIndex && fatness <= maxIndex) enemies.Add(CurrentStageEnemies[i]);
            }

            // Get random enemy from enemies list
            if (enemies.Count == 0) return null;
            return enemies[Random.Range(0, enemies.Count)];
        }

        #endregion

        private int _lastLevel = 0;
        private void LevelUpDirectors() {
            if (_lastLevel == _directorsLevel) return;
            _lastLevel = _directorsLevel;

            // Level up directors
            _creditsPerSecondPD = _creditsPerSecondPD + (_creditsPerSecondPerLevelPD * _directorsLevel);
            
            _creditsOnActivateAC = _creditsOnActivateAC + (_creditsOnActivateAC * _directorsLevel);

            _creditsOnActivateTED = _creditsOnActivateTED + (_creditsOnActivateTED * _directorsLevel);
            _creditsPerSecondTED = _creditsPerSecondTED + (_creditsPerSecondTED * _directorsLevel);
        }

        private float _exp = 0;
        private float _expToLevelUp = 100;
        private float _expPerSecond = 0.3f;
        private float _difficulty = 1;
        private void ConstantLvlUp() {
            // if (_directorsLevel == 0) return;
            GetExp();

            // Level up
            if (_exp >= _expToLevelUp) {
                _exp = 0;
                _directorsLevel++;

                _expToLevelUp = _expToLevelUp * 1.5f;
            }

            void GetExp() {
                _exp += _expPerSecond * Time.deltaTime * _difficulty;
                if (_exp >= _expToLevelUp) {
                    _exp = 0;
                    _directorsLevel++;
                }
            }
        }

        public void AddExp(float exp) {
            _exp += exp;
        }

        private void GetWheightRange()
        {
            var x = Mathf.Sin(Helpers.FromRangeToPercentage(_directorsLevel, 0, 100, true) * Mathf.PI);
            _weightRange = Mathf.Clamp(x, 0.2f, 1);
        }

        private void GetCurrentStageEnemies()
        {
            for (int i = 0; i < AllEnemies.Count; i++)
            {
                if (AllEnemies[i].EnemyBaseStats.StageCondition.Contains(_currentStage))
                    CurrentStageEnemies.Add(AllEnemies[i]);
            }

            CurrentStageEnemies.Sort((x, y) => x.EnemyBaseStats.Cost.CompareTo(y.EnemyBaseStats.Cost));
        }
        #endregion
    }
}
