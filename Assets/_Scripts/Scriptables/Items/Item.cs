using System;
using System.Collections.Generic;
using UnityEngine;

namespace rene_roid_player {
    [CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/New Item", order = 2)]
    public class Item : ScriptableObject
    {
        public string Name;
        public string Description;
        public Sprite Icon;
        //public int Amount;

        [SerializeReference] public List<ItemBase> Items = new List<ItemBase>();

        #region Menu Items
        [ContextMenu(nameof(AddStats))] void AddStats() => Items.Add(new AddStats());
        [ContextMenu(nameof(HealOnKill))] void HealOnKill() => Items.Add(new HealOnKill());
        [ContextMenu(nameof(MoveFastWhenLowHealth))] void MoveFastWhenLowHealth() => Items.Add(new MoveFastWhenLowHealth());
        [ContextMenu(nameof(ChanceToDealExtraHit))] void ChanceToDealExtraHit() => Items.Add(new ChanceToDealExtraHit());

        // Game Items
        [ContextMenu(nameof(LionEmblem))] void LionEmblem() => Items.Add(new LionEmblem());
        [ContextMenu(nameof(Dopamine))] void Dopamine() => Items.Add(new Dopamine());
        [ContextMenu(nameof(MarcaShoes))] void MarcaShoes() => Items.Add(new MarcaShoes());
        [ContextMenu(nameof(Rock))] void Rock() => Items.Add(new Rock());
        [ContextMenu(nameof(Aspirin))] void Aspirin() => Items.Add(new Aspirin());
        [ContextMenu(nameof(SpringShoes))] void SpringShoes() => Items.Add(new SpringShoes());
        [ContextMenu(nameof(ImmovableSword))] void ImmovableSword() => Items.Add(new ImmovableSword());
        [ContextMenu(nameof(WingedShoes))] void WingedShoes() => Items.Add(new WingedShoes());
        [ContextMenu(nameof(FashionEars))] void FashionEars() => Items.Add(new FashionEars());
        [ContextMenu(nameof(MonsterWheights))] void MonsterWheights() => Items.Add(new MonsterWheights());
        #endregion
    }

    [Serializable]
    public class ItemBase
    {
        private bool _isInitialized = false;
        public virtual void OnGet(PlayerBase player, ItemManager itemManager)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // Activate item manager item
        }

        // In update case: Add Component to player (script of the item)

        public virtual void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            // Deactivate item manager item
        }
    }

    [System.Serializable]
    public class AddStats : ItemBase
    {
        public string Name = "Add Stats";

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

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
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

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
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

    [System.Serializable]
    public class HealOnKill : ItemBase {
        public string Name = "HealOnKill";
        public float HealAmount = 10;

        public HealOnKill() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.HealOnKillAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.HealOnKillAmount -= 1;  
        }
    }

    [System.Serializable]
    public class MoveFastWhenLowHealth : ItemBase {
        public string Name = "Move Fast When Low Health";
        public float SpeedBoost = 20f;

        public MoveFastWhenLowHealth() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.MoveFastWhenLowHealthAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.MoveFastWhenLowHealthAmount -= 1;  
        }
    }

    [System.Serializable]
    public class ChanceToDealExtraHit : ItemBase {
        public string Name = "Chance To Deal Extra Hit";
        public float Chance = 0.1f; // 10%
        public float DamageMultiplier = 1.5f; // 50% more damage

        public ChanceToDealExtraHit() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.ChanceToDealExtraHitAmount += 1;
        }
        
        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.ChanceToDealExtraHitAmount -= 1;  
        }
    }

    [System.Serializable]
    public class LionEmblem : ItemBase {
        public string Name = "Lion Emblem";
        public float DefenceIncrease = 5f;

        public LionEmblem() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.LionEmblemAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.LionEmblemAmount -= 1;  
        }

    }

    [System.Serializable]
    public class Dopamine : ItemBase {
        public string Name = "Dopamine";
        public float AttackMoveSpeedIncrease = 0.045f;

        public Dopamine() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.DopamineAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.DopamineAmount -= 1;  
        }
    }

    
    [System.Serializable]
    public class MarcaShoes : ItemBase {
        public string Name = "Marca Shoes";
        public float MovementSpeedIncrease = 0.08f;

        public MarcaShoes() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.MarcaShoesAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.MarcaShoesAmount -= 1;  
        }
    }

    
    [System.Serializable]
    public class Rock : ItemBase {
        public string Name = "Rock";
        public float Armor = 5f;

        public Rock() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.RockAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.RockAmount -= 1;  
        }
    }


    [System.Serializable]
    public class Aspirin : ItemBase {
        public string Name = "Aspirin";
        public float HealthRegen = 0.5f;

        public Aspirin() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.AspirinAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.AspirinAmount -= 1;  
        }
    }

    
    [System.Serializable]
    public class SpringShoes : ItemBase {
        public string Name = "Spring Shoes";
        public float JumpHeightIncrease = 0.1f;

        public SpringShoes() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.SpringShoesAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.SpringShoesAmount -= 1;  
        }
    }

    [System.Serializable]
    public class ImmovableSword : ItemBase {
        public string Name = "Immovable Sword";
        public float DamagePorcen = 0.15f;
        public float TimeToActive = 5f;
        public float TimeToDesactive = 7f;

        public ImmovableSword() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.ImmovableSwordAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.ImmovableSwordAmount -= 1;  
        }
    }

    [System.Serializable]
    public class MonsterWheights : ItemBase {
        public string Name = "Monster Weights";
        public float SpeedRedux = 0.1f;
        
        public MonsterWheights() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.MonsterWheightsAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.MonsterWheightsAmount -= 1;  
        }
    }

    [System.Serializable]
    public class WingedShoes : ItemBase {
        public string Name = "Winged Shoes";

        public WingedShoes() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.WingedShoesAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.WingedShoesAmount -= 1;  
        }
    }

    [System.Serializable]
    public class FashionEars : ItemBase {
        public string Name = "Fashion Ears";
        public float SpeedIncrease = 0.15f;
        public float TimeToDesactive = 3f;

        public FashionEars() { }

        public override void OnGet(PlayerBase player, ItemManager itemManager)
        {
            base.OnGet(player, itemManager);
            // Activate item manager item
            itemManager.FashionEarsAmount += 1;
        }

        public override void OnRemove(PlayerBase player, ItemManager itemManager)
        {
            base.OnRemove(player, itemManager);
            // Deactivate item manager item
            itemManager.FashionEarsAmount -= 1;  
        }
    }
}
