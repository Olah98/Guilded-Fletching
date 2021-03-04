using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    public bool activeCheckpoint = false;
   
    // Start is called before the first frame update
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && activeCheckpoint==false)
        {
            //Reset active checkpoint
            GameObject[] checkpoints;
            checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");

            //Players may only save at a checkpoint once until they save at another one.
            foreach (GameObject checkpoint in checkpoints)
            {
                checkpoint.GetComponent<Checkpoint>().activeCheckpoint = false;
            }
            //set new checkpoint to active
            activeCheckpoint = true;

            //Set's variables for the save
            SaveManager.instance.activeSave.respawnPos = transform.position;
            SaveManager.instance.SaveVariables();
            SaveManager.instance.activeSave.sceneName = SceneManager.GetActiveScene().name;
            SaveManager.instance.Save();
  

        }

    }
}
