/*
Author: Christian Mullins
Date: 02/13/2021
Summary: Enemy specializing in close combat, inheritted from base enemy class.
*/
using System.Collections;
using UnityEngine;

public class MeleeEnemy : BaseEnemy {
    private float storeSpeed; // altering speed for charging values
    private float storePlayerSpeed;

    protected override void Start() {
        isAggroed = false;
        attackTimer = attackFrequency; // initialize an attack to occur
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        storePlayerSpeed = playerTrans.GetComponent<Character>().speed;
        storeSpeed = speed;
    }

    protected override void FixedUpdate() {
        // aggro and attack
        if (!IsPlayerInAggroRange()) return;
        if (IsPlayerInAttackRange() && attackTimer >= attackFrequency) { 
            Attack();
            attackTimer = 0f;
        }

        // manage attack timer
        if (isAggroed) attackTimer += Time.deltaTime;

        // movement
        Vector3 movePos = playerTrans.position - transform.position;
        movePos.y = 0f;
        transform.position += movePos.normalized * speed * Time.deltaTime;
        // adjust rotation to always look forward
        transform.LookAt(movePos + transform.position, transform.up);
    }

    /// <summary>
    /// Melee implementation of enemy attack by charging the player.
    /// </summary>
    protected override void Attack() {
        // charge player
        speed = storePlayerSpeed * 1.25f;
        // cooldown
        StartCoroutine("AttackCoolDown");
    }

    /// <summary>
    /// Called inside Attack() to reset relavent values for new attack.
    /// </summary>
    private IEnumerator AttackCoolDown() {
        yield return new WaitForSeconds(1f);
        // step one
        print("Backing up!");
        speed = -speed;
        attackTimer = 0f;
        yield return new WaitForSeconds(0.5f);
        // step two
        speed = 0f;
        yield return new WaitForSeconds(0.25f);
        // leave coroutine
        speed = storeSpeed;
        yield return null;
    }
}