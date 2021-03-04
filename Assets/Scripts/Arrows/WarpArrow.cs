/*
Author: Christian Mullins
Date: 02/04/2021
Summary: Arrow is used to teleport player to object hit.
*/
using UnityEngine;

public class WarpArrow : BaseArrow {
    private Transform pTrans;
    // store directions in an array for navigating teleportation algorithm
    private readonly Vector3[] dirs = { 
        Vector3.up,    Vector3.down,    Vector3.left,
        Vector3.right, Vector3.forward, Vector3.back };
        
    /// <summary>
    /// Check for appropriate collision, if so execute teleportation.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected override void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Warpable") {
            pTrans = GameObject.FindGameObjectWithTag("Player").transform;
            /*
            NOTE:
                CharacterController overrides direct movement of the Player
                GameObject, in order to prevent this you MUST disable it, 
                directly teleport, and THEN reenable the CharacterController.
            */
            pTrans.GetComponent<CharacterController>().enabled = false;
            pTrans.position = GetTeleportPosition(-transform.up + transform.position);
            pTrans.GetComponent<CharacterController>().enabled = true;
        }
        //Warp arrow is fragile and will break on collision
        Destroy(gameObject);
    }

    /// <summary>
    /// If the argument position is not a valid position (will collide with
    /// objects), iteratively move argument position in a direction away
    /// from collisions.
    /// </summary>
    /// <param name="colPos">Vector3 of the collision's position.</param>
    /// <returns>Valid position for the player.</returns>
    protected Vector3 GetTeleportPosition(Vector3 colPos) {
        Vector3 teleportTo = colPos;
        Vector3 incrementPos = GetCollisionDirection(colPos);
        if (incrementPos != Vector3.zero) {
            // continually increment until a valid position is found
            int debug = 0; // to prevent possible crashes while debugging
            while (incrementPos != Vector3.zero) {
                teleportTo += incrementPos;
                incrementPos = GetCollisionDirection(teleportTo);
                if (debug > 50) {
                    Debug.LogError("GetTelePortPosition(): no valid" + 
                    "teleportation position found.");
                    break;
                }
            }
        }
        return teleportTo;
    }

    /// <summary>
    /// Given a certain position, raycast to output a direction for a
    /// valid teleportation direction that shouldn't let a player move inside
    /// an object.
    /// </summary>
    /// <param name="checkPos">Vector3 position to check raycasts.</param>
    /// <returns>Direction to avoid collisions or (0, 0, 0).</returns>
    private Vector3 GetCollisionDirection(Vector3 checkPos) {
        Vector3 outputDir = Vector3.zero;
        // loop through every possible direction
        for (int i = 0; i < 6; ++i) {
            // change raylength to conform to capsule
            float rayLength = (i < 2) ? 0.5f : 0.25f;
            if (Physics.Raycast(checkPos, dirs[i], rayLength)) {
                // check if the player is "squashed" (collides on both sides)
                for (int axis = 0; axis < 3; ++axis) {
                    // if squashed, return increment towards player's position
                    if (outputDir[axis] + -dirs[i][axis] == 0f)
                        return ((pTrans.position - transform.position) 
                                 + Vector3.up).normalized;
                }
                // if hit, add the inverse of the direction found
                outputDir += -dirs[i];
            }
        }
        return outputDir.normalized;
    }
}