/*
Author: Christian Mullins
Date: 02/08/2021
Summary: Prototype moving platform script that is being used to demonstrate the Bramble arrow.
*/

using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour {
    public float speed;
    public bool isBrambled;
    private LinkedList<Transform> movePoints;
    private LinkedListNode<Transform> curNode;

    void Start() {
        movePoints  = new LinkedList<Transform>(transform.GetComponentsInChildren<Transform>());
        curNode = movePoints.First;
        isBrambled = false;
        transform.DetachChildren();
        /*
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        */
    }

    void FixedUpdate() {
        if (!isBrambled) {
            //move platform
            Vector3 moveTo = (curNode.Value.position - transform.position).normalized;
            transform.Translate(moveTo * speed * Time.deltaTime);
            //check if this in range of the current move point
            if (transform.position == curNode.Value.position) {
                //if the next node is null, return to the first in loop
                curNode = curNode.Next ?? movePoints.First;
            }
        }
    }
}