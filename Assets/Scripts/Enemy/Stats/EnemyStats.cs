using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStats : MonoBehaviour
{
    public float PersueSpeed;
    public float AlertSpeed;
    public float PatrolSpeed;
    public float FovAngleStrong;
    public float FovAngleWeak;
    public float DetectRangeStrong;
    public float DetectRangeWeak;
    public float RaycastOffset;
    public float PatrolRange;
    public float AlertPhaseDuration;
    public int FireCount;
    public float FireRate;
    public float AlertRate;
    public float AlertRange;
    public float CamaraRotateRate;
    public float Speed;
    public float FireCooldown;
    public float AlertPhaseCountdown;
    public float AlertCounter;
    public float TrackingTime;
    public float TrackingCountdown;

    void Start()
    {
        InitialiseStats();
    }

    public virtual void InitialiseStats()
    {
        PersueSpeed = 4;
        AlertSpeed = 3;
        PatrolSpeed = 1;
        FovAngleStrong = 90;
        FovAngleWeak = 30;
        DetectRangeStrong = 0;
        RaycastOffset = 0;
        PatrolRange = 5;
        AlertPhaseDuration = 60;
        FireCount = 4;
        FireRate = 0.4f;
        AlertRate = 10;
        AlertRange = 0;
        CamaraRotateRate = 0;
        Speed = 2;
        FireCooldown = 0;
        AlertPhaseCountdown = 0;
        AlertCounter = 0;
    }

}

public interface IStats
{

}
