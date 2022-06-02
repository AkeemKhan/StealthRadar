using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatistics
{
    // Player Stats
    public static int MaxHealth;
    public static int MaxStamina;
    public static float Health;
    public static float Stamina;
    public static float MaxSpeed;
    public static float Speed;
    public static float RotateSpeed;
    public static float Movex;
    public static float Movey;
    public static bool PlayerAlive;
    public static bool InGame;
    public static bool InMenu;
    public static bool InAlertPhase;

    // Overall Game Stats
    public static List<float> ClearTimes = new List<float>();
    public static List<bool> FloorClear = new List<bool>();
    public static int FloorsCleared;
    public static int RoomsFound;
    public static int Detected;
    public static int PrisonersRescued;
    public static int SecretRoomsFound;
    public static int KeyCardsFound;
    public static int HighestStealthStreak;
    public static float TimeInAlert;

    // Current Game Stats
    public static int CurrentStealthStreak;
    public static float CurrentGameTime;
    public static float CurrentAlertTime;
    public static int Detections = 0;    

    public static int Stage = 1;
    public static int EnemyCount = 8;
    public static int EnemiesKilled;
    public static int EnemiesKilledThisRound;

    public static int PlayerLevel = 1;
    public static int PlayerExp = 0;
    public static int DifficultyModifier = 0;

    // Accessors
    public static bool CurrentFloorCleared
    {
        get { return FloorClear.LastOrDefault();  }
        set
        {
            FloorClear[FloorClear.Count - 1] = value;
            if (value)
                FloorClear.Add(false);
        }
    }

    public static float CurrentSpeed
    {
        get { return MaxSpeed; }
    }
    
    public static void ResetStats()
    {
        CurrentStealthStreak = 0;
        Detections = 0;

        if (Stage == 1)
        {
            EnemyCount = 8;
            Health = MaxHealth = 100;
            Stamina = MaxStamina = 20;
            FloorsCleared = RoomsFound = Detected = PrisonersRescued =
            SecretRoomsFound = KeyCardsFound = EnemiesKilled = HighestStealthStreak = 0;            
            InAlertPhase = false;
            MaxSpeed = 2.8f;
            Speed = 2;
        }
    }

    public static void IncreaseExp(float addExp)
    {
        PlayerExp += (int)addExp;
        
        while (PlayerExp >= 100)
        {
            PlayerExp -= 100; 
            LevelUp();
        }
    }

    public static void LevelUp()
    {
        PlayerLevel++;
        Speed *= Speed >= MaxSpeed ? 1.01f : 1.04f;
        MaxHealth += Random.Range(5, 15);
        MaxStamina += 1;

        Stamina = MaxStamina;
        Health += MaxHealth/10;
    }

    public static void ClearFloor()
    {
        ClearTimes.Add(CurrentGameTime);
        CurrentGameTime = 0;
        CurrentStealthStreak = 0;
        DifficultyModifier += Detections * 5;

        var completionBonus = 50;
        var undetectedBonus = PlayerStatistics.Detections == 0 ? 50 : 0;
        var enemyKilledBonus = EnemiesKilledThisRound * 5;
        var stealthKillStreakBonus = System.Math.Pow(PlayerStatistics.CurrentStealthStreak, 2);
        var totalBonus = completionBonus + undetectedBonus + enemyKilledBonus + stealthKillStreakBonus;

        IncreaseExp((int)totalBonus);

        var healAmount = MaxHealth / (Detections == 0 ? 20 : 10);

        Stamina = MaxStamina;
        Health += healAmount;

        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        Stage++;
        EnemyCount++;
    }  

    public static void DamagePlayer(float damage)
    {
        Health -= damage;
        if (Health < 0) PlayerAlive = false;

        if (PlayerStatistics.Health <= 0)
        {
            if (PlayerLevel > 1)
            {
                PlayerLevel = 1;
            }
            else if (PlayerLevel == 1)
            {
                Stage = 1;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public static void DamageStamina(float damage)
    {
        Stamina -= damage;        
    }
}
