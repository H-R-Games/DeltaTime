using UnityEngine;

namespace rene_roid_enemy
{
    public class Intelligentenemy : EnemyBase
    {
        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            UpdateState();
        }

        #region State Machine
        public override void UpdateState()
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    break;
                case EnemyStates.Attack:
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
                    break;
            }
        }

        public override void ChangeState(EnemyStates newState)
        {
            switch (_enemyState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    break;
                case EnemyStates.Attack:
                    break;
                case EnemyStates.Stun:
                    break;
                case EnemyStates.Target:
                    break;
            }

            switch (newState)
            {
                case EnemyStates.Idle:
                    break;
                case EnemyStates.Move:
                    break;
                case EnemyStates.Attack:
                    break;
                case EnemyStates.Stun:
                    StunnStart(5);
                    break;
                case EnemyStates.Target:
                    break;
            }
            _enemyState = newState;
        }
        #endregion

#if UNITY_EDITOR
        private void OnGUI()
        {
            
        }

        private void OnDrawGizmos()
        {

        }
#endif
    }
}