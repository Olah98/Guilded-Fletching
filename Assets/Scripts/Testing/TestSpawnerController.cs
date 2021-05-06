/*
Author: Christian Mullins
Date: 02/23/2021
Summary: Controls and facilitates the usage of the TestSpawner but passes
    commands from the player/user to the TestSpawner.
*/
using UnityEngine;
using UnityEngine.UI;

public class TestSpawnerController : MonoBehaviour {
    public TestSpawner spawner;
    public Canvas spawningCanvas;
    public int[] numOfEnemies;
    public Text[] numOfEnemiesText;

    // array values below to never be altered unless new enemies are added
    private readonly string[] prefixText = { 
        "# of Melee: ", "# of Archer: ", "# of Bomb: " 
    };

    private void Start() {
        numOfEnemies = new int[spawner.enemyPrefabs.Length];
        //assign numOfEnemiesTex
        spawningCanvas.enabled = false;
    }

    private void Update() {
        // make sure that the text is always up to date with numOfEnemies
        for (int i = 0; i < spawner.enemyPrefabs.Length; ++i) {
            numOfEnemiesText[i].text = prefixText[i] + numOfEnemies[i];
        }
    }

    /// <summary>
    /// Used for selecting more enemies to spawn on the button UI system.
    /// </summary>
    /// <param name="enemyTypeStr">String of enemy type.</param>
    public void IncrementEnemyNumber(string enemyTypeStr) {
        switch (enemyTypeStr) {
            case "Melee": numOfEnemies[(int)EnemyType.Melee]++;
                break;
            case "Archer": numOfEnemies[(int)EnemyType.Archer]++;
                break;
            case "Bomber": numOfEnemies[(int)EnemyType.Bomb]++;
                break;
            default: Debug.LogError("IncrementEnemyNumber: '" + enemyTypeStr
                                     + "' is not a valid enemy name.");
                break;
        }
    }

    /// <summary>
    /// Return all enemy number values to 0.
    /// </summary>
    public void ClearEnemyNumbers() {
        for (int i = 0; i < numOfEnemies.Length; ++i)
            numOfEnemies[i] = 0;
    }

    /// <summary>
    /// Spawn number of enemies based on the values selected, then reset values.
    /// </summary>
    public void SpawnEnemies() {
        for (int enemy = 0; enemy < numOfEnemies.Length; ++enemy) {
            for (int i = 0; i < numOfEnemies[enemy]; ++i)
                spawner.SpawnEnemy((EnemyType)enemy);
            numOfEnemies[enemy] = 0;
        }
    }

    /// <summary>
    /// Function for calling UI button to clear all enemies from the scene.
    /// </summary>
    public void ClearEnemiesFromScene() {
        spawner.DestroyAllEnemies();
    }

    //  OnTrigger functions control logic of when to display spawner canvas
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") 
            spawningCanvas.enabled = true;
    }
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") 
            spawningCanvas.enabled = false;
    }
}
