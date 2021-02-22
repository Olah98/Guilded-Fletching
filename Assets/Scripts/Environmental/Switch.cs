/*
Author: Christian Mullins
Date: 02/15/2021
Summary: Class that interacts with and triggers the Door script class.
*/
using UnityEngine;

public class Switch : MonoBehaviour {
    public Door myDoor;
    public bool isFlipped;

    void Start() {
        // push this Switch into the Switch List for myDoor
        myDoor.mySwitches.Add(this);
    }

    /// <summary>
    /// Function to call when switch is hit by an arrow or interacting with.
    /// </summary>
    public void HitSwitch() {
        if (isFlipped) return;

        isFlipped = true;
        if (myDoor.IsAllSwitchesFlipped()) {
            StartCoroutine(myDoor.SlideOpen());
        }
    }

    /// <summary>
    /// Check if an arrow hits the switch, then activate.
    /// </summary>
    /// <param name="other">Object hitting the switch.</param>
    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Arrow") {
            HitSwitch();
        }
    }
}