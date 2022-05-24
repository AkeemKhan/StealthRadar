using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyStats : MonoBehaviour
{
    public float PursueSpeed;
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

    public bool AlwaysFire;
    public float FireRatePerSecond;
    public float MeleeDamage;
    public float BulletSpeed;
    public float BulletDamage;

    void Start()
    {
        InitialiseStats();
    }

    public virtual void InitialiseStats()
    {
    }
}

public interface IStats
{

}
