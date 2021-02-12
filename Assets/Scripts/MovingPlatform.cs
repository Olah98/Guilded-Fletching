/*
Author: Christian Mullins
Date: 02/08/2021
Summary: Prototype moving platform script that is being used to demonstrate the Bramble arrow.
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MovingPlatform : MonoBehaviour {
    public float speed;
    public bool isBrambled;

    private LinkedList<Transform> movePoints;
    private LinkedListNode<Transform> curNode;
    
    void Start() {
        var waypoints = from point in GetComponentsInChildren<Transform>()
                        where point.tag == "Waypoint" select point;
        //only store children with the "Waypoint" tag in a LL
        movePoints  = new LinkedList<Transform>(waypoints);
        curNode = movePoints.First;
        isBrambled = false;
        //unparent waypoints
        foreach (var w in waypoints) w.parent = null;
    }

    void FixedUpdate() {
        if (!isBrambled) {
            //move platform
            Vector3 moveTo = (curNode.Value.position - transform.position).normalized;
            transform.Translate(moveTo * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Waypoints are checked through trigger system.
    /// </summary>
    /// <param name="other">Colliding object.</param>
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Waypoint") {
            //if the next node is null, return to the first in loop
            curNode = curNode.Next ?? movePoints.First;
        }
    }
}