/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The bramble arrow that pauses an object in place.
*/
using UnityEngine;

public class BrambleArrow : BaseArrow {
    public float bindTime;
    private GameObject boundObj;

    /// <summary>
    /// Check for appropriate collision, if so execute teleportation.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected override void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Stoppable" && !isAbilityUsed) {
            Bind(other.gameObject);
            isAbilityUsed = true;
        }
    }

    /// <summary>
    /// Freeze object in space for a limited time.
    /// </summary>
    /// <param name="binding">Object that will be bound.</param>
    private void Bind(GameObject binding) {
        base.Hit();
        boundObj = binding;
        // adjust bool if script is found
        if (boundObj.TryGetComponent<MovingPlatform>(out var mPlat)) {
            mPlat.isBrambled = true;
        }
        // else check for rigidbody (if object run by physics)
        else if (boundObj.TryGetComponent<Rigidbody>(out var boundRB)) {
            boundRB.isKinematic = true;
        }

        Invoke("UnbindObject", bindTime);
    }

    /// <summary>
    /// Set bound object free and clear Rigidbody variable.
    /// </summary>
    private void UnbindObject() {
        if (boundObj.TryGetComponent<MovingPlatform>(out var mPlat)){
            mPlat.isBrambled = false;
        }
        else if (boundObj.TryGetComponent<Rigidbody>(out var boundRB)) {
            boundRB.isKinematic = false;
            boundObj = null;
        }

    }
}
