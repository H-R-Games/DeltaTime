using UnityEngine;

namespace rene_roid_player {
    [CreateAssetMenu(fileName = "PlayerBaseStats", menuName = "ScriptableObjects/PlayerBaseStats", order = 1)]
    public class PlayerBaseStats : ScriptableObject
    {
        [Header("Player Stats")]
        [Tooltip("The player's name")]
        public string Name = "Player";

        [Tooltip("The player's health")]
        public float Health = 100f;

        [Tooltip("The player's health regeneration/s")]
        public float HealthRegen = 1f;

        [Tooltip("The player's base damage")]
        public float Damage = 12f;

        [Tooltip("The player's armor")]
        public float Armor = 0f;

        [Tooltip("The player's base movement speed")]
        public float MovementSpeed = 14f;


        [Header("Leveling Stats")]
        [Tooltip("The player's health per level")]
        public float HealthPerLevel = 33f;

        [Tooltip("The player's health regeneration per level")]
        public float HealthRegenPerLevel = 0.2f;

        [Tooltip("The player's damage per level")]
        public float DamagePerLevel = 2.4f;

        [Tooltip("The player's armor per level")]
        public float ArmorPerLevel = 0f;

        [Tooltip("The player's movement speed per level")]
        public float MovementSpeedPerLevel = 0f;
    }
}
