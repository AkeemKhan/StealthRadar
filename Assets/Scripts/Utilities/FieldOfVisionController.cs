using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfVisionController : MonoBehaviour {

    public GameObject Target;
    public EnemyStats EnemyStats;
    public Vector3 Direction;
    public bool IsInSight;

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
        Direction = Target.transform.position - transform.position;
        float angle = Vector3.Angle(Direction, transform.right);

        if (angle < EnemyStats.FovAngleStrong / 2)
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
