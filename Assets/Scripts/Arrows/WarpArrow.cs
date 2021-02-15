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
        Transform pTrans = GameObject.FindGameObjectWithTag("Player").transform;
        /*
            NOTE:
                CharacterController overrides direct movement of the Player
                GameObject, in order to prevent this you MUST disable it, 
                directly teleport, and THEN reenable the CharacterController.
        */
        pTrans.GetComponent<CharacterController>().enabled = false;
        pTrans.position = transform.position;
        pTrans.GetComponent<CharacterController>().enabled = true;
        print("Warp");
        //Warp arrow is fragile and will break on collision
        Destroy(gameObject);
    }
}
