using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnCoordinator : MonoBehaviour
{
    private static RespawnCoordinator instance;
    public Vector3 lastCheckpoint;

    private void Awake()
    {
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
