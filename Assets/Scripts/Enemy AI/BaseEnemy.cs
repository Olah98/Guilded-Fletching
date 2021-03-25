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
    public bool isDead = false;
    public bool isBrambled;

    protected float _attackTimer;
    protected Transform _playerTrans;

    private float _storeAggroArea;
    private Character _character;

    protected virtual void Start() {
        isAggroed = false;
        isBrambled = false;
        _attackTimer = 0f;
        _storeAggroArea = aggroArea;
        var player = GameObject.FindGameObjectWithTag("Player");
        _playerTrans = player.transform;
        _character = player.GetComponent<Character>();
    }

    protected virtual void FixedUpdate() { 
        if (isBrambled) return;
    }

    protected void Update() {
        if (isDead) {
            SaveManager.instance.activeSave.unsavedDead.Add(gameObject.name);
            Destroy(gameObject);
        }

        aggroArea = (_character.isCrouching) ? _storeAggroArea * 0.4f  : _storeAggroArea;
    }


    /// <summary>
    /// Inheritted function that will act differently depending on the enemy.
    /// </summary>
    protected virtual void Attack() {}

    /// <summary>
    /// Inheritted function that will act differently depending on the enemy.
    /// </summary>
    /// <param name="target">Vector3 of position to shoot.</param>
    protected virtual void ShootAt(Vector3 target) {}

    /// <summary>
    /// Public variable that enables enemies to take damage from other scripts.
    /// </summary>
    /// <param name="damage">Damage value that the enemy will take.</param>
    public void TakeDamage(int damageTaking) {
        health -= damageTaking;
        if (health < 0) {
            isDead=true; }
    }

    /// <summary>
    /// Check if the player is in aggroArea and adjust isAggroed accordingly.
    /// </summary>
    /// <returns>Bool if enemy is aggroed.</returns>
    protected bool IsPlayerInAggroRange() {
        if (!isBrambled)
        {
            isAggroed = (Vector3.Distance(transform.position, _playerTrans.position)
          <= aggroArea);
        } else
        {
            isAggroed = false;
        }
        return isAggroed;
    }

    /// <summary>
    /// Check if the player is in attack range.
    /// </summary>
    /// <returns>Bool if enemy is inside of attack range.</returns>
    protected bool IsPlayerInAttackRange() {
        return (Vector3.Distance(transform.position, _playerTrans.position) 
                <= rangeOfAttack);
    }

    /// <summary>
    /// Draws spheres to give visual feedback on the range of attack and aggro
    /// area of the enemy selected.
    /// </summary>
    private void OnDrawGizmosSelected() {
        //draw aggro range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroArea);
        //draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeOfAttack);
    }
}