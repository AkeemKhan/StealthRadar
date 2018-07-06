using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimeController : MonoBehaviour {
    
    // Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        InGameTimeTracker();
        InGameAlertTracker();
    }

    public void InGameTimeTracker()
    {
        if (PlayerStatistics.InGame)
        {
            PlayerStatistics.CurrentGameTime += Time.deltaTime;
        }
    }

    public void InGameAlertTracker()
    {
        if (PlayerStatistics.InAlertPhase)
        {
            PlayerStatistics.CurrentAlertTime -= Time.deltaTime;
            PlayerStatistics.TimeInAlert += Time.deltaTime;
        }
    }
}
