/*
Author: Christian Mullins
Date: 02/15/2021
Summary: The class called from the Switch to open the Door's GameObject.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Door : MonoBehaviour {
    public float openSpeed;
    public List<Switch> mySwitches;
    private Transform slideTo;

    void Start() {
        slideTo = transform.GetChild(0);
    }

    /// <summary>
    /// Iterate through every Switch to check if they are all flipped.
    /// </summary>
    /// <returns>True if all switches are flipped.</returns>
    public bool IsAllSwitchesFlipped() {
        foreach (var s in mySwitches) {
            if (!s.isFlipped) 
                return false;
        }
        return true;
    }

    /// <summary>
    /// Coroutine to move the Door outside of using Update().
    /// </summary>
    public IEnumerator SlideOpen() {
        // detach slideTo child
        slideTo.parent = null;
        do {
            Vector3 moveTo = (slideTo.position - transform.position).normalized;
            transform.Translate(moveTo * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        } while (!IsPositionApproximateTo(slideTo.position));
        // re-attach slideTo child
        slideTo.parent = transform;
        yield return null;
    }

    //FIX THIS SO THAT RETURNS TRUES
    private bool IsPositionApproximateTo(Vector3 compareTo) {
        if (Vector3.Distance(transform.position, compareTo) < 0.25f) 
            return true;

        return false;
    }
}