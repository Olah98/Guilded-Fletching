/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The airburst arrow acts similar to how a "bomb" arrow will work in 
        other games
*/
using UnityEngine;
using System.Collections;

public class AirburstArrow : BaseArrow {
    [Header("Airburst Values")]
    public float burstPower;
    public float burstRadius;
    public bool canPushPlayer; // set to false for now

    /// <summary>
    /// Inheritted function for bursting instead
    /// </summary>
    protected override void Hit() {
        Collider[] hits = {};
        int numOfHits = Physics.OverlapSphereNonAlloc(transform.position, 
                                                burstRadius, hits);
        // take all collisions and handle them all seperately based on their 
        // tags
        for (int i = 0; i < numOfHits; ++i) {
            if (hits[i].tag == "Arrow") {
                if (hits[i].gameObject != gameObject) 
                    Destroy(hits[i].gameObject);
                continue;
            }

            int damage = CalculateDamage(hits[i].transform.position);
            // explosion
            Rigidbody hRB = hits[i].attachedRigidbody;
            if (hRB != null) {
                hRB.AddExplosionForce(burstPower, transform.position, 
                                      burstRadius, 0.75f, ForceMode.Impulse);
            }
            if (hits[i].tag == "Enemy")
                hits[i].GetComponent<BaseEnemy>().TakeDamage(damage);
            if (hits[i].tag == "Player") {
                // handle player collision without RigidBody
                if (canPushPlayer) {
                    var cc = hits[i].GetComponent<CharacterController>();
                    Vector3 forceDir =  hits[i].transform.position 
                                        - transform.position;
                    float clampVal = Vector3.Distance(transform.position, 
                                                      hits[i].transform.position);
                    forceDir = Vector3.ClampMagnitude(forceDir, 0.25f);
                    // override CC movements in order to move position
                    cc.enabled = false;
                    hits[i].transform.position += forceDir * burstPower;
                    cc.enabled = true;
                }
                hits[i].GetComponent<Character>().TakeDamage(damage);
            }
        } // end forloop
        Destroy(gameObject);
    }

    /// <summary>
    /// Perform damage calculation based on proximity and 
    /// </summary>
    /// <param name="pointHit"></param>
    /// <returns></returns>
    private int CalculateDamage(Vector3 pointHit) {
        float outFloat = Vector3.Distance(transform.position, pointHit);
        outFloat = (burstRadius - outFloat) * damage;
        return (int)outFloat;
    }

    // Only use to show how proximity is related to initial velocity
    [System.Obsolete]
    private IEnumerator WaitAndPrintVelocity(Transform hit) {
        yield return new WaitForSecondsRealtime(0.1f);

        // print proximity to this and current velocity
        float prox = Vector3.Distance(transform.position, 
                                      hit.transform.position);
        var hitRB = hit.GetComponent<Rigidbody>();
        print("hit: " + hit.name + ", proximity: " + prox 
              + ", veloc: " + hitRB.velocity.magnitude);

        yield return null;
    }
}
