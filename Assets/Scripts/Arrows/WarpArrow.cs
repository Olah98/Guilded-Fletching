/*
Author: Christian Mullins
Date: 02/04/2021
Summary: Arrow is used to teleport player to object hit.
*/
using UnityEngine;

public class WarpArrow : BaseArrow {
    private Transform _warpPoint;

    protected override void Awake() {
        base.Awake();
        _warpPoint = transform.GetChild(0);
    }

    /// <summary>
    /// Check for appropriate collision, if so execute teleportation.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    public void Use(GameObject other)
    {
        if (other.transform.tag == "Warpable")
        {
            Transform pTrans = GameObject.FindGameObjectWithTag("Player").transform;
            /*
            NOTE:
                CharacterController overrides direct movement of the Player
                GameObject, in order to prevent this you MUST disable it, 
                directly teleport, and THEN reenable the CharacterController.
            */
            pTrans.GetComponent<CharacterController>().enabled = false;
            pTrans.position = _warpPoint.position;
            pTrans.GetComponent<CharacterController>().enabled = true;
        }
        //Warp arrow is fragile and will break on collision
        //Destroy(gameObject);
    }
}