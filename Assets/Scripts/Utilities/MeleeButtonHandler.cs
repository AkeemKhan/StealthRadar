using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MeleeButtonHandler : EventTrigger, IPointerUpHandler, IPointerDownHandler
{
    public PlayerController PlayerController;
    public GameObject PlayerObject;

    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("ATTACKING IN BUTTON");
        Debug.Log(PlayerController);
        PlayerController?.Attack();
    }
}
