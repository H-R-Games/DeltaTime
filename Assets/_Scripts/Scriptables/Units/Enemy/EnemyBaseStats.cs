using UnityEngine;

namespace rene_roid_enemy {
    [CreateAssetMenu(fileName = "EnemyBaseStats", menuName = "ScriptableObjects/EnemyBaseStats", order = 1)]
    public class EnemyBaseStats : ScriptableObject
    {
        [Header("Enemy stats")]
        [Tooltip("The enemy's name")]
        public string Name = "Enemy";

        [Tooltip("The enemy's health")]
        public float Health = 80f;

        [Tooltip("The enemy's damage")]
        public float Damage = 12f;

        [Tooltip("The enemy's armor")]
        public float Armor = 0f;

        [Tooltip("The enemy's base movement speed")]
        public float MovementSpeed = 6f;

        [Tooltip("The enemy's money reward")]
        public int MoneyReward = 10;

        [Tooltip("The enemy's experience reward")]
        public int ExperienceReward = 10;


        [Header("Leveling stats")]
        [Tooltip("The enemy's health per level")]
        public float HealthPerLevel = 24f;

        [Tooltip("The enemy's damage per level")]
        public float DamagePerLevel = 2.4f;

        [Tooltip("The enemy's armor per level")]
        public float ArmorPerLevel = 0f;

        [Tooltip("The enemy's movement speed per level")]
        public float MovementSpeedPerLevel = 0f;

        [Tooltip("The enemy's money reward per level")]
        public int MoneyRewardPerLevel = 2;

        [Tooltip("The enemy's experience reward per level")]
        public int ExperienceRewardPerLevel = 2;
    }
}
