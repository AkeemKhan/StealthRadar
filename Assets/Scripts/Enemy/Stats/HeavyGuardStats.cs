using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemy.Stats
{
    internal class HeavyGuardStats : EnemyStats
    {
        public float BasePersueSpeed = 0f;
        public float BaseAlertSpeed = 0.85f;
        public float BasePatrolSpeed = 0.75f;
        public float BaseFovAngleStrong = 90;
        public float BaseDetectRangeStrong = 5f;

        public float MaxPersueSpeed = 0f;
        public float MaxAlertSpeed = 1.5f;
        public float MaxPatrolSpeed = 1.2f;
        public float MaxFovAngleStrong = 200;
        public float MaxDetectRangeStrong = 7f;

        void Start()
        {
            InitialiseStats();
        }

        public override void InitialiseStats()
        {
            AlwaysFire = true;

            MeleeDamage = 10;
            BulletSpeed = 20;
            BulletDamage = 25;
            PursueSpeed = 0;

            FireRatePerSecond = 10f;
            FireRate = 1 / FireRatePerSecond;

            var alertSpeed = PlayerStatistics.Stage > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Stage)) * BaseAlertSpeed : BaseAlertSpeed;
            AlertSpeed = alertSpeed > MaxAlertSpeed ? MaxAlertSpeed : alertSpeed;

            var patrolSpeed = PlayerStatistics.Stage > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Stage)) * BasePatrolSpeed : BasePatrolSpeed;
            PatrolSpeed = patrolSpeed > MaxPatrolSpeed ? MaxPatrolSpeed : patrolSpeed;

            var fovAngle = (PlayerStatistics.Stage > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Stage)) * BaseFovAngleStrong : BaseFovAngleStrong);
            FovAngleStrong = fovAngle > MaxFovAngleStrong ? MaxFovAngleStrong : fovAngle;

            var detectRange = PlayerStatistics.Stage > 1 ? (float)(Math.Pow(1.05f, PlayerStatistics.Stage)) * BaseDetectRangeStrong : BaseDetectRangeStrong;
            DetectRangeStrong = detectRange > MaxDetectRangeStrong ? MaxDetectRangeStrong : detectRange;

            FovAngleWeak = 60;
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
