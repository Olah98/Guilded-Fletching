/*
Author: Christian Mullins
Date: 02/11/2021
Summary: Parent script to all enemy AI.
*/
using UnityEngine;

public class BaseEnemy : MonoBehaviour {
    public int health;
    public float speed;
    public int damage;
    public float attackFrequency;
    [Tooltip("Denoted by red wire sphere.")]
    public float rangeOfAttack;
    [Tooltip("Denoted by yellow wire sphere.")]
    public float aggroArea;
    public bool isAggroed;

    protected float attackTimer;
    protected static Transform playerTrans;

    protected virtual void Start() {
        isAggroed = false;
        attackTimer = 0f;
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
    }

    protected virtual void FixedUpdate() {}

    /// <summary>
    /// Inheritted function that will act differently depending on the enemy.
    /// </summary>
    protected virtual void Attack() {}

    /// <summary>
    /// Public variable that enables enemies to take damage from other scripts.
    /// </summary>
    /// <param name="damage">Damage value that the enemy will take.</param>
    public void TakeDamage(int damageTaking) {
        health -= damageTaking;
        if (health < 0) Destroy(gameObject);
    }

    /// <summary>
    /// Check if the player is in aggroArea and adjust isAggroed accordingly.
    /// </summary>
    /// <returns>Bool if enemy is aggroed.</returns>
    protected bool IsPlayerInAggroRange() {
        if (Vector3.Distance(transform.position, playerTrans.position) <= aggroArea) {
            isAggroed = true;
            return true;
        }
        isAggroed = false;
        return false;
    }

    /// <summary>
    /// Check if the player is in attack range.
    /// </summary>
    /// <returns>Bool if enemy is inside of attack range.</returns>
    protected bool IsPlayerInAttackRange() {
        return (Vector3.Distance(transform.position, playerTrans.position) 
                <= rangeOfAttack);
    }

    /// <summary>
    /// Draws spheres to give visual feedback on the range of attack and aggro
    /// area of the enemy selected.
    /// </summary>
    private void OnDrawGizmosSelected() {
        //draw agggro range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroArea);
        //draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeOfAttack);
    }
}