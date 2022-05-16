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
            PersueSpeed = 1.5f;
            AlertSpeed = 1.5f;
            PatrolSpeed = 1f;
            FovAngleStrong = 180;
            FovAngleWeak = 30;
            DetectRangeStrong = 5;
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
