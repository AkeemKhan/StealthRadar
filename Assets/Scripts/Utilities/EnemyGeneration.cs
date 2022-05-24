using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGeneration : MonoBehaviour
{
    public List<Vector2> EnemySpawnPositions;
    public int EnemyCount;
    public int DifficultyModifier;

    //public GameObject WeakGuard;
    //public GameObject LightGuard;
    public GameObject StandardGuard;
    public GameObject HeavyGuard;
    //public GameObject EliteGuard;
    //public GameObject Boss;

    public void Initialise(List<Vector2> enemySpawnPositions, int enemyCount, int difficultyModifier)
    {
        EnemySpawnPositions = enemySpawnPositions;
        EnemyCount = enemyCount;
        DifficultyModifier = difficultyModifier;
    }

    public void GenerateEnemies()
    {
        for (int i = 0; i < EnemyCount; i++)
        {
            var SelectedEnemy = Random.Range(0, 100) <= 20 ? HeavyGuard : StandardGuard;
            if (EnemySpawnPositions.Count > 0)
            {
                Vector2 position = EnemySpawnPositions[UnityEngine.Random.Range(0, EnemySpawnPositions.Count - 1)];
                Instantiate(SelectedEnemy, new Vector3(position.x, position.y), Quaternion.identity);;
            }
        }
    }
}
