using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntStats : EnemyStats, IStats
{
    public float BaseAlertSpeed = 0.85f;
    public float BasePatrolSpeed = 0.75f;
    public float BaseFovAngleStrong = 70;
    public float BaseDetectRangeStrong = 2f;
    public float BaseBulletDamage = 5f;
    public float BaseBulletSpeed = 5;

    public float MaxPersueSpeed = 2.9f;
    public float MaxAlertSpeed = 2.7f;
    public float MaxPatrolSpeed = 2.5f;
    public float MaxFovAngleStrong = 230;
    public float MaxDetectRangeStrong = 10f;
    public float MaxBulletDamage = 50;
    public float MaxBulletSpeed = 20;

    public const float SCALE_FACTOR = 1.1f;

    void Start()
    {
        InitialiseStats();
    }

    public override void InitialiseStats()
    {
        ExpOnKill = 5 * SCALE_FACTOR * PlayerStatistics.Stage;

        AlwaysFire = true;
        
        MeleeDamage = 50;
        FireRatePerSecond = 10f;
        FireRate = 1 / FireRatePerSecond;
        DetectRangeWeak = 2.5f;
        FovAngleWeak = 40;

        var scaleFactorByLevel = (float)(Math.Pow(SCALE_FACTOR, PlayerStatistics.Stage));

        var bulletDamage = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseBulletDamage : BaseBulletDamage;        
        BulletDamage = bulletDamage > MaxBulletDamage ? MaxBulletDamage : bulletDamage;

        var bulletSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseBulletSpeed : BaseBulletSpeed;
        BulletSpeed = bulletSpeed > MaxBulletSpeed ? MaxBulletSpeed : bulletSpeed;

        PursueSpeed = 0;

        var alertSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseAlertSpeed : BaseAlertSpeed;
        AlertSpeed = alertSpeed > MaxAlertSpeed ? MaxAlertSpeed : alertSpeed;

        var patrolSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BasePatrolSpeed : BasePatrolSpeed;
        PatrolSpeed = patrolSpeed > MaxPatrolSpeed ? MaxPatrolSpeed : patrolSpeed;

        var fovAngle = (PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseFovAngleStrong : BaseFovAngleStrong);
        FovAngleStrong = fovAngle > MaxFovAngleStrong ? MaxFovAngleStrong : fovAngle;

        var detectRange = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseDetectRangeStrong : BaseDetectRangeStrong;
        DetectRangeStrong = detectRange > MaxDetectRangeStrong ? MaxDetectRangeStrong : detectRange;

        RaycastOffset = 0;
        PatrolRange = 5;
        AlertPhaseDuration = 60;
        AlertRate = 10;
        AlertRange = 0;
        CamaraRotateRate = 0;
        Speed = 1f;
        FireCooldown = 0;
        AlertPhaseCountdown = 0;
        AlertCounter = 0;
        TrackingTime = 5;
    }
}
