using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerData
{
    public int MaxHealth { get; private set; } = 4;
    public int MaxSoulFuel { get; private set; } = 0;

    public int HealthItems { get; private set; } = 0;
    public int MaxHealthItems { get; private set; } = 2;

    public int Health { get; private set; } = 4;
    public int SoulFuel { get; private set; } = 0;

    public bool HasAbility { get; private set; } = false;

    int _soulParts = 0;
    int _maxSoulParts = 0;

    public event Action<int> OnHealthChanged;
    public event Action<int> OnMaxHealthChanged;
    public event Action<int> OnHealthItemsChanged;
    public event Action<int> OnSoulFuelChanged;
    public event Action<int> OnSoulPartsChanged;
    public event Action<int> OnMaxSoulFuelChanged;
    public event Action OnDeath;


    public void SetupPlayer(PlayerStats stats)
    {
        MaxHealth = stats.MaxHealth;
        MaxHealthItems = stats.MaxHealthItems;
        MaxSoulFuel = stats.MaxSoulFuel;
        _maxSoulParts = stats.MaxSoulParts;

        ChangeHealth(stats.MaxHealth - Health);
        ReplenishHealthItems();
        ChangeSoulFuel(stats.MaxSoulFuel - SoulFuel);
    }

    public void GetUpgrade(PlayerUpgradeType type)
    {
        switch (type)
        {
            case PlayerUpgradeType.Health:
                MaxHealth++;
                OnMaxHealthChanged?.Invoke(MaxHealth);
                ChangeHealth(1);
                break;
            case PlayerUpgradeType.Souls:
                MaxSoulFuel++;
                OnMaxSoulFuelChanged?.Invoke(MaxSoulFuel);
                ChangeSoulFuel(1);
                break;
            case PlayerUpgradeType.Ability:
                HasAbility = true;
                break;
        }
    }

    public void OnRespawnActivated()
    {
        ChangeHealth(MaxHealth - Health);
        ReplenishHealthItems();
    }

    public void ChangeHealth(int amount)
    {
        Health = Math.Clamp(Health + amount, 0, MaxHealth);
        OnHealthChanged?.Invoke(Health);
        if (Health <= 0)
            OnDeath?.Invoke();
    }

    public void UseHealthItem()
    {
        HealthItems--;
        OnHealthItemsChanged?.Invoke(HealthItems);
        ChangeHealth(MaxHealth - Health);
    }

    public void ReplenishHealthItems()
    {
        HealthItems = MaxHealthItems;
        OnHealthItemsChanged?.Invoke(HealthItems);
    }

    public void DamageToSoul(int damage)
    {
        if (SoulFuel == MaxSoulFuel)
            return;

        _soulParts += damage;
        int newSoul = _soulParts / _maxSoulParts;
        if (newSoul > 0)
        {
            ChangeSoulFuel(newSoul);
            JSAM.AudioManager.PlaySound(OblivioSounds.PlayerSoulsRegen);  // TODO not here...
        }

        if (SoulFuel == MaxSoulFuel)
            _soulParts = 0;
        else
            _soulParts -= newSoul * _maxSoulParts;
        OnSoulPartsChanged?.Invoke(_soulParts);
    }

    public void ChangeSoulFuel(int amount)
    {
        int oldSoulFuel = SoulFuel;
        SoulFuel = Math.Clamp(oldSoulFuel + amount, 0, MaxSoulFuel);
        if (oldSoulFuel != SoulFuel)
            OnSoulFuelChanged?.Invoke(SoulFuel);
    }
}
