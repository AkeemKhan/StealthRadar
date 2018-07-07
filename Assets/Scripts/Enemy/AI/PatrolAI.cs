using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : EnemyAI {

    public List<Vector3> PatrolRoute = new List<Vector3>();
    public GameObject GunOffset;
    private LayerMask _layerMask = 1;
    public bool IsInTargetSight;
    public bool NewPatrolLocation = true;

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
        InitialisePatrolRoute();
        EnemyState = EnemyState.Patrol;
    }

    public void InitialisePatrolRoute()
    {
        PatrolRoute = new List<Vector3>();
        PatrolRoute.Add(
                new Vector3(
                Random.Range(transform.position.x - EnemyStats.PatrolRange, transform.position.x + EnemyStats.PatrolRange),
                Random.Range(transform.position.y - EnemyStats.PatrolRange, transform.position.y + EnemyStats.PatrolRange),
                transform.position.z)
            );
        for (int i = 0; i < 3; i++)
        {
            Vector3 temp = PatrolRoute[i];
            PatrolRoute.Add(
                new Vector3(
                Random.Range(temp.x - EnemyStats.PatrolRange, temp.x + EnemyStats.PatrolRange),
                Random.Range(temp.y - EnemyStats.PatrolRange, temp.y + EnemyStats.PatrolRange),
                temp.z)
            );
        }
        TargetPosition = PatrolRoute[0];
    }

    public override void AIUpdate()
    {
        PlayerPosition = PlayerObject.transform.position;
        if (EnemyState != EnemyState.Disabled)
        {
            if (EnemyStats.FireCooldown < EnemyStats.FireRate)
                EnemyStats.FireCooldown += Time.deltaTime;

            if (EnemyStats.AlertPhaseCountdown > 0)
                EnemyStats.AlertPhaseCountdown -= Time.deltaTime;

            if (EnemyStats.AlertCounter <= 10)
                EnemyStats.AlertCounter += Time.captureFramerate;

            FieldOfVisionUpdate();
            AIStateUpdate();
        }
    }

    public override void AIStateUpdate()
    {
        // Check if in sight
        if (IsInTargetSight)
        {
            if (EnemyStats.FireCooldown >= EnemyStats.FireRate)
            {
                EnemyStats.FireCooldown = 0;
                //Instantiate(bullet, gunOffset.transform.position, transform.rotation);
            }
            EnemyState = EnemyState.Persue;
        }

        // AI Actions depending on state
        if (EnemyState == EnemyState.Persue) //Persue player if in line of sight
        {
            TargetPosition = PlayerPosition;
            MovementToPosition();
        }
        if (EnemyState == EnemyState.Patrol || EnemyState == EnemyState.Alert) //Persue player if in line of sight
        {
            EnemyStats.Speed = EnemyState == EnemyState.Patrol 
                ? EnemyStats.PatrolSpeed
                : EnemyStats.AlertSpeed;

            Patrol();            
        }

        MovementToPosition();
    }

    public override void FieldOfVisionUpdate()
    {
        if (FieldOfVisionController.IsInSight)
        {            
            RaycastHit2D hit = Physics2D.Raycast(GunOffset.transform.position, FieldOfVisionController.Direction, EnemyStats.DetectRangeStrong, _layerMask);

            if (hit)
            {
                Debug.Log("Hit");
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
                    Debug.Log("Hitting Wall");
                    IsInTargetSight = false;

                    if (EnemyStats.AlertPhaseCountdown <= 0)
                        EnemyState = EnemyState.Patrol;
                    else
                        EnemyState = EnemyState.Alert;
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
    }

    public void GetRotationPosition()
    {
        Vector3 vectorToTarget = TargetPosition - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.rotation = Quaternion.Slerp(transform.rotation, q, Time.deltaTime * speed);
        transform.rotation = q;
    }

    public void Patrol()
    {
        //IF reached patrol location, trigger new location
        if (transform.position == TargetPosition)
        {
            NewPatrolLocation = true;
        }

        //If new location, remove 1st, use new 1st
        //If 0, create new routes
        if (NewPatrolLocation)
        {
            if (PatrolRoute.Count == 0)
            {
                InitialisePatrolRoute();
                Debug.Log("New route");
            }
            else
            {
                PatrolRoute.Remove(PatrolRoute[0]);
            }
            NewPatrolLocation = false;
        }

        MovementToPosition();
    }    

    void OnCollisionEnter2D(Collision2D coll)
    {
        //if (coll.transform.tag == EntityConstants.WALL_TAG)
        //{
        //    Debug.Log("WALL COLLISION");
        //    PatrolRoute.Remove(PatrolRoute[0]);
        //}
    }

    void OnCollisionStay2D(Collision2D col)
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
