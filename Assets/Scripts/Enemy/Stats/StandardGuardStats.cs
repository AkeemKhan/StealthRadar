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
            PersueSpeed = 4;
            AlertSpeed = 3;
            PatrolSpeed = 1;
            FovAngleStrong = 90;
            FovAngleWeak = 30;
            DetectRange = 2;
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
}
