using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyProb
{
    public GameObject enemy;
    public int prob;
}

public class SpawnEnemies : MonoBehaviour
{


    public int numSpawnEnemies;
    public List<EnemyProb> enemies;
    public GameObject poolEnemies;
    public LayerMask layerMaskPlayer;

    public void InstantiateEnemies(Vector3 enemyPos, Transform target)
    {
        GameObject enemy = Instantiate(GetEnemyToSpawn(), enemyPos, Quaternion.identity, poolEnemies.transform);

        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        enemyController.target = target;

    }

    GameObject GetEnemyToSpawn()
    {

        int totalProb = 0;

        foreach (EnemyProb enemy in enemies)
        {
            totalProb += enemy.prob;
        }

        int randomValue = UnityEngine.Random.Range(1, totalProb + 1);

        for (int i = 0; i < enemies.Count; i++)
        {
            if (randomValue <= enemies[i].prob)
            {
                return enemies[i].enemy;
            }
            else
            {
                randomValue -= enemies[i].prob;
            }
        }

        return null;
    }
    
}
