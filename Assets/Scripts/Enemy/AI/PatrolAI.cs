using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PatrolAI : EnemyAI {
    
    public List<Vector3> NavRoute = new List<Vector3>();

    public GameObject GunOffset;
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
    }

    public void InitialisePatrolRoute()
    {
        //NavigationRoute = new List<Vector3>();
        //NavigationRoute.Add(
        //        new Vector3(
        //        Random.Range(transform.position.x - EnemyStats.PatrolRange, transform.position.x + EnemyStats.PatrolRange),
        //        Random.Range(transform.position.y - EnemyStats.PatrolRange, transform.position.y + EnemyStats.PatrolRange),
        //        transform.position.z)
        //    );
        //for (int i = 0; i < 3; i++)
        //{
        //    Vector3 temp = NavigationRoute[i];
        //    NavigationRoute.Add(
        //        new Vector3(
        //        Random.Range(temp.x - EnemyStats.PatrolRange, temp.x + EnemyStats.PatrolRange),
        //        Random.Range(temp.y - EnemyStats.PatrolRange, temp.y + EnemyStats.PatrolRange),
        //        temp.z)
        //    );
        //}

        NavigateToRandomNode();
        TargetPosition = NavigationRoute[0];
    }

    public override void AIUpdate()
    {
        if (EnemyState != EnemyState.Disabled)
        {
            PlayerPosition = PlayerObject.transform.position;
            if (EnemyStats.FireCooldown < EnemyStats.FireRate)
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
         
            if (NavigationRoute.Count == 0)
            {
                InitialisePatrolRoute();
            }
        }

        if (!(PreventMove > 0))
            MovementToPosition();
    }

    public override void FieldOfVisionUpdate()
    {
        if (FieldOfVisionController.IsInSight)
        {            
            RaycastHit2D hit = Physics2D.Raycast(GunOffset.transform.position, FieldOfVisionController.Direction, EnemyStats.DetectRangeStrong, _layerMask);

            if (hit)
            {
                if (hit.collider.tag == EntityConstants.PLAYER_TAG)
                {
                    MovementTarget = MovementTarget.Player;

                    Debug.DrawRay(GunOffset.transform.position, FieldOfVisionController.Direction, Color.blue);
                    IsInTargetSight = true;

                    if (!DetectedPlayer)
                    {
                        DetectedPlayer = true;
                        PlayerStatistics.Detections++;
                    }

                    EnemyStats.Speed = EnemyStats.PersueSpeed;
                    TargetPosition = PlayerPosition;
                    SetAlert();

                    // Initiate Alert Phase
                    if (EnemyStats.AlertCounter >= EnemyStats.AlertRate)
                    {
                        SetAlertStatus();
                        EnemyStats.AlertCounter = 0;
                    }
                }

                if (hit.collider.tag == EntityConstants.WALL_TAG)
                {
                    MovementTarget = MovementTarget.Player;

                    if (IsInTargetSight)
                    {
                        // Initiate tracking
                        didFindplayer = true;
                        Debug.Log("Initiate Tracking");
                        IsInTargetSight = false;
                        NewMovementLocation = true;
                    }

                    if (EnemyState != EnemyState.Track && EnemyStats.AlertPhaseCountdown <= 0)
                        EnemyState = EnemyState.Patrol;
                }
            }
        }
        else
        {
            EnemyStats.Speed = EnemyStats.PatrolSpeed;
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
        }
    }

    public void MovementToPosition()
    {
        GetRotationPosition();
        transform.position = Vector2.MoveTowards(transform.position, TargetPosition, EnemyStats.Speed * Time.deltaTime);

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

            Debug.Log("NEW LOCATION");

            // Bug - AI gets stuck, just make them OP when stuck
            if (RequestNewLocationCount > 5)
            {
                PreventMove = 100;
                EnemyStats.FovAngleStrong = 360;
            }

            if (EnemyState  == EnemyState.Alert || EnemyState == EnemyState.Persue)
            {
                NavigateToPlayer();
            }
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
        //PatrolRoute.Clear();
        //foreach (var pos in path)
        //{
        //    PatrolRoute.Add(pos);
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
            Debug.Log("WALL COLLISION");
            if (FieldOfVisionController.IsInSight && EnemyState != EnemyState.Disabled)
            {
                PreventMove = 1;
                PlayerStatistics.Health -= 50;

                if (PlayerStatistics.Health <= 0)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                }
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

        if (EnemyState == EnemyState.Alert || EnemyState == EnemyState.Persue)
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
        if (EnemyState == EnemyState.Track && EnemyStats.TrackingCountdown >= 0)
        {
            //Debug.Log("--- Tracking -- " + PlayerObject.transform.position);
            //backTrackCounter += Time.deltaTime;
            //if (backTrackCounter >= trackRate)
            //{
            if (transform.position == TargetPosition)
            {
                Debug.Log("Same Position in tracking update");
            }

            NavigationRoute.Add(PlayerObject.transform.position);
            Instantiate(trackingPrint, new Vector3(PlayerPosition.x, PlayerPosition.y), Quaternion.identity);
            //if (Clipped) Patrol();
                //if (PatrolRoute.Count > maxBackTrack)
                //{
                //    PatrolRoute.RemoveAt(0);
                //}

            //    backTrackCounter = 0;
            //}
        }                
    }

    public void KilledByPlayer()
    {
        EnemyState = EnemyState.Disabled;
        transform.GetComponent<SpriteRenderer>().enabled = false;
        gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<SpriteRenderer>().renderingLayerMask = 0;
        gameObject.GetComponent<Collider2D>().enabled = false;        
        Destroy(gameObject.GetComponent<Rigidbody2D>());

        PlayerStatistics.IncreaseExp(10 + Random.Range(-10, 10));
        PlayerStatistics.EnemiesKilledThisRound++;

        if (PlayerStatistics.Detections == 0)
            PlayerStatistics.CurrentStealthStreak++;
    }
}
