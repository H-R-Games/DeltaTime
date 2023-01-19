using rene_roid_enemy;
using System.Collections;
using System.Collections.Generic;
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

        void Update()
        {
            if (CurrentStageEnemies.Count < 0) return;
            DirectorsUpdate();
        }

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
        [SerializeField] private float _creditsPD = 120f;
        [SerializeField] private float _creditsPerSecondPD = 1.2f;

        [SerializeField] private float _creditsPerSecondPerLevelPD = 2f;
        [SerializeField] [Range(0, 1)] private float _creditPercentagePerEnemyPD = .2f;
        [SerializeField] [Range(0, 1)] private float _creditPercentageToStartSpawningPD = 0.8f;

        [SerializeField] private float _spawningDurationPD = 15f;
        [SerializeField] private float _timeBetweenEnemySpawnPD = .7f;
        [SerializeField] private float _timeBetweenWavesPD = 5f;

        [SerializeField] private float _maxEnemiesPerWavePD = 10f;
        [SerializeField] private float _maxEnemiesPerWaveLevelUpPD = 1f;

        [SerializeField] private PassiveDirectorState _passiveDirectorState = PassiveDirectorState.Innactive;

        // Smol vars
        private float _checkStartSpawningCooldown = 5f;
        private float _lastTime = 0;

        private void PassiveDirectorUpdate()
        {
            switch (_passiveDirectorState)
            {
                case PassiveDirectorState.Innactive:
                    break;
                case PassiveDirectorState.Gathering:
                    // Update
                    PassiveDirectorGathering();
                    
                    // Change state conditions
                    if (_lastTime + _checkStartSpawningCooldown <= Time.time)
                    {
                        _lastTime = Time.time;
                        if (StartSpawning()) PassiveDirectorChangeState(PassiveDirectorState.Spawning);
                    }
                    
                    break;
                case PassiveDirectorState.Spawning:
                    // Update
                    PassiveDirectorSpawning();

                    // Change state conditions
                    
                    break;
                case PassiveDirectorState.Waiting:
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
            print("Credit requirement: " + CurrentStageEnemies[Mathf.RoundToInt(Helpers.FromPercentageToRange(0.8f, 0, CurrentStageEnemies.Count))].EnemyBaseStats.Cost);
            return CurrentStageEnemies[Mathf.RoundToInt(Helpers.FromPercentageToRange(0.8f, 0, CurrentStageEnemies.Count))].EnemyBaseStats.Cost >= _creditsPD;
        }


        // ----------- Spawning

        private void PassiveDirectorSpawning()
        {
            // Spawn enemies every _timeBetweenEnemySpawnPD
            // Spawn for _spawningDurationPD seconds or until _maxEnemiesPerWavePD is reached or until _creditsPD is 0
            // Change state to waiting
        }
        #endregion

        private void GetCurrentStageEnemies()
        {
            for (int i = 0; i < AllEnemies.Count; i++)
            {
                if (AllEnemies[i].EnemyBaseStats.StageCondition.Contains(_currentStage))
                    CurrentStageEnemies.Add(AllEnemies[i]);
            }
        }
        #endregion
    }
}
