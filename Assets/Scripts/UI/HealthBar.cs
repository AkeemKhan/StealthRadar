using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        var slider = GetComponent<Slider>();
        slider.maxValue = PlayerStatistics.MaxHealth;
        slider.value = PlayerStatistics.Health;
    }
}
