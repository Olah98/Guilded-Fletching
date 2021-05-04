﻿/*
Author: Christian Mullins
Date: 02/08/2021
Summary: Base moving platform script that is being used to demonstrate the Bramble arrow.
*/

using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour, IBrambleable
{
    public float speed;
    public bool isStopped; // originally "isBrambled"

    protected LinkedList<Transform> movePoints;
    protected LinkedListNode<Transform> curNode;

    protected virtual void Start()
    {
        var waypoints = from point in GetComponentsInChildren<Transform>()
                        where point.tag == "Waypoint" select point;
        //only store children with the "Waypoint" tag in a LL
        movePoints = new LinkedList<Transform>(waypoints);
        curNode = movePoints.First;
        isStopped = false;
        //unparent waypoints
        foreach (var w in waypoints) w.parent = transform.parent;
    }

    protected virtual void FixedUpdate() {
        if (isStopped) return;
        //move platform
        Vector3 moveTo = (curNode.Value.position - transform.position).normalized;
        transform.position += moveTo * speed * Time.fixedDeltaTime;
    }

    //defined in IBrambleable interface
    public void Bramble(in bool enabled) {
        isStopped = enabled;
    }

    public IEnumerator CheckForPlayerSquashing(Character character) {
        isStopped = true;
        do {
            yield return new WaitForFixedUpdate();
        } while (character.isSquashed);
        isStopped = false;
        yield return null;
    }

    /// <summary>
    /// Waypoints are checked through trigger system.
    /// </summary>
    /// <param name="other">Colliding object.</param>
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint" && other.transform == curNode.Value)
        {
            //if the next node is null, return to the first in loop
            curNode = curNode.Next ?? movePoints.First;
        }
    }
}