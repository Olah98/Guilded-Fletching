/*
Author: Christian Mullins
Date: 02/15/2021
Summary: Class that interacts with and triggers the Door script class.
*/
using UnityEngine;

public class Switch : MonoBehaviour {
    [Tooltip("Can be any object that needs a triggered movement.")]
    public Door myDoor; // can be a door or ladder
    public bool isFlipped;
    [Tooltip("Check if the player is able to interact with this, or it can" +
            " only be triggered by arrows.")]
    public bool isInteractable;
    public bool triggerByArrow;

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
        if (other.transform.tag == "Arrow" && triggerByArrow) {
            HitSwitch();
        }
    }
}