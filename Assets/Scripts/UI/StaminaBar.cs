using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateStaminaBar();
    }

    public void UpdateStaminaBar()
    {
        var slider = GetComponent<Slider>();
        slider.maxValue = PlayerStatistics.MaxStamina;
        slider.value = PlayerStatistics.Stamina;
    }
}
