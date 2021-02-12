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

    protected virtual void Awake() {
        rB = GetComponent<Rigidbody>();
        isAbilityUsed = false;
    }

    /// <summary>
    /// Inheritted function that will be implemented differently on each arrow type.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected virtual void OnCollisionEnter(Collision other) {
        if (!isAbilityUsed) {
            Hit();
            isAbilityUsed = true;
        }
    }

    /// <summary>
    /// Inheritted function that will handle behavior upon hitting an object.
    /// </summary>
    protected virtual void Hit() {
        
    }
}