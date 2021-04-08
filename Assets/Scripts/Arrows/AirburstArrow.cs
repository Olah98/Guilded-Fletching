/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The airburst arrow acts similar to how a "bomb" arrow will work in 
        other games
*/
using UnityEngine;
using System.Collections;
using System.Threading.Tasks;

public class AirburstArrow : BaseArrow {
    [Header("Airburst Values")]
    public float burstPower;
    public float burstRadius;
    public bool canPushPlayer; // set to false for now
    private bool _hasDamagedPlayer = false;

    /// <summary>
    /// Inheritted function for bursting instead
    /// </summary>
    protected override void  Hit() {
        Collider[] hits = new Collider[20];
        // max number of collisions = hits.Length
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
            // check to destroy
            if (hits[i].TryGetComponent<Blastable>(out var b)) {
                if (b.isDestroyable) Destroy(hits[i].gameObject);
                continue;
            }
            // explosion
            Rigidbody hRB = hits[i].attachedRigidbody;
            if (hRB != null) {
                hRB.AddExplosionForce(burstPower, transform.position, 
                                      burstRadius, 5f, ForceMode.VelocityChange);
                //StartCoroutine(WaitAndPrintVelocity(hits[i].transform));
            }
            if (hits[i].tag == "Enemy")
                hits[i].GetComponent<BaseEnemy>().TakeDamage(damage);
            if (hits[i].tag == "Player" && !_hasDamagedPlayer) {
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
                if (!_hasDamagedPlayer)
                {
                    hits[i].GetComponent<Character>().TakeDamage(damage);
                    Debug.Log(hits[i].tag);
                    _hasDamagedPlayer = true;
                }
                
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
