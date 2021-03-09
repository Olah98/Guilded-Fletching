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
    //velocity tracker
    public Vector3 GetVelocity { get { return velocity; } }
    private Vector3 velocity = Vector3.zero;
    private Vector3 posLastFrame;
    
    void Start() {
        var waypoints = from point in GetComponentsInChildren<Transform>()
                        where point.tag == "Waypoint" select point;
        //only store children with the "Waypoint" tag in a LL
        movePoints  = new LinkedList<Transform>(waypoints);
        curNode = movePoints.First;
        isBrambled = false;
        posLastFrame = transform.position;
        //unparent waypoints
        foreach (var w in waypoints) w.parent = transform.parent;
    }

    void FixedUpdate() {
        if (!isBrambled) {
            //move platform
            Vector3 moveTo = (curNode.Value.position - transform.position).normalized;
            transform.position += moveTo * speed * Time.fixedDeltaTime;
        }

        velocity = (transform.position - posLastFrame) / Time.fixedDeltaTime;
        posLastFrame = transform.position;
    }

    /// <summary>
    /// Waypoints are checked through trigger system.
    /// </summary>
    /// <param name="other">Colliding object.</param>
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Waypoint" && other.transform == curNode.Value) {
            //if the next node is null, return to the first in loop
            curNode = curNode.Next ?? movePoints.First;
        }
    }
}