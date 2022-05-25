using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteAI : PatrolAI
{
    void Start()
    {
        Initialise();
    }

    // Update is called once per frame
    void Update()
    {
        AIUpdate();
    }

    public override void Initialise()
    {
        EnemyStats.InitialiseStats();

        PlayerObject = GameObject.FindGameObjectWithTag(EntityConstants.PLAYER_TAG);
        Target = PlayerObject;
        TargetPosition = Target.transform.position;
        FieldOfVisionController.Initialise(PlayerObject, EnemyStats);

        EnemyState = EnemyState.Patrol;

        InitialisePatrolRoute();

        DefaultFov = EnemyStats.FovAngleStrong;
        AlertDetectRange = 10f;

        GunOffset = transform.GetChild(0).gameObject;

        CanFire = true; Debug.Log("CAN FIRE");

        if (Gun != null)
        {
            Gun.GetComponent<SpriteRenderer>().enabled = CanFire;
        }
    }

    public override void RangeWeaponHandler()
    {
        if (CanFire)
        {
            if (EnemyStats.FireCooldown >= EnemyStats.FireRate)
            {
                EnemyStats.FireCooldown = 0;
                
                // Get Recoil position
                var ray = new Ray2D(transform.position, FieldOfVisionController.Direction);
                var bulletTarget = ray.GetPoint(10);
                var bulletTargetRecoil = new Vector2(bulletTarget.x + Random.Range(-1f, 1f), bulletTarget.y + Random.Range(-1f, 1f));

                // Create bullet initialise stats
                var bullet = Instantiate(AmmoType, GunOffset.transform.position, transform.rotation);
                var script = bullet.GetComponent<Plasma>();
                script.TargetPosition = bulletTargetRecoil;
                script.Player = PlayerObject;
                script.Damage = EnemyStats.BulletDamage;
                script.Speed = Random.Range(1, EnemyStats.BulletSpeed);

                // Rotate to recoil spot
                Vector3 vectorToTarget = bulletTargetRecoil - new Vector2(bullet.transform.position.x, bullet.transform.position.y);
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                bullet.transform.rotation = q;
            }
        }
    }
}
