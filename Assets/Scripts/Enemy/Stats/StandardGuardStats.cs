using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Enemy.Stats
{
    public sealed class StandardGuardStats : EnemyStats, IStats
    {
        public float BasePersueSpeed = 1.6f;
        public float BaseAlertSpeed = 1.5f;
        public float BasePatrolSpeed = 1.4f;
        public float BaseFovAngleStrong = 90;
        public float BaseDetectRangeStrong = 3f;        

        public float MaxPersueSpeed = 2.5f;
        public float MaxAlertSpeed = 2f;
        public float MaxPatrolSpeed = 1.7f;
        public float MaxFovAngleStrong = 200;
        public float MaxDetectRangeStrong = 6f;

        public const float SCALE_FACTOR = 1.005f;

        void Start()
        {
            InitialiseStats();
        }

        public override void InitialiseStats()
        {
            ExpOnKill = 10;

            MeleeDamage = 50;
            BulletSpeed = 10;
            BulletDamage = 10;
            FireRatePerSecond = 4f;
            FireRate = 1 / FireRatePerSecond;
            DetectRangeWeak = 2.5f;
            FovAngleWeak = 40;          

            var scaleFactorByLevel = (float)(Math.Pow(SCALE_FACTOR, PlayerStatistics.Stage));

            var persueSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BasePersueSpeed : BasePersueSpeed;
            PursueSpeed = persueSpeed > MaxPersueSpeed ? MaxPersueSpeed : persueSpeed;

            var alertSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel * BaseAlertSpeed : BaseAlertSpeed;
            AlertSpeed = alertSpeed > MaxAlertSpeed ? MaxAlertSpeed : alertSpeed;

            var patrolSpeed = PlayerStatistics.Stage > 1 ? scaleFactorByLevel *  BasePatrolSpeed : BasePatrolSpeed;
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
}
