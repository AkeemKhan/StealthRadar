using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeButton : MonoBehaviour
{
    public PlayerController PlayerController;
    public MeleeButtonHandler MeleeButtonHandler;
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

            PlayerController = PlayerObject.GetComponent<PlayerController>();
            MeleeButtonHandler = transform.GetComponent<MeleeButtonHandler>();

            MeleeButtonHandler.PlayerObject = PlayerObject;
            MeleeButtonHandler.PlayerController = PlayerController;
        }
    }
}
