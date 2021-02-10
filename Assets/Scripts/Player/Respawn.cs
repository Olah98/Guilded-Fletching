﻿//Add this to the player script when you can

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Vector3 respawnPoint;

    void Start()
    {
        respawnPoint = transform.position;
    }
    
    void Update()
    {
        if (transform.position.y <= -5)
            transform.position = respawnPoint;
    }
}
