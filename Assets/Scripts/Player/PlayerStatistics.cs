using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    public static int EnemiesKilled;
    public static int HighestStealthStreak;
    public static float TimeInAlert;

    // Current Game Stats
    public static int CurrentStealthStreak;
    public static float CurrentGameTime;
    public static float CurrentAlertTime;

    public static int Level = 1;
    public static int EnemyCount = 10;

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
    

    // Methods
    public static void ResetStats()
    {
        if (Level == 1)
        {
            Health = Stamina = MaxHealth = MaxStamina = 100;
            FloorsCleared = RoomsFound = Detected = PrisonersRescued =
            SecretRoomsFound = KeyCardsFound = EnemiesKilled = HighestStealthStreak =
            CurrentStealthStreak = 0;
            CurrentGameTime = TimeInAlert = 0;
            InAlertPhase = false;
            MaxSpeed = Speed = 2;
        }
    }

    public static void ClearFloor()
    {
        ClearTimes.Add(CurrentGameTime);
        CurrentGameTime = 0;
        // CurrentFloorCleared = true;
        Level++;
        EnemyCount++;
    }

    public static void DamagePlayer(float damage)
    {
        Health -= damage;
        if (Health < 0) PlayerAlive = false;
    }
}
