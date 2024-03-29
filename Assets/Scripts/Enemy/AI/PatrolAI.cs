﻿using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatrolAI : EnemyAI {

    public List<Vector3> NavRoute = new List<Vector3>();
    
    private LayerMask _layerMask = 1;
    public bool IsInTargetSight;
    public bool NewMovementLocation = true;

    // Tracking
    public GameObject trackingPrint;
    public List<Vector2> BackTracker = new List<Vector2>();
    public float trackRate = 0.05f;
    public int maxBackTrack = 50;
    public float backTrackCounter;
    public bool didFindplayer = false;

    public List<GameObject> LastCollisionNodes = new List<GameObject>();
    public float CollisionCleanupRate = 0f;

    public float PreventMove = 0f;
    public int RequestNewLocationCount = 0;
    public float RequestNewLocationRefresh = 0;

    public bool DetectedPlayer = false;
    public bool DumbPatrol = false;
    public bool CanFire;
    public bool HuntMode => DetectedPlayer && CanFire;
    public float DefaultFov;
    public float AlertDetectRange;

    public float DetectionCounter;
    public float DetectionThreshold = 1f;

    public GameObject GunOffset;
    public GameObject AmmoType;
    public GameObject Gun;
    public GameObject Marker;

    void Start ()
    {
        Initialise();
    }
		
	void Update ()
    {
        AIUpdate();
    }

    public override void Initialise()
    {
        base.Initialise();
        EnemyState = EnemyState.Patrol;

        InitialisePatrolRoute();

        EnemyStats.InitialiseStats();
        DetectionThreshold = EnemyStats.DetectionThreshold;
        DefaultFov = EnemyStats.FovAngleStrong;
        AlertDetectRange = 10f;

        GunOffset = transform.GetChild(0).gameObject;

        CanFire = Random.Range(0, 100) < PlayerStatistics.DifficultyModifier || EnemyStats.AlwaysFire;
        
        if (Gun != null)
        {
            Gun.GetComponent<SpriteRenderer>().enabled = CanFire;
        }
    }

    public void InitialisePatrolRoute()
    {
        //if (DumbPatrol)
        //{
        //    NavigationRoute = new List<Vector3>();
        //    NavigationRoute.Add(
        //            new Vector3(
        //            Random.Range(transform.position.x - EnemyStats.PatrolRange, transform.position.x + EnemyStats.PatrolRange),
        //            Random.Range(transform.position.y - EnemyStats.PatrolRange, transform.position.y + EnemyStats.PatrolRange),
        //            transform.position.z)
        //        );
        //    for (int i = 0; i < 3; i++)
        //    {
        //        Vector3 temp = NavigationRoute[i];
        //        NavigationRoute.Add(
        //            new Vector3(
        //            Random.Range(temp.x - EnemyStats.PatrolRange, temp.x + EnemyStats.PatrolRange),
        //            Random.Range(temp.y - EnemyStats.PatrolRange, temp.y + EnemyStats.PatrolRange),
        //            temp.z)
        //        );
        //    }
        //}
        //else
        //{
            NavigateToRandomNode();
        //}

        TargetPosition = NavigationRoute[0];
    }

    public override void AIUpdate()
    {
        if (EnemyState != EnemyState.Disabled)
        {
            PlayerPosition = PlayerObject.transform.position;
            if (EnemyStats.FireCooldown <= EnemyStats.FireRate)
                EnemyStats.FireCooldown += Time.deltaTime;

            if (EnemyStats.AlertPhaseCountdown > 0)
                EnemyStats.AlertPhaseCountdown -= Time.deltaTime;

            if (EnemyStats.AlertCounter <= 10)
                EnemyStats.AlertCounter += Time.captureFramerate;

            if (EnemyState == EnemyState.Track)
                EnemyStats.TrackingCountdown -= Time.deltaTime;

            if (CollisionCleanupRate < 1)
                CollisionCleanupRate += Time.deltaTime;

            if (PreventMove >= 0)
                PreventMove -= Time.deltaTime;
            else
                RequestNewLocationCount = 0;

            if (RequestNewLocationRefresh >= 0)
                RequestNewLocationRefresh -= Time.deltaTime;
            else
            {
                RequestNewLocationRefresh = 5;
                RequestNewLocationCount = 0;
            }

            FieldOfVisionUpdate();
            AIStateUpdate();
        }
    }
    
    public override void AIStateUpdate()
    {
        if (EnemyState == EnemyState.Patrol || EnemyState == EnemyState.Alert) //Persue player if in line of sight
        {
            EnemyStats.Speed = EnemyState == EnemyState.Patrol 
                ? EnemyStats.PatrolSpeed
                : EnemyStats.AlertSpeed;
         
            if (EnemyState == EnemyState.Alert && EnemyStats.AlertPhaseCountdown <= 0)
            {
                Debug.Log("Ended Alert");
                EnemyState = EnemyState.Patrol;
                NewMovementLocation = true;
            }

            if (NavigationRoute.Count == 0)
            {
                NewMovementLocation = true;
            }
        }

        if (EnemyState == EnemyState.Pursue)
        {
            EnemyStats.Speed = EnemyStats.PursueSpeed;

            if (NavigationRoute.Count == 0)
            {
                NewMovementLocation = true;
            }
        }

        //if (EnemyState == EnemyState.Track)
        //{            
        //    if (EnemyStats.TrackingCountdown > 0)
        //    {
        //        EnemyStats.Speed = EnemyStats.PursueSpeed;
        //        TrackingUpdate();
        //    }
        //    if (EnemyStats.TrackingCountdown <= 0)
        //    {
        //        Debug.Log("Tacking finished");
        //        Debug.Log("Changed to ALERT");
        //        EnemyState = EnemyState.Alert;

        //        Debug.Log("NOW LOCATION 1");
        //        NewMovementLocation = true;
        //        //MovementToPosition();
        //    }
        //}

        if (!(PreventMove > 0))
            MovementToPosition();
    }

    public override void FieldOfVisionUpdate()
    {        
        if (EnemyState == EnemyState.Alert)
        {
            EnemyStats.FovAngleStrong = 300;
        }
        else
        {
            EnemyStats.FovAngleStrong = DefaultFov;
        }

        if (FieldOfVisionController.IsInSight)
        {
            var useDetectRange = EnemyState == EnemyState.Patrol 
                ? PlayerMovement.IsMoving ? EnemyStats.DetectRangeStrong 
                : EnemyStats.DetectRangeWeak : EnemyStats.DetectRangeStrong;

            RaycastHit2D hit = Physics2D.Raycast(GunOffset.transform.position, FieldOfVisionController.Direction, useDetectRange, _layerMask);

            if (hit)
            {
                if (hit.collider.tag == EntityConstants.PLAYER_TAG)
                {
                    if (Vector2.Distance(PlayerPosition, transform.position) < 2)
                    {
                        DetectionCounter += Time.deltaTime * 10;
                    }
                    else
                    {
                        DetectionCounter += Time.deltaTime;
                    }
                    DetectionCounter = DetectionCounter > DetectionThreshold ? DetectionThreshold : DetectionCounter;

                    if (DetectionCounter >= DetectionThreshold || DetectedPlayer)
                    {
                        EnemyState = EnemyState.Pursue;

                        IsInTargetSight = true;

                        RangeWeaponHandler();

                        if (!DetectedPlayer)
                        {
                            DetectedPlayer = true;
                            DetectionCounter = 0;
                            EnemyStats.DetectRangeStrong = CanFire ? AlertDetectRange : EnemyStats.DetectRangeStrong;
                            PlayerStatistics.Detections++;
                        }

                        EnemyStats.Speed = EnemyStats.PursueSpeed;
                        TargetPosition = PlayerPosition;
                        // SetAlert();

                        // Initiate Alert Phase
                        if (EnemyStats.AlertCounter >= EnemyStats.AlertRate)
                        {
                            SetAlertStatus();
                            EnemyStats.AlertCounter = 0;
                        }
                    }
                }

                if (hit.collider.tag == EntityConstants.WALL_TAG)
                {
                    MovementTarget = MovementTarget.Player;

                    if (IsInTargetSight)
                    {
                        // Initiate tracking
                        didFindplayer = true;
                        //Debug.Log("Initiate Tracking");
                        NavigationRoute.Clear();
                        IsInTargetSight = false;
                        //EnemyState = EnemyState.Track;
                        EnemyState = EnemyState.Alert;
                        //EnemyStats.TrackingCountdown = EnemyStats.TrackingTime;
                        Debug.Log("Started Alert");
                        EnemyStats.AlertPhaseCountdown = EnemyStats.AlertPhaseDuration;
                    }

                    //if (EnemyState != EnemyState.Track && EnemyStats.AlertPhaseCountdown <= 0)
                    //{
                    //    Debug.Log("Changed to PATROL FROM TRACK");
                    //    EnemyState = EnemyState.Patrol;
                    //}
                }
            }
        }
        else
        {
            if (DetectionCounter > 0)
                DetectionCounter -= Time.deltaTime;

            EnemyStats.Speed = EnemyStats.PatrolSpeed;
        }
    }

    public virtual void RangeWeaponHandler()
    {
        if (CanFire && PreventMove <= 0)
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
                var script = bullet.GetComponent<Bullet>();
                script.TargetPosition = bulletTargetRecoil;
                script.Damage = EnemyStats.BulletDamage;
                Debug.Log($"Script DAM : {script.Damage}");
                Debug.Log($"EnemyStats DAM : {EnemyStats.BulletDamage}");
                script.Speed = EnemyStats.BulletSpeed;

                // Rotate to recoil spot
                Vector3 vectorToTarget = bulletTargetRecoil - new Vector2(bullet.transform.position.x, bullet.transform.position.y);
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                bullet.transform.rotation = q;

            }
        }
    }

    public void SetAlertStatus()
    {
        SetAlert();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, EnemyStats.DetectRangeStrong);

        foreach (Collider2D item in hitColliders)
        {
            if (item.tag == EntityConstants.ENEMY_TAG)
            {
                item.gameObject.GetComponent<PatrolAI>().SetAlert();
            }
        }
    }

    public void SetAlert()
    {
        if (EnemyState != EnemyState.Disabled)
        {
            EnemyState = EnemyState.Alert;
            EnemyStats.AlertPhaseCountdown = EnemyStats.AlertPhaseDuration;
            NewMovementLocation = true;
        }
    }

    public void MovementToPosition()
    {
        GetRotationPosition();

        if (EnemyStats.Speed > 0)
            transform.position = Vector2.MoveTowards(transform.position, TargetPosition, EnemyStats.Speed * Time.deltaTime);

        if (EnemyState.Track == EnemyState)
        {
            //Debug.Log("Moving - TargetPosition" + TargetPosition + " " + transform.position + " " 
            //    + "Is in pos - " + (TargetPosition == transform.position).ToString());

            if (TargetPosition == transform.position)
            {
                NavigationRoute.Remove(NavigationRoute[0]);

                TargetPosition = NavigationRoute[0];
                //Debug.Log("SAME!!!!!" + PatrolRoute.Count);
                //PatrolRoute.Remove(PatrolRoute[0]);
            }
        }

        if (EnemyState == EnemyState.Patrol || EnemyState == EnemyState.Alert)
        {
            if (NavigationRoute.Count == 0)
            {
                NewMovementLocation = true;
            }
        }

        if (TargetPosition == transform.position)
        {
            if (NavigationRoute.Any())
            {
                TargetPosition = NavigationRoute[0];
                NavigationRouteHistory.Insert(0, TargetPosition);
                NavigationRoute.Remove(NavigationRoute[0]);
            }
        }

        if (NewMovementLocation)
        {
            NewMovementLocation = false;
            NavigationRoute.Clear();


            // Bug - AI gets stuck, just make them OP when stuck
            if (RequestNewLocationCount > 5)
            {
                PreventMove = 100;
                EnemyStats.FovAngleStrong = 360;
            }

            if (EnemyState  == EnemyState.Alert || EnemyState == EnemyState.Pursue)
            {
                NavigateToPlayer();
            }
            //else if (DumbPatrol)
            //{
            //    InitialisePatrolRoute();
            //}
            else
            {
                NavigateToRandomNode();
            }
        }
    }

    public void GetRotationPosition()
    {
        if (TargetPosition == null)
            return;

        Vector3 vectorToTarget = TargetPosition - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
        transform.rotation = q;
       
    }

    public bool tmpb;

    public void FollowTrackingPath()
    {
        //var path = PlayerObject.GetComponent<PlayerController>().BackTracker;
        //NavigationRoute.Clear();
        //foreach (var pos in path)
        //{
        //    NavigationRoute.Add(pos);
        //}
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.transform.tag == EntityConstants.WALL_TAG)
        {
            Clipped = true;
        }

        if (coll.transform.tag == EntityConstants.PLAYER_TAG)
        {
            DetectedPlayer = true;

            if (FieldOfVisionController.IsInSight && EnemyState != EnemyState.Disabled)
            {
                PreventMove = 1;

                if (PreventMove <= 0)
                    PlayerStatistics.DamagePlayer(EnemyStats.MeleeDamage);
            }
            else
            {
                KilledByPlayer();
            }
        }

        if (coll.transform.tag == EntityConstants.ENEMY_TAG)
        {
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), coll.collider);
        }
    }

    void OnCollisionExit()
    {
        Clipped = false;
    }

    public bool Clipped;

    void OnCollisionStay2D(Collision2D col)
    {
        //Debug.Log("COLLIDING LOTS HERERERERE");

        if (CollisionCleanupRate < 1)
            return;

        CollisionCleanupRate = 0;

        NewMovementLocation = true;

        var closestNode = FindClosestNode();
        //LastCollisionNodes.Add(closestNode);

        TargetPosition = closestNode?.transform.position ?? TargetPosition;

        //NavigationRoute.Clear();

        //foreach (var item in NavigationRouteHistory)
        //{
        //    NavigationRoute.Add(item);
        //}

        if (EnemyState == EnemyState.Alert || EnemyState == EnemyState.Pursue)
        {
            NavigateToPlayer();
        }
        else
        {
            NavigateToRandomNode();
        }

    }

    public void TrackingUpdate()
    {
        Debug.Log("TRACKING");
        if (EnemyState == EnemyState.Track && EnemyStats.TrackingCountdown >= 0)
        {
            //Debug.Log("--- Tracking -- " + PlayerObject.transform.position);
            //backTrackCounter += Time.deltaTime;
            //if (backTrackCounter >= trackRate)
            //{

            EnemyStats.Speed = EnemyStats.PursueSpeed;
            if (transform.position == TargetPosition)
            {
                Debug.Log("Same Position in tracking update");
            }

            NavigationRoute.Add(PlayerObject.transform.position);
            // Instantiate(trackingPrint, new Vector3(PlayerPosition.x, PlayerPosition.y), Quaternion.identity);
            //if (Clipped) Patrol();
                //if (PatrolRoute.Count > maxBackTrack)
                //{
                //    PatrolRoute.RemoveAt(0);
                //}

            //    backTrackCounter = 0;
            //}
        }                
    }    
}
