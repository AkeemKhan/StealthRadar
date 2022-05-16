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
    public bool NewPatrolLocation = true;

    // Tracking
    public GameObject trackingPrint;
    public List<Vector2> BackTracker = new List<Vector2>();
    public float trackRate = 0.05f;
    public int maxBackTrack = 50;
    public float backTrackCounter;
    public bool didFindplayer = false;

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
                

            FieldOfVisionUpdate();
            AIStateUpdate();
        }
    }

    public override void AIStateUpdate()
    {
        //if (EnableDebug)
        //{
        //    Debug.Log($"Enemy State it: {EnemyState.ToString()}");
        //    Debug.Log($"Route: {NavigationRoute.Count}");
        //}

        // Check if in sight
        //if (IsInTargetSight)
        //{
        //    if (EnemyStats.FireCooldown >= EnemyStats.FireRate)
        //    {
        //        EnemyStats.FireCooldown = 0;
        //        //Instantiate(bullet, gunOffset.transform.position, transform.rotation);
        //    }
        //    EnemyState = EnemyState.Persue;
        //}

        //// AI Actions depending on state
        //if (EnemyState == EnemyState.Persue) //Persue player if in line of sight
        //{
        //    TargetPosition = PlayerPosition;
        //    //MovementToPosition();
        //}

        if (EnemyState == EnemyState.Patrol || EnemyState == EnemyState.Alert) //Persue player if in line of sight
        {
            EnemyStats.Speed = EnemyState == EnemyState.Patrol 
                ? EnemyStats.PatrolSpeed
                : EnemyStats.AlertSpeed;

            //Patrol();            
            if (NavigationRoute.Count == 0)
            {
                if (EnableDebug)
                {
                    Debug.Log($"NONENOENONEONEONE");
                }

                InitialisePatrolRoute();
            }
        }

        //if (EnemyState == EnemyState.Track)
        //{
        //    if (EnemyStats.TrackingCountdown > 0)
        //    {
        //        EnemyStats.Speed = EnemyStats.PersueSpeed;
        //        TrackingUpdate();
        //        NavigateToRandomNode();
        //    }
        //    if (EnemyStats.TrackingCountdown <= 0)
        //    {
        //        Debug.Log("Tacking finished");
        //        EnemyState = EnemyState.Alert;
        //        Patrol();
        //        //MovementToPosition();                
        //    }
        //}

        //if (EnemyState == EnemyState.StayOnPath)
        //{
        //    if (NavigationRoute.Count == 0)
        //    {
        //        EnemyState = EnemyState.Patrol;
        //        NavigateToPlayer();
        //    }
        //}
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
                    Debug.DrawRay(GunOffset.transform.position, FieldOfVisionController.Direction, Color.blue);
                    IsInTargetSight = true;
                    EnemyStats.Speed = EnemyStats.PersueSpeed;
                    TargetPosition = PlayerPosition;
                    SetAlert();

                    // Initiate Alert Phase.
                    if (EnemyStats.AlertCounter >= EnemyStats.AlertRate)
                    {
                        SetAlertStatus();
                        EnemyStats.AlertCounter = 0;
                    }
                }

                if (hit.collider.tag == EntityConstants.WALL_TAG)
                {
                    if (IsInTargetSight)
                    {
                        // Initiate tracking
                        didFindplayer = true;
                        Debug.Log("Initiate Tracking");
                        IsInTargetSight = false;
                        NavigationRoute.Clear();
                        //EnemyState = EnemyState.Track;


                        // PATHFINDING
                        //var closestNode = FindClosestNode();
                        //var playerClosestNode = FindPlayerClosestNode();
                        //var path = LevelGeneration.Navigation.FindPath(LevelGeneration.NodeIdMap[closestNode]);
                        //var pathNodes = Path(LevelGeneration.NodeIdMap[closestNode], LevelGeneration.NodeIdMap[playerClosestNode]).Reverse();

                        //EnemyState = EnemyState.StayOnPath;
                        //PatrolRoute.AddRange(pathNodes.Select(p => LevelGeneration.IdNodeMap[p.node].transform.position));

                        //Debug.Log("Patrol route count: " +  PatrolRoute.Count);

                        //foreach (var item in PatrolRoute)
                        //{
                        //    Debug.Log(item);
                        //}

                        //EnemyStats.TrackingCountdown = EnemyStats.TrackingTime;

                        //IEnumerable<(double distance, int node)> Path(int start, int destination)
                        //{
                        //    yield return (path[destination].distance, destination);
                        //    for (int i = destination; i != start; i = path[i].prev)
                        //    {
                        //        yield return (path[path[i].prev].distance, path[i].prev);
                        //    }
                        //}

                        // PATHFINDING END
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
            Debug.Log(item.name);
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

        //if (EnemyState.Track == EnemyState)
        //{
        //    //Debug.Log("Moving - TargetPosition" + TargetPosition + " " + transform.position + " " 
        //    //    + "Is in pos - " + (TargetPosition == transform.position).ToString());

        //    if (TargetPosition == transform.position)
        //    {
        //        NavigationRoute.Remove(NavigationRoute[0]);

        //        TargetPosition = NavigationRoute[0];
        //        //Debug.Log("SAME!!!!!" + PatrolRoute.Count);
        //        //PatrolRoute.Remove(PatrolRoute[0]);
        //    }
        //}

        if (EnemyState == EnemyState.Patrol || EnemyState == EnemyState.Alert)
        {
            if (NavigationRoute.Count == 0)
            {                
                return;
            }
        }

        if (TargetPosition == transform.position)
        {
            if (NavigationRoute.Any())
            {
                TargetPosition = NavigationRoute[0];
                NavigationRoute.Remove(NavigationRoute[0]);
            }
        }

        if (NewPatrolLocation)
        {
            NewPatrolLocation = false;
            NavigateToRandomNode();
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            else
            {
                KilledByPlayer();
            }
        }

        if (coll.transform.tag == EntityConstants.ENEMY_TAG)
        {
            NewPatrolLocation = true;
        }
    }

    void OnCollisionExit()
    {
        Clipped = false;
    }

    public bool Clipped;

    void OnCollisionStay2D(Collision2D col)
    {
        if (EnemyState != EnemyState.Track)
        {
            if (EnemyState == EnemyState.Patrol)
            {
                NewPatrolLocation = true;
            }
            if (EnemyState == EnemyState.Alert)
            {
                NewPatrolLocation = true;
            }
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
    }
}
