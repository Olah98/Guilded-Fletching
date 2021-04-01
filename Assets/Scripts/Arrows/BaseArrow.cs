﻿/*
Author: Christian Mullins
Date: 02/04/2021
Summary: Parent arrow class to all game arrows.
*/
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BaseArrow : MonoBehaviour {
    public int damage;
    [Tooltip("Acts as a multiplier when calculating draw speed.")]
    [Range(0.1f, 2.0f)]
    public float drawSpeed = 1.0f; // baseline multiplier
    public float distance;
    

    protected bool isAbilityUsed;
    protected Rigidbody rB;

    protected virtual void Awake() {
        rB = GetComponent<Rigidbody>();
        isAbilityUsed = false;
    }
/*
    private void FixedUpdate() {
        if (!rB.IsSleeping() && !isAbilityUsed)
            transform.up = rB.velocity;
    }
*/
    /// <summary>
    /// Inheritted function that will be implemented differently on each arrow 
    /// type.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected virtual void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Enemy") {
            other.gameObject.GetComponent<BaseEnemy>().TakeDamage(damage);
            Debug.Log("Enemy Collision");
        }
        if (other.transform.tag == "Player")
        {
            other.gameObject.GetComponent<Character>().TakeDamage(damage);
            Debug.Log("Damage dealt");
        }
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

    

    //By Warren
    void DestroyObjectDelayed()
    {
        // Kills the game object in 60 seconds after loading the object
        Destroy(gameObject, 60);
    }
}
