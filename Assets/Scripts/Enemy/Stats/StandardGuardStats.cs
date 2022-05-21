using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Enemy.Stats
{
    public sealed class StandardGuardStats : EnemyStats, IStats
    {
        public float BasePersueSpeed = 1.8f;
        public float BaseAlertSpeed = 1.7f;
        public float BasePatrolSpeed = 1.5f;
        public float BaseFovAngleStrong = 160;
        public float BaseDetectRangeStrong = 2f;

        public float MaxPersueSpeed = 2.5f;
        public float MaxAlertSpeed = 2f;
        public float MaxPatrolSpeed = 1.7f;
        public float MaxFovAngleStrong = 200;
        public float MaxDetectRangeStrong = 6f;


        void Start()
        {
            InitialiseStats();
        }

        public override void InitialiseStats()
        {
            var persueSpeed = PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Level)) * BasePersueSpeed : BasePersueSpeed;
            PersueSpeed = persueSpeed > MaxPersueSpeed ? MaxPersueSpeed : persueSpeed;

            var alertSpeed = PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Level)) * BaseAlertSpeed : BaseAlertSpeed;
            AlertSpeed = alertSpeed > MaxAlertSpeed ? MaxAlertSpeed : alertSpeed;

            var patrolSpeed = PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Level)) *  BasePatrolSpeed : BasePatrolSpeed;
            PatrolSpeed = patrolSpeed > MaxPatrolSpeed ? MaxPatrolSpeed : patrolSpeed;

            var fovAngle = (PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Level)) * BaseFovAngleStrong : BaseFovAngleStrong);            
            FovAngleStrong = fovAngle > MaxFovAngleStrong ? MaxFovAngleStrong : fovAngle;

            var detectRange = PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Level)) * BaseDetectRangeStrong : BaseDetectRangeStrong;
            DetectRangeStrong = detectRange > MaxDetectRangeStrong ? MaxDetectRangeStrong : detectRange;

            FovAngleWeak = 30;
            RaycastOffset = 0;
            PatrolRange = 5;
            AlertPhaseDuration = 60;
            FireCount = 4;
            FireRate = 0.4f;
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
