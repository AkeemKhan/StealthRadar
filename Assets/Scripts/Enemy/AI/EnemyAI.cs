using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IEnemyAI
{
    public GameObject PlayerObject;
    protected GameObject Target;
    public EnemyState EnemyState;

    [SerializeField]
    public EnemyStats EnemyStats;

    public Vector3 TargetPosition;
    //public Vector3 PlayerPosition;
    public FieldOfVisionController FieldOfVisionController;

    public Vector3 PlayerPosition;
    
    void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {       

    }

    public virtual void Initialise()
    {
        // Find Player
        PlayerObject = GameObject.FindGameObjectWithTag(EntityConstants.PLAYER_TAG);
        Target = PlayerObject;
        TargetPosition = Target.transform.position;
        FieldOfVisionController.Initialise(PlayerObject, EnemyStats);
    }

    public virtual void AIUpdate() { }

    public virtual void AIStateUpdate() { }

    public virtual void FieldOfVisionUpdate() { }

}

public enum EnemyState
{
    Patrol,
    Alert,
    Persue,
    Shoot,
    Track,
    Disabled
}

public interface IEnemyAI
{

}
