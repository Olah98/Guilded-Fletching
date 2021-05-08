using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ArcherEnemy))]
public class ArcherAnimController : MonoBehaviour {
    public Animator bowAnimator => _bowAnimator;
    [SerializeField] private Animator _bowAnimator;
    private Animator _bodyAnimator;
    private float _fireAnimTime;
    private ArcherEnemy thisEnemy;

    public readonly Dictionary<string, int> enemyAnimHashTable
        = new Dictionary<string, int> {
            { "Attacking", Animator.StringToHash("Attacking")   },
            { "Dead",      Animator.StringToHash("Dead")        },
    };

    public bool isFiringAnimation { get; private set; }

    private void Start() {
        _bodyAnimator = GetComponent<Animator>();
        thisEnemy = GetComponent<ArcherEnemy>();
        isFiringAnimation = false;
        //get length of firing animation
        _SetAllBools(enemyAnimHashTable["Dead"], false);
        _SetAllBools(enemyAnimHashTable["Attacking"], false);
    }

    public IEnumerator TriggerEnemyAttackAnim() {
        isFiringAnimation = true;
        _SetAllBools(enemyAnimHashTable["Attacking"], true);
        yield return new WaitForSeconds(2f); // set as length of fire anim
        _SetAllBools(enemyAnimHashTable["Attacking"], false);
        isFiringAnimation = false;
    }

    public void TriggerDeathAnim() {
        _SetAllBools(enemyAnimHashTable["Dead"], true);
    }

    //functions below aid in cleaning up excessive lines of code due 
    //to seperate animators
    private void _SetAllBools(int hash, bool value) {
        _bowAnimator.SetBool(hash, value);
        _bodyAnimator.SetBool(hash, value);
    }
    private void _SetAllFloats(int hash, float value) {
        _bowAnimator.SetFloat(hash, value);
        _bodyAnimator.SetFloat(hash, value);
    }
    private void _SetAllTriggers(int hash) {
        _bowAnimator.SetTrigger(hash);
        _bodyAnimator.SetTrigger(hash);
    }
}
