/*
Author: Christian Mullins
Date: 03/08/2021
Summary: Controls the flow on animation for the player in it's various states.
    -Idle
    -Walking
    -Jumping
    -Loading Arrow
    -Switching Arrows
    -Pulling string (drawing arrow)
    -Full charge    (FullyDrawn)
    -Release string (shooting)
    -Crouching
    -Take Damage
*/
using UnityEngine;
using UnityEngine.Animations;

/// <summary>
/// Enum to denote animation states.
/// </summary>
public enum AnimState {
    Idle,            Walking,      Jumping,    LoadingArrow,
    SwitchingArrows, DrawingArrow, FullyDrawn, Shooting,
    Crouching,       TakeDamage,   NULL
}


[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour {
    [SerializeField] private Animator _handAnimator;
    [SerializeField] private Animator _bowAnimator;
    public Animator handAnimator => _handAnimator;
    public Animator bowAnimator  => _bowAnimator;
    [Header("Randomized Hurt Animations")]
    [SerializeField] private Animation[] _handHurtAnims;
    [SerializeField] private Animation[] _bowHurtAnims;

    private Character _character;
    private AnimatorOverrideController _handOverride;
    private AnimatorOverrideController _bowOverride;

    // debugging code
    /*
    private AnimState _currentState;
    private readonly string[] _animStrs = {
        "Idle", "Walking", "Jumping", "LoadingArrow", 
        "SwitchingArrows", "DrawingArrow", "FullyDrawn", 
        "Shooting", "NULL"
    };
    */

    private void Start() {
        _character = GetComponent<Character>();
        _handOverride = new AnimatorOverrideController(_handAnimator.runtimeAnimatorController);
        _bowOverride = new AnimatorOverrideController(_bowAnimator.runtimeAnimatorController);
    }

/*
    Currently adding:
        ----crouching : bool isCrouching
        fully drawn (holding) (**NEW ANIMATION**)
        ----jumping (check for not being grounded)
        ----take damage (set a trigger for this)
*/
    private void Update() {
        _SetAllFloats("BowCharge", _character.attackCharge);

        // set draw animation to sync with charge values
        if (_character.attackCharge > 0f)
            _SetAllFloats("DrawSpeed", _character.attackCharge / 100f);
        // set movement based animations
        _SetAllBools("IsGrounded", (_character.jumpCD <= 0f));
        _SetAllBools("IsCrouching", _character.isCrouching);
        _SetAllBools("IsMoving", Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f);
        
        // trigger fire animation
        if (_character.attackCD > 0)
            _SetAllTriggers("Fire");
        //_Debug_PrintAnim(_bowAnimator);
    }

    /// <summary>
    /// Public mutator that triggers the TakeDamageAnimation state
    /// </summary>
    public void TriggerDamageAnim() {
        /*
            TODO:
                Set if statements to prevent this animation from being called if 
                the player has another animation playing that has a "higher"
                priority.
        */
        //int damageIndex = Random.Range(0, _handHurtAnims.Length);
        //set clips using the AnimatorOverrideController
        //_handOverride.Anim

        //_SetAllTriggers("Damage");
    }

    private void _Debug_PrintAnim(Animator animator) {
        try {
            print("currentAnim: " + animator.GetCurrentAnimatorClipInfo(0)?[0].clip.name);
        } catch (System.IndexOutOfRangeException) {
            print("currentAnim: N/A");
        }
    }

    //functions below aid in cleaning up excessive lines of code due 
    //to seperate animators
    private void _SetAllBools(string name, bool value) {
        bowAnimator.SetBool(name, value);
        handAnimator.SetBool(name, value);
    }
    private void _SetAllFloats(string name, float value) {
        bowAnimator.SetFloat(name, value);
        handAnimator.SetFloat(name, value);
    }
    private void _SetAllTriggers(string name) {
        bowAnimator.SetTrigger(name);
        handAnimator.SetTrigger(name);
    }
}
