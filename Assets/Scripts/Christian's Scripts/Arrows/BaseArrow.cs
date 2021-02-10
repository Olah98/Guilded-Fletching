/*
Author: Christian Mullins
Date: 02/04/2021
Summary: Parent arrow class to all game arrows.
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseArrow : MonoBehaviour {
    public int damage;
    public float drawSpeed;
    public float distance;
    public bool isLit;

    protected bool isAbilityUsed;
    protected Rigidbody rB;

    void Awake() {
        rB = GetComponent<Rigidbody>();
        isAbilityUsed = false;
    }

    /// <summary>
    /// Inheritted function that will be implemented differently on each arrow type.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected virtual void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Wall") {
            //TO DO
                //Create boolean gate for if the arrow should stick into the wall or bounce off.
            
            //call function as invoke to make sure the collider goes through a little.
            Hit();
        }
    }

    /// <summary>
    /// Inheritted function that will handle behavior upon hitting an object.
    /// </summary>
    protected virtual void Hit() {
        //implement arrow sticking function below
        //rB.isKinematic = true;
    }
}