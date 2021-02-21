using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{

    private RespawnCoordinator rc;
    // Start is called before the first frame update
    private void Start()
    {
        rc = GameObject.FindGameObjectWithTag("RC").GetComponent<RespawnCoordinator>();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            rc.lastCheckpoint = transform.position;
        }

    }
}
