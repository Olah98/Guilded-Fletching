/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The bramble arrow that pauses an object in place.
*/
using UnityEngine;

public class AirburstArrow : BaseArrow {
    /// <summary>
    /// Inheritted function for bursting instead, invoked OnCollision.
    /// </summary>
    protected override void Hit() {
        base.Hit();
        //burst attack
        //TO DO:
            //Create sphere collider that will expand and cause damage on collision?

    }
}
