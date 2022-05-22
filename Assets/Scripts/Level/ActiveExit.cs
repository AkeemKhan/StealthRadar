using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveExit : MonoBehaviour
{
    public bool IsExit = false;

	void Start ()
    {
        transform.GetComponent<SpriteRenderer>().enabled = false;
    }
	
	void Update ()
    {
		if (IsExit)
            transform.GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == EntityConstants.PLAYER_TAG)
        {
            if (IsExit)
            {                
                PlayerStatistics.ClearFloor();
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    public void SetAsExit()
    {
        Debug.Log("Exit set ");
        IsExit = true;
        transform.GetComponent<SpriteRenderer>().enabled = true;
    }
}
