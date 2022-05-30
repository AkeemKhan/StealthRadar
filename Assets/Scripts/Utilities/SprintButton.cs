using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SprintButton : MonoBehaviour
{
    public PlayerMovement PlayerMovement;
    public SprintButtonHandler SprintButtonHandler;
    public GameObject PlayerObject;

    // Update is called once per frame
    void Update()
    {
        if (PlayerObject == null)
        {
            PlayerObject = GameObject.FindGameObjectWithTag(EntityConstants.PLAYER_TAG);

            Debug.Log(PlayerObject?.name ?? "NOT FOUND");

            if (PlayerObject == null)
                return;

            PlayerMovement = PlayerObject.GetComponent<PlayerMovement>();
            SprintButtonHandler = transform.GetComponent<SprintButtonHandler>();

            SprintButtonHandler.PlayerObject = PlayerObject;
            SprintButtonHandler.PlayerMovement = PlayerMovement;
        }
    }

}
