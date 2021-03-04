/*
Author: Christian Mullins
Date: 02/23/2021
Summary: The script controls the actual spawner object itself and has public
    functions to be manipulated by the TestSpawnerController.cs
*/
using UnityEngine;
using System.Collections.Generic;

public enum EnemyType {
    Melee, Archer, Bomb
}

[RequireComponent(typeof(BoxCollider))]
public class TestSpawner : MonoBehaviour {
    [Header("Enemies: Melee, Archer, & Bomber")]
    public GameObject[] enemyPrefabs;

    private BoxCollider spawnCollider;
    private LinkedList<GameObject> enemyList;

    private void Start() {
        spawnCollider = GetComponent<BoxCollider>();
        enemyList = new LinkedList<GameObject>();
    }

    /// <summary>
    /// Main public function to control spawning.
    /// </summary>
    /// <param name="type">EnemyType enum denoting what to spawn.</param>
    public void SpawnEnemy(EnemyType type) {
        Vector3 spawnPos = GetNewSpawnLocation();
        GameObject newEnemy = enemyPrefabs[(int)type];
        newEnemy = Instantiate(newEnemy, spawnPos, newEnemy.transform.rotation);
        enemyList.AddLast(newEnemy);
    }

    /// <summary>
    /// Destroy all the enemy GameObjects stored in the enemyList.
    /// </summary>
    public void DestroyAllEnemies() {
        var curNode = enemyList.First;
        while (curNode != null) {
            Destroy(curNode.Value);
            curNode = curNode.Next;
        }
        enemyList = new LinkedList<GameObject>();
    }

    /// <summary>
    /// Use when spawning an enemy in the TestSpawner.
    /// </summary>
    /// <returns>Valid spawn position for this spawner.</returns>
    private Vector3 GetNewSpawnLocation() {
        Vector3 sRange = transform.lossyScale * 0.5f;
        Vector3 adjustPos = new Vector3 (Random.Range(-sRange.x, sRange.x),
                                         Random.Range(-sRange.y, sRange.y),
                                         Random.Range(-sRange.z, sRange.z));
        return transform.position + adjustPos;                                    
    }
}
