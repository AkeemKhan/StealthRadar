using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Enemy.Stats
{
    public class StandardGuardStats : EnemyStats
    {
        public readonly float Speed;
        public readonly float PersueSpeed;
        public readonly float AlertSpeed;
        public readonly float PatrolSpeed;

        public readonly float FovAngleStrong;
        public readonly float FovAngleWeak;
        public readonly float DetectRange;
        public readonly float RaycastOffset;
        public readonly float PatrolRange;

        public readonly int FireCount;
        public readonly float FireCooldown;
        public readonly float FireRate;

        public readonly float AlertPhaseCountdown;
        public readonly float AlertRate;
        public readonly float AlertCounter;
        public readonly float AlertRange;
        public readonly float CamaraRotateRate;
    }
}
