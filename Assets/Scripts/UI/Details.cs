using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Details : MonoBehaviour
{

    public int PlayerLevel;
    public int PlayerExp;
    public int Detections;
    public int TotalEnemiesKilled;
    public int EnemiesKilledSoFar;
    public int StealthStreak;
    public int Stage;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UIUpdate();
    }

    private void UIUpdate()
    {
        PlayerLevel = PlayerStatistics.PlayerLevel;
        PlayerExp = PlayerStatistics.PlayerExp;
        Detections = PlayerStatistics.Detections;
        TotalEnemiesKilled = PlayerStatistics.EnemiesKilled;
        EnemiesKilledSoFar = PlayerStatistics.EnemiesKilledThisRound;
        StealthStreak = PlayerStatistics.CurrentStealthStreak;
        Stage = PlayerStatistics.Stage;

        var pl = GameObject.Find("PlayerLevel");
        var exp = GameObject.Find("PlayerExp");
        var det = GameObject.Find("Detections");
        var ko = GameObject.Find("TotalEnemiesKilled");
        var str = GameObject.Find("Streak");
        var stg = GameObject.Find("Stage");

        var plText = pl.GetComponent<UnityEngine.UI.Text>();
        plText.text = PlayerLevel.ToString();

        var expText = exp.GetComponent<UnityEngine.UI.Text>();
        expText.text = PlayerExp.ToString();

        var detC = det.GetComponent<UnityEngine.UI.Text>();
        detC.text = Detections.ToString();

        var koC = ko.GetComponent<UnityEngine.UI.Text>();
        koC.text = EnemiesKilledSoFar.ToString();

        var strC = str.GetComponent<UnityEngine.UI.Text>();
        strC.text = StealthStreak.ToString();

        var stgC = stg.GetComponent<UnityEngine.UI.Text>();
        stgC.text = Stage.ToString();
    }
}
