/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The airburst arrow acts similar to how a "bomb" arrow will work in other games
*/
using UnityEngine;

public class AirburstArrow : BaseArrow {
    [Header("Airburst Values")]
    public float burstPower;
    public float burstRadius;

    /// <summary>
    /// Inheritted function for bursting instead
    /// </summary>
    protected override void Hit() {
        print("BOOM!");
        //gather collisions
        Collider[] hits = Physics.OverlapSphere(transform.position, burstRadius);
        
        foreach (var h in hits) {
            //explosion
            Rigidbody hRB = h.attachedRigidbody;
            if (hRB != null) {
                hRB.AddExplosionForce(burstPower, transform.position, burstRadius, 0.75f, ForceMode.Impulse);
            }
            //calculate damage if this is an enemy
            if (h.tag == "Enemy") {
                int damage = (int)(burstRadius - Vector3.Distance(transform.position, h.transform.position));
                damage *= 20;
                damage = Mathf.Clamp(damage, 0, damage);
                //h.GetComponent<BaseEnemy>().health -= damage;
            }
        }
    }
}
