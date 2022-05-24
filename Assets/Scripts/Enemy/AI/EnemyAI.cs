using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IEnemyAI
{
    public GameObject PlayerObject;
    public PlayerMovement PlayerMovement => PlayerObject.GetComponent<PlayerMovement>();

    protected GameObject Target;
    public EnemyState EnemyState;
    public bool EnableDebug = false;

    public GameObject ClosestNode;
    public GameObject PlayerClosestNode;

    [SerializeField]
    public EnemyStats EnemyStats;

    public Vector3 TargetPosition;
    //public Vector3 PlayerPosition;
    public FieldOfVisionController FieldOfVisionController;

    public Vector3 PlayerPosition;

    public List<Vector3> NavigationRoute = new List<Vector3>();
    public List<Vector3> NavigationRouteHistory = new List<Vector3>();
    public List<GameObject> NavigationRouteGameObjects = new List<GameObject>();

    public MovementTarget MovementTarget;

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

    public void NavigateToRandomNode()
    {
        var closestNode = FindClosestNode();

        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PFNode");

        var randomNumber = Random.Range(0, gos.Length - 1);
        var randomNode = gos[randomNumber];

        int startId = LevelGeneration.VectorNodeMap[closestNode.transform.position].Id;
        int destId = LevelGeneration.VectorNodeMap[randomNode.transform.position].Id;

        var path = LevelGeneration.Navigation.FindPath(startId, destId);
        //NavigationRouteGameObjects = path.Select(p => LevelGeneration.IdNodeMapGO[p]).ToList();
        NavigationRoute = path.Select(p => LevelGeneration.NodeVectorMap[LevelGeneration.IdNodeMap[p]]).ToList();
    }

    public void NavigateToPlayer()
    {
        var closestNode = FindClosestNode();
        var playerClosestNode = FindPlayerClosestNode();

        int startId = LevelGeneration.VectorNodeMap[closestNode.transform.position].Id;
        int destId = LevelGeneration.VectorNodeMap[playerClosestNode.transform.position].Id;

        var path = LevelGeneration.Navigation.FindPath(startId, destId);

        NavigationRoute = path.Select(p => LevelGeneration.NodeVectorMap[LevelGeneration.IdNodeMap[p]]).ToList();
        NavigationRoute.Add(PlayerPosition);
    }

    public GameObject FindClosestNode(List<GameObject> ignore = null)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PFNode");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            if (ignore != null && ignore.Contains(go))
            {
                Debug.Log("IGNORING");
                continue;
            }

            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                var hit = Physics2D.Linecast(this.transform.position, go.transform.position);
                
                if (hit.collider.tag != EntityConstants.WALL_TAG)
                {
                    closest = go;
                    distance = curDistance;
                }

            }
        }
        ClosestNode = closest;
        return closest;
    }

    public GameObject FindPlayerClosestNode()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("PFNode");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = PlayerPosition;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                var hit = Physics2D.Linecast(this.transform.position, go.transform.position);

                if (hit.collider.tag != EntityConstants.WALL_TAG)
                {
                    closest = go;
                    distance = curDistance;
                }
            }
        }

        PlayerClosestNode = closest;
        return closest;
    }

    public virtual void AIUpdate() { }

    public virtual void AIStateUpdate() { }

    public virtual void FieldOfVisionUpdate() { }
}

public enum EnemyState
{
    Patrol,
    Alert,
    Pursue,
    Shoot,
    Track,
    StayOnPath,
    Disabled
}

public enum MovementTarget
{
    Player,
    Location
}

public interface IEnemyAI
{

}
