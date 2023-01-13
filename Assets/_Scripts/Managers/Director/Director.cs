using rene_roid_enemy;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid
{
    public class Director : MonoBehaviour
    {
        #region Internal
        [Header("Internal")]
        [SerializeField] private float _currentStage = 0;
        [SerializeField] private float _currentWheight = 0;
        private float _wheightRange = .2f;
        #endregion


        #region External
        public List<EnemyBase> Enemies = new List<EnemyBase>();
        #endregion

        void Start()
        {
            // Order Enemies by cost
            Enemies.Sort((x, y) => x.Cost.CompareTo(y.Cost));
        }

        void Update()
        {
            DirectorsUpdate();
        }

        #region Directors

        private void DirectorsUpdate()
        {
            PassiveDirector();
        }

        #region Passive Director
        [Header("Passive Director")]
        [SerializeField] private float _creditsPasssiveDirector;
        [SerializeField] private float _creditsPasssiveDirectorPerSecond;

        [SerializeField] private float _maxTimeBetweenSpawnsPassiveDirector;

        private bool _isPassiveDirectorActive;
        private float _timeBetweenSpawnsPassiveDirector;
        private bool _checkTimeBetweenSpawnsPassiveDirector;

        private void PassiveDirector()
        {
            if (!_isPassiveDirectorActive) return;
            _timeBetweenSpawnsPassiveDirector -= Time.deltaTime;

            GenerateCreditsPassiveDirector();

            SpawnEnemiesPassiveDirector();
        }


        private void GenerateCreditsPassiveDirector()
        {
            _creditsPasssiveDirector += _creditsPasssiveDirectorPerSecond * Time.deltaTime;
        }

        private void SpawnEnemiesPassiveDirector()
        {
            if (_timeBetweenSpawnsPassiveDirector > 0) return;

            // Spawn Enemies
            if (!CanSpawnEnemies(_creditsPasssiveDirector)) return;

            

            _timeBetweenSpawnsPassiveDirector = _maxTimeBetweenSpawnsPassiveDirector;
        }

        public void StartPassiveDirector()
        {
            _isPassiveDirectorActive = true;
        }

        public void StopPassiveDirector()
        {
            _isPassiveDirectorActive = false;
        }
        #endregion

        #region Director Functions
        private bool CanSpawnEnemies(float credits)
        {
            for (int i = 0; i < Enemies.Count; i++)
            {
                // If enemy stage conditions does not contain _currentStage continue
                if (!Enemies[i].StageCondition.Contains(_currentStage)) continue;

                // If enemy cost more than credits return continue
                if (Enemies[i].Cost > credits) continue;

                // If enemy is in the wheight range return true
                if (Enemies[i].Wheigth >= _currentWheight - (_currentWheight * _wheightRange) && Enemies[i].Wheigth <= _currentWheight + (_currentWheight * _wheightRange)) return true;

            }

            return false;
        }
        #endregion
        #endregion
    }
}
