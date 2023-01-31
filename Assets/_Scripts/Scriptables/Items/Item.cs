using System;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_player {
    [CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/New Item", order = 2)]
    public class Item : ScriptableObject
    {
        public string Name;
        public Sprite Icon;
        public int Amount;

        [SerializeReference] public List<ItemBase> Items = new List<ItemBase>();

        #region Menu Items
        [ContextMenu(nameof(AddStats))] void AddStats() => Items.Add(new AddStats());
        #endregion
    }

    [Serializable]
    public class ItemBase
    {
        private bool _isInitialized = false;
        public virtual void OnGet(PlayerBase player)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // Add script component xd
        }

        // In update case: Add Component to player (script of the item)

        public virtual void OnRemove(PlayerBase player)
        {

        }
    }

    [System.Serializable]
    public class AddStats : ItemBase
    {
        public float Health;
        public float HealthPercentage;
        public float HealthRegen;
        public float HealthRegenPercentage;
        public float FlatDamageBonus;
        public float SpecialDamageBonus;
        public float DamageBonus;
        public float Armor;
        public float ArmorPercentage;
        public float MovementSpeed;
        public float MovementSpeedPercentage;

        public AddStats() { }

        public override void OnGet(PlayerBase player)
        {
            base.OnGet(player);
            // Add stats to player... To:Do
            if (Health != 0) player.AddHealthFlat(Health);
            if (HealthPercentage != 0) player.AddHealthPercentage(HealthPercentage);
            if (HealthRegen != 0) player.AddHealthRegenFlat(HealthRegen);
            if (HealthRegenPercentage != 0) player.AddHealthRegenPercentage(HealthRegenPercentage);
            if (FlatDamageBonus != 0) player.AddFlatDamageBonus(FlatDamageBonus);
            if (SpecialDamageBonus != 0) player.AddSpecialMultiplier(SpecialDamageBonus);
            if (DamageBonus != 0) player.AddPercentageDamageBonus(DamageBonus);
            if (Armor != 0) player.AddArmorFlat(Armor);
            if (ArmorPercentage != 0) player.AddArmorPercentage(ArmorPercentage);
            if (MovementSpeed != 0) player.AddMovementSpeedFlat(MovementSpeed);
            if (MovementSpeedPercentage != 0) player.AddMovementSpeedPercentage(MovementSpeedPercentage);
        }

        public override void OnRemove(PlayerBase player)
        {
            base.OnRemove(player);
            // Remove stats from player... To:Do
            if (Health != 0) player.RemoveHealthFlat(Health);
            if (HealthPercentage != 0) player.RemoveHealthPercentage(HealthPercentage);
            if (HealthRegen != 0) player.RemoveHealthRegenFlat(HealthRegen);
            if (HealthRegenPercentage != 0) player.RemoveHealthRegenPercentage(HealthRegenPercentage);
            if (FlatDamageBonus != 0) player.RemoveFlatDamageBonus(FlatDamageBonus);
            if (SpecialDamageBonus != 0) player.RemoveSpecialMultiplier(SpecialDamageBonus);
            if (DamageBonus != 0) player.RemovePercentageDamageBonus(DamageBonus);
            if (Armor != 0) player.RemoveArmorFlat(Armor);
            if (ArmorPercentage != 0) player.RemoveArmorPercentage(ArmorPercentage);
            if (MovementSpeed != 0) player.RemoveMovementSpeedFlat(MovementSpeed);
            if (MovementSpeedPercentage != 0) player.RemoveMovementSpeedPercentage(MovementSpeedPercentage);
        }
    }
}
