using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private RespawnCoordinator rc;
    private bool checkPointed = false;
    // Start is called before the first frame update
    private void Start()
    {
        rc = GameObject.FindGameObjectWithTag("RC").GetComponent<RespawnCoordinator>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!checkPointed)
            {
                rc.lastCheckpoint = transform.position;
                checkPointed = true;
            }
        }

    }
}
