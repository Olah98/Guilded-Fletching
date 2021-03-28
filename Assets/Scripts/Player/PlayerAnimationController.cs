/*
Author: Christian Mullins
Date: 03/08/2021
Summary: Controls the flow on animation for the player in it's various states.
    -Idle
    -Walking
    -Jumping (high)
    -Loading Arrow
    -Switching Arrows
    -Pulling string (drawing arrow)
    -Full charge    (FullyDrawn)
    -Release string (shooting)
*/
using UnityEngine;

/// <summary>
/// Enum to denote animation states.
/// </summary>
public enum AnimState {
    Idle,            Walking,      Jumping,    LoadingArrow,
    SwitchingArrows, DrawingArrow, FullyDrawn, Shooting,
    NULL
}

[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour {
    public Animator handAnimator;
    public Animator bowAnimator;

    private Character _character;

    /*
        Presently working animations:
            -Walking
            -DrawingArrow
            -FullyDrawn
            -Shooting
    */

    private void Start() {
        _character = GetComponent<Character>();
    }

    private void Update() {
        _SetAllFloats("BowCharge", _character.attackCharge);

        // set draw animation to sync with charge values
        if (_character.attackCharge > 0f)
            _SetAllFloats("DrawSpeed", _character.attackCharge / 100f);

        // set movement based animations
        _SetAllBools("IsGrounded", (_character.jumpCD <= 0f));
        _SetAllBools("IsMoving", Input.GetAxis("Horizontal") != 0f 
                                        || Input.GetAxis("Vertical") != 0f);
        // trigger fire animation
        if (_character.attackCD == 1f) _SetAllTriggers("Fire");
    }

    /// <summary>
    /// Return animation priority of a given enum.
    /// </summary>
    /// <param name="state">The priority we'd like returned.</param>
    /// <returns>Priority of parameter enum.</returns>
    public static int GetPriority(AnimState state) {
        switch (state) {
            case AnimState.Idle:
            case AnimState.SwitchingArrows: 
                return 0;
            case AnimState.Walking:         
            case AnimState.LoadingArrow:
                return 1;
            case AnimState.Jumping:
            case AnimState.DrawingArrow:
            case AnimState.FullyDrawn:
            case AnimState.Shooting:
                return 2;
        }
        throw new System.IndexOutOfRangeException();
    }

    /// <summary>
    /// Compare two animation states and return the greater priority. If the
    /// priorities are equal then AnimState.NULL.
    /// </summary>
    /// <returns>The greater priority enum.</returns>
    public static AnimState ComparePriority(AnimState state1, AnimState state2) {
        int s1Num = GetPriority(state1);
        int s2Num = GetPriority(state2);
        if (s1Num == s2Num) return AnimState.NULL;

        return (s1Num > s2Num) ? state1 : state2;
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
