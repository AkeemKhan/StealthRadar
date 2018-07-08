using UnityEngine;
using System.Collections;
using Assets.Scripts.Utilities;

public class FollowObject : MonoBehaviour {

    public GameObject Target;

    void Start ()
    {
        Target = GameObject.FindGameObjectWithTag(EntityConstants.PLAYER_TAG);
    }
	
	void Update ()
    {
        if (Target == null)
            Target = GameObject.FindGameObjectWithTag(EntityConstants.PLAYER_TAG);
        else
            transform.position = Target.transform.position - new Vector3(0,0,1);
	}
}
