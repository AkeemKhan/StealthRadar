using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SprintButtonHandler : EventTrigger, IPointerUpHandler, IPointerDownHandler
{
    public PlayerMovement PlayerMovement;
    public GameObject PlayerObject;

    public override void OnPointerDown(PointerEventData eventData)
    {
        PlayerMovement.ButtonSprinting = true;
    }

    public override void OnPointerUp(PointerEventData data)
    {
        PlayerMovement.ButtonSprinting = false;
    }
}