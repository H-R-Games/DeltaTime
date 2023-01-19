using UnityEngine;

namespace rene_roid_enemy {
    [CreateAssetMenu(fileName = "EnemyBaseStats", menuName = "ScriptableObjects/EnemyBaseStats", order = 1)]
    public class EnemyBaseStats : ScriptableObject
    {
        [Header("Enemy Info")]
        [Tooltip("The enemy's name")]
        public string Name = "Enemy";

        [Tooltip("The enemy's description")]
        public string Description = "Enemy Description";

        [Tooltip("The enemy cost")]
        public float Cost = 6;

        [Tooltip("The enemy's weight")]
        public float Weight = 1;
        
        [Tooltip("The enemy's stage condition")]
        public System.Collections.Generic.List<float> StageCondition = new System.Collections.Generic.List<float>();

        [Header("Enemy stats")]
        [Tooltip("The enemy's health")]
        public float Health = 80f;

        [Tooltip("The enemy's damage")]
        public float Damage = 12f;

        [Tooltip("The enemy's armor")]
        public float Armor = 0f;

        [Tooltip("The enemy's base movement speed")]
        public float MovementSpeed = 6f;


        [Header("Leveling stats")]
        [Tooltip("The enemy's health per level")]
        public float HealthPerLevel = 24f;

        [Tooltip("The enemy's damage per level")]
        public float DamagePerLevel = 2.4f;

        [Tooltip("The enemy's armor per level")]
        public float ArmorPerLevel = 0f;

        [Tooltip("The enemy's movement speed per level")]
        public float MovementSpeedPerLevel = 0f;
    }
}
