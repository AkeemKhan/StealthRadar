using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    public PlayerMovement PlayerMovement;
   
	// Use this for initialization
	void Start ()
    {
        InitialisePlayer();
    }
	
	// Update is called once per frame
	void Update ()
    {       

    }

    public void InitialisePlayer()
    {
        PlayerStatistics.ResetStats();
        PlayerObject = gameObject;
    }
}
