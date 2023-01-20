using rene_roid_enemy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

namespace rene_roid
{
    public class Director : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        [SerializeField] private int _directorsLevel = 1;
        [SerializeField] private float _currentStage = 1;
        [SerializeField] private float _currentWheight = 0;
        private float _wheightRange = .2f;
        #endregion
        
        #region External
        public List<EnemyBase> AllEnemies = new List<EnemyBase>();
        public List<EnemyBase> CurrentStageEnemies = new List<EnemyBase>();
        #endregion

        void Start()
        {
            // Order Enemies by cost
            AllEnemies.Sort((x, y) => x.EnemyBaseStats.Cost.CompareTo(y.EnemyBaseStats.Cost));

            GetCurrentStageEnemies();
        }

        // TESINT -----------------
        //private void Test()
        //{
        //    float t = 0;

        //    while (t < 1)
        //    {
        //        t += Time.deltaTime / 1;
        //        // Move the cube with sine wave
        //        float y = Mathf.Sin(t * 2 * Mathf.PI);
        //        Cube.transform.position = new Vector3(Cube.transform.position.x, Mathf.Abs(y), Cube.transform.position.z);
        //        print(Mathf.Sin(t * 2 * Mathf.PI) * 2);
        //        yield return null;
        //    }

        //    print("Done");

        //    StartCoroutine(Test());
        //}
        // TESINT -----------------

        void Update()
        {
            if (CurrentStageEnemies.Count < 0) return;
            DirectorsUpdate();

            Test();
        }

        // TESINT -----------------
        public GameObject Cube;
        [Range(0, 1)] public float posSin = 0;
        public float range = 0;
        private void Test()
        {
            var x = Mathf.Sin(posSin * Mathf.PI);
            //print(x);

            range = Mathf.Clamp(x, 0.2f, 1);

            Cube.transform.position = new Vector3(Cube.transform.position.x, x, Cube.transform.position.z);
        }
        // TESINT -----------------

        #region Directors

        private void DirectorsUpdate()
        {
            PassiveDirectorUpdate();
        }

        private void DirectorLevelUp()
        {
            
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
        [SerializeField] [Range(0, 1)] private float _creditPercentageToStartSpawningPD = 80f;
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

        // Waiting --
        private float _waitTimePD = 0;

        private void PassiveDirectorUpdate()
        {
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
            return CurrentStageEnemies[Mathf.RoundToInt(Helpers.FromPercentageToRange(_creditPercentageToStartSpawningPD, 0, CurrentStageEnemies.Count))].EnemyBaseStats.Cost * multiplier <= _creditsPD;
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

            // Spawn enemy
            _waitBetweenEnemySpawnTimePD = Time.time;
            SpawnEnemy();

            // Check if end of wave
            //if (CurrentStageEnemies[0].EnemyBaseStats.Cost < _creditsPD) return; // Still have more credits
            //if (_timeSpawningPD < _spawningDurationPD) return; // Not spent max time spawning
            //if (_enemiesSpawnedPD < _maxEnemiesPerWavePD) return; // Not spawned max ammount of enemies

            // Check if end of wave
            if (CurrentStageEnemies[0].EnemyBaseStats.Cost > _creditsPD || _timeSpawningPD > _spawningDurationPD || _enemiesSpawnedPD >= _maxEnemiesPerWavePD)
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
            _enemiesSpawnedPD++;
            _creditsPD -= 0;
        }

        // ----------- Waiting

        #endregion

        private void GetWheightRange()
        {
            var x = Mathf.Sin(Helpers.FromRangeToPercentage(_directorsLevel, 0, 100, true) * Mathf.PI);
            _wheightRange = Mathf.Clamp(x, 0.2f, 1);
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
