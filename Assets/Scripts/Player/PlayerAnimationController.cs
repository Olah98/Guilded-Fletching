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
    private Animator _handAnimator;
    //private Animator bowAnimator; // implement later
    private Character _character;
    // animation state strings                           priority
    private const string idleStr    = "Idle";            //low
    private const string walkingStr = "Walking";         //mid
    private const string jumpingStr = "Jumping";         //high
    private const string loadingStr = "LoadingArrow";    //mid
    private const string switchStr  = "SwitchingArrows"; //low
    private const string drawingStr = "DrawingArrow";    //high
    private const string fullStr    = "FullyDrawn";      //high
    private const string shootStr   = "Shooting";        //high

    private void Start() {
        _character = GetComponent<Character>();
        _handAnimator = GetComponent<Animator>();
        //_handAnimator.SetBool("Idle", true);
        //bowAnimator.SetBool("Idle", true);
    }

    /// <summary>
    /// Set an animation to tru based on the parameter enum.
    /// </summary>
    /// <param name="state">Animation state to change to.</param>
    public void SetAnimation(AnimState state, bool setTo) {
        string animStr = string.Empty;
        switch (state) {
            case AnimState.Idle:            animStr = idleStr;    break;
            case AnimState.Walking:         animStr = walkingStr; break;
            case AnimState.Jumping:         animStr = jumpingStr; break;
            case AnimState.LoadingArrow:    animStr = loadingStr; break;
            case AnimState.SwitchingArrows: animStr = switchStr;  break;
            case AnimState.DrawingArrow:    animStr = drawingStr; break;
            case AnimState.FullyDrawn:      animStr = fullStr;    break;
            case AnimState.Shooting:        animStr = shootStr;   break;
        }
        //_handAnimator.SetBool(animStr, setTo);
        //print("PlayerAnimating: " + state);
    }

    /// <summary>
    /// Set every animation state as false.
    /// </summary>
    public void SetAllAnimationsFalse() {
        _handAnimator.SetBool(idleStr,    false);
        _handAnimator.SetBool(walkingStr, false);
        _handAnimator.SetBool(jumpingStr, false);
        _handAnimator.SetBool(loadingStr, false);
        _handAnimator.SetBool(switchStr,  false);
        _handAnimator.SetBool(drawingStr, false);
        _handAnimator.SetBool(fullStr,    false);
        _handAnimator.SetBool(shootStr,   false);
    }

    /// <summary>
    /// Return animation priority of a given enum.
    /// </summary>
    /// <param name="state">The priority we'd like returned.</param>
    /// <returns>Priority of parameter enum.</returns>
    public int GetPriority(AnimState state) {
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
    public AnimState ComparePriority(AnimState state1, AnimState state2) {
        int s1Num = GetPriority(state1);
        int s2Num = GetPriority(state2);
        if (s1Num == s2Num) return AnimState.NULL;

        return (s1Num > s2Num) ? state1 : state2;
    }
}
