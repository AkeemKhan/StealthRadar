using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Enemy.Stats
{
    public sealed class StandardGuardStats : EnemyStats, IStats
    {
        void Start()
        {
            InitialiseStats();
        }

        public override void InitialiseStats()
        {
            PersueSpeed = 1.8f;
            AlertSpeed = 1.7f;
            PatrolSpeed = 1.5f;
            FovAngleStrong = 180;
            DetectRangeStrong = 10;

            //PersueSpeed = 1.8f * PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.1f, PlayerStatistics.Level)) : 1.8f;
            //AlertSpeed = 1.7f * PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.1f, PlayerStatistics.Level)) : 1.7f;
            //PatrolSpeed = 1.5f * PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.1f, PlayerStatistics.Level)) : 1.5f;
            //FovAngleStrong = 160 * PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.1f, PlayerStatistics.Level)) : 160;
            //DetectRangeStrong = 2 * PlayerStatistics.Level > 1 ? (float)(Math.Pow(1.1f, PlayerStatistics.Level)) : 2f;

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
