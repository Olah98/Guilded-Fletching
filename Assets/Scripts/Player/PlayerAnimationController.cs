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
Referencing: DapperDino's UI Tutorial https://www.youtube.com/watch?v=Ikt5T-v2ZrM
*/
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

/// <summary>
/// Enum to denote animation states.
/// </summary>
public enum AnimState {
    Idle,            Walking,      Jumping,    LoadingArrow,
    SwitchingArrows, DrawingArrow, FullyDrawn, Shooting,
    Crouching,       TakeDamage,   Dead,       NULL
}
public enum SwapPhase {
    NotStarted = -1, Started, HandOffScreen, HandBackOnScreen,
    Ended = -1
}

[RequireComponent(typeof(Character))]
public class PlayerAnimationController : MonoBehaviour {
    public Animator handAnimator   => _handAnimator;
    public Animator bowAnimator    => _bowAnimator;
    [SerializeField] private Animator _handAnimator;
    [SerializeField] private Animator _bowAnimator;
    [Header("Randomized Hurt Animations")]
    [SerializeField] private AnimationClip[] _handHurtAnims;
    [SerializeField] private AnimationClip[] _bowHurtAnims;

    public readonly Dictionary<string, int> playerAnimHashTable 
        = new Dictionary<string, int> {
            { "DrawSpeed",   Animator.StringToHash("DrawSpeed")   },
            { "IsGrounded",  Animator.StringToHash("IsGrounded")  },
            { "IsCrouching", Animator.StringToHash("IsCrouching") },
            { "IsMoving",    Animator.StringToHash("IsMoving")    },
            { "IsDead",      Animator.StringToHash("IsDead")      },
            { "JumpMotion",  Animator.StringToHash("JumpMotion")  },
            { "Fire",        Animator.StringToHash("Fire")        },
            { "Damage",      Animator.StringToHash("Damage")      },
            { "SwitchArrow", Animator.StringToHash("SwitchArrow") },
    };
    // animator overriding variables
    private Character _character;
    private Dictionary<int, string> _animHashTable;
    private static int _curHurtAnimationClip = 1;
    private AnimatorOverrideController _handOverride;
    private AnimatorOverrideController _bowOverride;
    private Controls _controls;
    private float _swapArrowAnimTime;

    // properties
    public bool  blockInputForAnim  { get; private set; }
    public SwapPhase arrowSwapPhase { get; private set; }
    private float _jumpMotion => Mathf.Clamp(_character.getVelocity.y / _character.totalJumpPower, 0f, _character.totalJumpPower);

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }
    
    private void Start() {
        blockInputForAnim = false;
        //store ALL possible animations in _animHashTable
        _animHashTable = new Dictionary<int, string>();
        var allAnims = new List<AnimationClip>(_bowAnimator.runtimeAnimatorController.animationClips);
        allAnims.AddRange(_bowHurtAnims);
        foreach (var c in allAnims) {
            if (!_animHashTable.ContainsKey(Animator.StringToHash(c.name))) {
                _animHashTable.Add(Animator.StringToHash(c.name), c.name.TrimEnd("_BowAnim".ToCharArray()));
            }
        }
        _character = GetComponent<Character>();
        _handOverride = new AnimatorOverrideController(_handAnimator.runtimeAnimatorController);
        _handAnimator.runtimeAnimatorController = _handOverride;
        _bowOverride = new AnimatorOverrideController(_bowAnimator.runtimeAnimatorController);
        _bowAnimator.runtimeAnimatorController = _bowOverride;

        _SetAllBools(playerAnimHashTable["IsDead"], false);
        // grab time for arrow swap animation from the current RuntimeAnimatorController
        _swapArrowAnimTime = new List<AnimationClip>(_bowOverride.animationClips).Find(ac => ac.name.StartsWith("Swap")).length;
    }

    private void Update() {
        //if (blockInputForAnim) return;
        /*
        // code used  for demostration purposes only
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q)) {
            _character.TakeDamage(1);
        }
        #endif
        */
        //By Warren from Dapper Dino YT Tutorial Referenced above
        var movementInput = _controls.Player.Movement.ReadValue<Vector2>();

        // set draw animation to sync with charge values
        if (_character.attackCharge > 1f)
            _SetAllFloats(playerAnimHashTable["DrawSpeed"], _character.attackCharge / 100f);
        else
            _SetAllFloats(playerAnimHashTable["DrawSpeed"], 0f);

        // set movement based animations
        _SetAllBools(playerAnimHashTable["IsGrounded"], (_character.jumpCD <= 0f));
        _SetAllBools(playerAnimHashTable["IsCrouching"], _character.isCrouching);
        _SetAllBools(playerAnimHashTable["IsMoving"], movementInput != Vector2.zero);
        _SetAllFloats(playerAnimHashTable["JumpMotion"], Mathf.Abs(_jumpMotion - 1f));

        // trigger fire animation (using a bool)
        //_SetAllBools(playerAnimHashTable["Fire"], (_character.attackCD >= 0.1f));
    }

    #region TriggerAnimationFunctions
    /// <summary>
    /// Public mutator that triggers the TakeDamageAnimation state
    /// </summary>
    public void TriggerDamageAnim() {
        int hurtIndex = Random.Range(0, 3);
        // set clips using the AnimatorOverrideController
        string prefix = "Hurt" + _curHurtAnimationClip + "_";
        _bowOverride[prefix + "BowAnim"] = _bowHurtAnims[hurtIndex];
        _handOverride[prefix + "HandAnim"] = _handHurtAnims[hurtIndex];
        _curHurtAnimationClip = hurtIndex;
        _SetAllTriggers(playerAnimHashTable["Damage"]);

        StartCoroutine(_WaitForAnim(AnimState.TakeDamage));
        _controls.Player.Fire.Disable();
        _controls.Player.Zoom.Disable();
    }

    /// <summary>
    /// Execute relavent animations for when the player dies.
    /// </summary>
    public void TriggerDeathAnim(GameObject inHandArrowGO) {
        _SetAllBools(playerAnimHashTable["IsDead"], true);
        _controls.Dispose();
        // drop bow and arrow
        var newBowRB = _bowAnimator.gameObject.AddComponent<Rigidbody>();
        var newHandArrowRB = inHandArrowGO.AddComponent<Rigidbody>();
        newBowRB.isKinematic = false;
        newBowRB.useGravity = true;
        newHandArrowRB.isKinematic = false;
        newHandArrowRB.useGravity = true;
        inHandArrowGO.transform.parent = null;
    }

    /// <summary>
    /// Set appropriate actions to trigger the animation for arrow swapping
    /// </summary>
    /// <param name="arrowStr"></param>
    public void TriggerArrowSwapAnim(in string arrowStr) {
        // trigger animation
        _SetAllTriggers(playerAnimHashTable["SwitchArrow"]);
        StartCoroutine(_WaitForAnim(AnimState.SwitchingArrows));
        int arrowIndex;
        // char comparisons are faster (preferable in animation)
        switch (arrowStr[0]) {
            case 'S': arrowIndex = 0; break;
            case 'B': arrowIndex = 1; break;
            case 'W': arrowIndex = 2; break;
            case 'A': arrowIndex = 3; break;
            default: throw new IndexOutOfRangeException(arrowStr + ": is not a valid arrow.");
        }
        StartCoroutine(_character.SyncArrowSwapWithAnim(arrowIndex));
    }

    /// <summary>
    /// Set the current phase to the argument phase.
    /// </summary>
    /// <param name="phase">New phase to be set.</param>
    public void SetSwapPhase(SwapPhase phase) {
        arrowSwapPhase = phase;
    }

    /// <summary>
    /// Execute relavent animations for when the player reloads
    /// </summary>
    public void TriggerReloadAnim() {
        if (_bowAnimator.GetBool(playerAnimHashTable["Fire"])) return;
        _controls.Player.Fire.Disable();
        _controls.Player.Zoom.Disable();
        _SetAllBools(playerAnimHashTable["Fire"], true);
        StartCoroutine(_WaitForAnim(AnimState.LoadingArrow));
        StartCoroutine(_character.SyncHideArrowWithAnim());
    }

    /// <summary>
    /// Coroutine to handle logic of input blocking when the player is executing a
    /// high priority animation.
    /// </summary>
    private IEnumerator _WaitForAnim(AnimState waitForState) {
        float waitAnimLength;
        switch (waitForState) {
            case AnimState.TakeDamage:
                waitAnimLength = _bowHurtAnims[_curHurtAnimationClip].length;
                break;
            case AnimState.LoadingArrow:
            case AnimState.SwitchingArrows:
                waitAnimLength = _swapArrowAnimTime;
                break;
            default: 
                throw new IndexOutOfRangeException(waitForState + ": is not a valid animation to wait for.");
        }
        blockInputForAnim = true;
        yield return new WaitForSeconds(waitAnimLength);
        if (!waitForState.Equals(AnimState.TakeDamage)) {
            yield return new WaitUntil(delegate() { return arrowSwapPhase.Equals(SwapPhase.HandBackOnScreen); });
        }
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(delegate() { return !_character.firePressed; } );
        _SetAllBools(playerAnimHashTable["Fire"], false);
        _character.attackCD = 0f;
        _character.chargeRate = 0f;
        _controls.Player.Fire.Enable();
        _controls.Player.Zoom.Enable();
        blockInputForAnim = false;
    }
    #endregion

    #region DebugStuff
    /// <summary>
    /// Debugging function to print the current animation that's being played.
    /// </summary>
    private void _PrintAnim() {
        try {
            int hash = Animator.StringToHash(_bowAnimator.GetNextAnimatorClipInfo(0)[0].clip.name);
            print("currentAnim: " + _animHashTable[hash]);
        } catch (System.IndexOutOfRangeException) {
            print("currentAnim: N/A");
        }
    }

    /// <summary>
    /// Get the current animator state for debugging purposes.
    /// </summary>
    /// <returns>Animator state string for debugging output.</returns>
    private string _GetCurrentAnimatorStateName() {
        AnimatorStateInfo info = _bowAnimator.GetCurrentAnimatorStateInfo(0);

        if (_animHashTable.TryGetValue(info.shortNameHash, out string stateStr)) {
            return stateStr;
        }
        throw new NullReferenceException("Unknown AnimatorState.");
    }
    #endregion

    //functions below aid in cleaning up excessive lines of code due 
    //to seperate animators
    private void _SetAllBools(int hash, bool value) {
        _bowAnimator.SetBool(hash, value);
        _handAnimator.SetBool(hash, value);
    }
    private void _SetAllFloats(int hash, float value) {
        _bowAnimator.SetFloat(hash, value);
        _handAnimator.SetFloat(hash, value);
    }
    private void _SetAllTriggers(int hash) {
        _bowAnimator.SetTrigger(hash);
        _handAnimator.SetTrigger(hash);
    }
}
// wrapper class for overriding animation clips
public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>> {
    public AnimationClipOverrides(int capacity) : base(capacity) {}
    public AnimationClip this[string name] {
        get { return this.Find(x => x.Key.name.Equals(name)).Value; }
        set {
            int index = this.FindIndex(x => x.Key.name.Equals(name));
            // -1 is returned if the value is not found
            if (index != -1)
                this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
        }
    }
}