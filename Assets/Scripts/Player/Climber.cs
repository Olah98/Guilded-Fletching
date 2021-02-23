/*
Author Christian Mullins
Date: 02/19/2021
Summary: Attach to object that you wish the player to be able to climb.
*/
using UnityEngine;

public class Climber : MonoBehaviour {
    [Range(1f, 5f)]
    public float climbSpeed;
    private CharacterController controller;

    void Start() {
        controller = null;
    }

    /// <summary>
    /// Called while the player is in range of the collider. And overrides
    /// native player controls for climbing movement scheme.
    /// </summary>
    /// <param name="other">Only activates if player.</param>
    private void OnTriggerStay(Collider other) {
        // for climbing
        if (other.tag == "Player") {
            controller = other.GetComponent<CharacterController>();
            Character character = other.GetComponent<Character>();
            // begin climb
            if (controller.isGrounded) {
                if (IsPlayerFacingLadder(other.transform)) {
                    // override controls in Character.cs
                    character.isClimbing = true;
                    controller.Move(GetClimbingMovement() * Time.fixedDeltaTime);
                }
                else character.isClimbing = false;
            }
            // handle mid climb
            else {
                character.isClimbing = true;
                controller.Move(GetClimbingMovement() * Time.fixedDeltaTime);
            }
        }
    }

    /// <summary>
    /// Called once the player has left range of the collider. This will trigger
    /// the return to the normal control scheme
    /// </summary>
    /// <param name="other">Only activates if player</param>
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player") {
            // if the player is ascending upon exit
            if (Input.GetAxis("Vertical") > 0f) {
                // give a boost to reach the top
                controller.Move(Vector3.up * 0.5f);
            }
            controller = null;
            other.GetComponent<Character>().isClimbing = false;
        }
    }

    /// <summary>
    /// Use while blocking standard movement scheme to get appropriate climbing
    /// output movement.
    /// </summary>
    /// <returns>Movement from input for climbing.</returns>
    private Vector3 GetClimbingMovement() {
        Vector3 moveDir = controller.transform.right * Input.GetAxis("Horizontal")
                        + controller.transform.up * Input.GetAxis("Vertical");
        return moveDir * climbSpeed;
    }

    /// <summary>
    /// To prevent players from accidentally hitting the climber collider when
    /// they don't intend to hit it.
    /// </summary>
    /// <param name="facingDir">True if the player is facing the direction
    ///  of the ladder.</param>
    /// <returns></returns>
    private bool IsPlayerFacingLadder(Transform facingDir) {
        Vector3 adjustedLook = facingDir.forward;
        adjustedLook.y = -facingDir.up.y;
        float castDistance = Vector3.Distance(facingDir.position, 
                                              transform.position);

        if (Physics.Raycast(facingDir.position, 
                            adjustedLook, out var hit, castDistance)) {
            if (hit.transform.gameObject == gameObject)
                return true;
        }
        return false;
    }
}