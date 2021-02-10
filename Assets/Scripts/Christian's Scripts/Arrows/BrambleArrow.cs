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
        if (other.transform.tag == "Stoppable") {
            Bind(other.gameObject);
            print("Hit");
        }
    }

    /// <summary>
    /// Freeze object in space for a limited time.
    /// </summary>
    /// <param name="binding">Object that will be bound.</param>
    private void Bind(GameObject binding) {
        base.Hit();
        boundObj = binding;
        //check for rigidbody (if object run by physics)
        if (boundObj.TryGetComponent<Rigidbody>(out Rigidbody boundRB)) {
            boundRB.isKinematic = true;
        }
        //else adjust bool
        else if (boundObj.TryGetComponent<MovingPlatform>(out MovingPlatform mPlat)) {
            mPlat.isBrambled = true;
        }
        Invoke("UnbindObject", bindTime);
    }

    /// <summary>
    /// Set bound object free and clear Rigidbody variable.
    /// </summary>
    private void UnbindObject() {
        if (boundObj.TryGetComponent<Rigidbody>(out Rigidbody boundRB)) {
            boundRB.isKinematic = false;
            boundObj = null;
        }
        else if (boundObj.TryGetComponent<MovingPlatform>(out var mPlat)){
            mPlat.isBrambled = false;
        }
    }
}
