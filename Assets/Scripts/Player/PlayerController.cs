using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject PlayerObject;
    public PlayerMovement PlayerMovement;

    public int PlayerLevel;
    public int PlayerExp;
    public int Detections;
    public float Speed;

    // Use this for initialization
    void Start ()
    {
        InitialisePlayer();
    }
	
	// Update is called once per frame
	void Update ()
    {
        PlayerLevel = PlayerStatistics.PlayerLevel;
        PlayerExp = PlayerStatistics.PlayerExp;
        Detections = PlayerStatistics.Detections;
        Speed = PlayerStatistics.Speed;
    }

    public void InitialisePlayer()
    {
        PlayerStatistics.ResetStats();
        PlayerObject = gameObject;
    }
}
