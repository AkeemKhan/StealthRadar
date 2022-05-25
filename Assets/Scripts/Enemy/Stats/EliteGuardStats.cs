using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteGuardStats : EnemyStats
{
    public float BasePersueSpeed = 1.8f;
    public float BaseAlertSpeed = 1.7f;
    public float BasePatrolSpeed = 1.5f;
    public float BaseFovAngleStrong = 90;
    public float BaseDetectRangeStrong = 10f;

    public float MaxPersueSpeed = 2.5f;
    public float MaxAlertSpeed = 2f;
    public float MaxPatrolSpeed = 1.7f;
    public float MaxFovAngleStrong = 200;
    public float MaxDetectRangeStrong = 10f;

    void Start()
    {
        InitialiseStats();
    }

    public override void InitialiseStats()
    {
        ExpOnKill = 150;

        MeleeDamage = 80;
        BulletSpeed = 7;
        BulletDamage = 20;

        FireRatePerSecond = 4f;
        FireRate = 1 / FireRatePerSecond;
        DetectRangeWeak = MaxDetectRangeStrong;
        FovAngleWeak = 40;
        
        PursueSpeed = MaxPersueSpeed;
        AlertSpeed = MaxAlertSpeed;
        PatrolSpeed = MaxPatrolSpeed;
        FovAngleStrong = MaxFovAngleStrong;
        DetectRangeStrong = MaxDetectRangeStrong;
        
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
