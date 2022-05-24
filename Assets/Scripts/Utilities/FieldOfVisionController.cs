using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVisionController : MonoBehaviour {

    public GameObject Target;
    public EnemyStats EnemyStats;
    public Vector3 Direction;
    public bool IsInSight;

    public PlayerMovement PlayerMovement;

    public void Initialise(GameObject target, EnemyStats stats)
    {
        Target = target;
        EnemyStats = stats;
    }
	
	void Start ()
    {
		
	}
	
	void Update ()
    {
        if (Target != null)
        {
            FieldOfVision();
        }
    }

    public void FieldOfVision()
    {
        if (PlayerMovement == null)
        {
            PlayerMovement = Target.GetComponent<PlayerMovement>();
        }

        Direction = Target.transform.position - transform.position;
        float angle = Vector3.Angle(Direction, transform.right);

        var useFov = PlayerMovement.IsMoving ? EnemyStats.FovAngleStrong : EnemyStats.FovAngleWeak;

        if (angle < useFov / 2)
        {
            if (!IsInSight)
            {
                IsInSight = true;
            }
        }
        else
        {
            if (IsInSight)
            {
                IsInSight = false;
            }            
        }
    }
}
