/*
Author: Christian Mullins
Date: 02/04/2021
Summary: Arrow is used to teleport player to object hit.
*/
using UnityEngine;

public class WarpArrow : BaseArrow {
    /// <summary>
    /// Check for appropriate collision, if so execute teleportation.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected override void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Wall") {
            //reflect velocity to a clamped point to get new position.
            Vector3 newPlayerPos = other.collider.ClosestPointOnBounds(other.transform.position);
            Vector3 tempVec = rB.velocity.normalized;
            //assume arrow arch, so zero the y-axis
            tempVec.y = 0f;
            newPlayerPos -= tempVec;
            //check if this arrow hit the floor
                //the floor is hit if the point of collision AND the hit gameobject has a lower
                //y-position than this gameobject's position
            if (transform.position.y > other.transform.position.y && transform.position.y > newPlayerPos.y) {
                //add the height of the player to the object
                newPlayerPos.y += 1f;
            }
            //teleport player to new position found
            Transform pTrans = GameObject.FindGameObjectWithTag("Player").transform;
            pTrans.SetPositionAndRotation(newPlayerPos, pTrans.rotation);
        }
    }
}
