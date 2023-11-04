using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Globals
{
    public static Player Player;
    public const int BULLET_LIFE_TIME = 10;

    static int _awareEnemiesCount = 0;
    public static int AwareEnemiesCount 
    {
        get => _awareEnemiesCount;
        set
        {
            int newCount = value;
            // Debug.Log($"AwareEnemiesCount {_awareEnemiesCount} -> {value}");
            if (newCount < 0)
                Debug.LogError("AwareEnemiesCount is less than 0");
            if (newCount == 0)
                GlobalEvents.OnNoEnemiesAware.Publish();
            else if (newCount > 0 && _awareEnemiesCount == 0)
                GlobalEvents.OnAnyEnemyAware.Publish();
            _awareEnemiesCount = newCount;
        }
    }


    public static GlobalSO GlobalSO;
    public static EditorUserSettings EditorUserSettings;

    public static AnimationCurve KnockbackCurve => GlobalSO.KnockbackCurve;
    public static float KnockbackTime => GlobalSO.KnockbackTime;
    public static bool IsAbilityEnabledFromStart => EditorUserSettings.IsAbilityEnabledFromStart;
    public static DialogSettings DialogSettings => GlobalSO.DialogSettings;
    public static Material DamageFlashMaterial => GlobalSO.DamageFlashMaterial;
    public static bool NotShittyConrols => EditorUserSettings.NotShittyConrols;

    public static bool IsGameInitialized = false;


}


public static class GameColors
{
    public static Color GoldColor = new Color32(0xE5, 0xA5, 0x2D, 0xFF);
}

public static class PhysicsMasks
{
    public static int PlayerBulletTargetMask = LayerMask.GetMask("Enemy", "Ground", "Obstacle", "MeleeTarget", "InteractOnAttack");
    public static int EnemyBulletTargetMask = LayerMask.GetMask("Player", "Ground", "Obstacle", "MeleeTarget");
    public static int BouncableMask = LayerMask.GetMask("Obstacle", "Ground", "MeleeTarget");
    public static int DamageableMask = LayerMask.GetMask("Player", "Enemy");
    public static int PlayerMask = LayerMask.GetMask("Player");
    public static int EnemyMask = LayerMask.GetMask("Enemy");
    public static int GroundMask = LayerMask.GetMask("Ground");
    public static int BulletMask = LayerMask.GetMask("Bullet");
    public static int DefaultMask = LayerMask.GetMask("Default");
}


public static class AnimationHashes
{
    // player
    public static readonly int IsBattleMode = Animator.StringToHash("IsBattleMode");
    public static readonly int Forward = Animator.StringToHash("Forward");
    public static readonly int Right = Animator.StringToHash("Right");
    public static readonly int Dash = Animator.StringToHash("Dash");
    public static readonly int MeleeAttack = Animator.StringToHash("MeleeAttack");
    public static readonly int Shoot = Animator.StringToHash("Shoot");
    public static readonly int Die = Animator.StringToHash("Die");
    public static readonly int Respawn = Animator.StringToHash("Respawn");

    // enemy
    public static readonly int Aware = Animator.StringToHash("Aware");
    public static readonly int Moving = Animator.StringToHash("Moving");
    public static readonly int AttackPrep = Animator.StringToHash("AttackPrep");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int AttackRest = Animator.StringToHash("AttackRest");
    public static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
}


public static class CoroutineWaiters
{
    public static readonly WaitForEndOfFrame WaitForEndOfFrame = new();
}
