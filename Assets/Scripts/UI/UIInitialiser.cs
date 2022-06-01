using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInitialiser : MonoBehaviour
{

    public bool JoystickActive;
    public bool ButtonsActive;    
    void Start()
    {
        var js = GameObject.Find("Fixed Joystick"); ;
        var melee = GameObject.Find("Melee");
        var spr = GameObject.Find("Sprint");

        js?.SetActive(JoystickActive);
        melee?.SetActive(ButtonsActive);
        spr?.SetActive(ButtonsActive);

        //Debug.Log(Application.platform);
        //if (Application.platform != RuntimePlatform.Android)
        //{
        //    js.SetActive(false);
        //    melee.SetActive(false);
        //    spr.SetActive(false);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
