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
using UnityEngine.Animations;
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
    Crouching,       TakeDamage,   NULL
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
    // animator overriding variables
    private Character _character;
    private Dictionary<int, string> _animHashTable;
    private static int _curHurtAnimationClip = 1;
    private AnimatorOverrideController _handOverride;
    private AnimatorOverrideController _bowOverride;
    private Controls _controls; //By Warren

    // properties
    public bool isDamageAnimPlaying { get {
        AnimatorClipInfo[] c = _bowAnimator.GetNextAnimatorClipInfo(0);
        return (c.Length > 0 && c[0].clip.name.StartsWith("Hurt"));
    } }
    private float _jumpMotion => Mathf.Clamp(_character.getVelocity.y / _character.totalJumpPower, 0f, _character.totalJumpPower);

    private void Awake()
    {
        _controls = new Controls(); //By Warren
        //_controls.Player.Jump.performed += ctx => Jump();
    }

    private void OnEnable()
    {
        _controls.Enable(); //By Warren
    }

    private void OnDisable()
    {
        _controls.Disable(); //By Warren
    }

    
    private void Start() {
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
    }

    private void Update() {
        if (isDamageAnimPlaying) {
            //_bowAnimator.WriteDefaultValues();
            //_handAnimator.WriteDefaultValues();
            return;
        }

        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Q))
            TriggerDamageAnim();
        #endif
        
        //By Warren from Dapper Dino YT Tutorial Referenced above
        var movementInput = _controls.Player.Movement.ReadValue<Vector2>();

        _SetAllFloats("BowCharge", _character.attackCharge);

        // set draw animation to sync with charge values
        if (_character.attackCharge > 0f && !isDamageAnimPlaying)
            _SetAllFloats("DrawSpeed", _character.attackCharge / 100f);
        else
            _SetAllFloats("DrawSpeed", 0f);
        // set movement based animations
        _SetAllBools("IsGrounded", (_character.jumpCD <= 0f));
        _SetAllBools("IsCrouching", _character.isCrouching);
        _SetAllBools("IsMoving", movementInput != Vector2.zero);
        _SetAllFloats("JumpMotion", Mathf.Abs(_jumpMotion - 1f));

        // trigger fire animation
        if (_character.attackCD > 0)
            _SetAllTriggers("Fire");
    }

    public bool AreAnimatorsEqual() {
        //make copies of both animators
        Animator copyHandAnim = _handAnimator;
        Animator copyBowAnim  = _bowAnimator;
        //get their overrides
        var copyHandOverride = new AnimatorOverrideController(copyHandAnim.runtimeAnimatorController);
        var copyBowOverride  = new AnimatorOverrideController(copyBowAnim.runtimeAnimatorController);
        var handOverrideList = new AnimationClipOverrides(copyHandOverride.overridesCount);
        var bowOverrideList  = new AnimationClipOverrides(copyBowOverride.overridesCount);
        copyHandOverride.GetOverrides(handOverrideList);
        copyBowOverride.GetOverrides(bowOverrideList);
        //override all clips to be empty or null
        var emptyAnimClip = new KeyValuePair<AnimationClip, AnimationClip>(new AnimationClip(), new AnimationClip());
        for (int i = 0; i < handOverrideList.Count; ++i) {
            handOverrideList[i] = emptyAnimClip;
            bowOverrideList[i]  = emptyAnimClip;
        }
        copyHandOverride.ApplyOverrides(handOverrideList);
        copyBowOverride.ApplyOverrides(bowOverrideList);
        return copyHandAnim.Equals(copyBowAnim);
    }

    /// <summary>
    /// Public mutator that triggers the TakeDamageAnimation state
    /// </summary>
    public void TriggerDamageAnim() {
        int hurtIndex = Random.Range(0, _handHurtAnims.Length);
        // set clips using the AnimatorOverrideController
        string prefix = "Hurt" + _curHurtAnimationClip + "_";

        _bowOverride[prefix + "BowAnim"] = _bowHurtAnims[hurtIndex];
        _handOverride[prefix + "HandAnim"] = _handHurtAnims[hurtIndex];
        _curHurtAnimationClip = hurtIndex + 1;
        _SetAllTriggers("Damage");
        StartCoroutine(_BlockFireAnims());
    }

    private IEnumerator _BlockFireAnims() {
        _SetAllBools("IsDamageAnim", true);
        yield return new WaitWhile(delegate() { return isDamageAnimPlaying; } );
        _SetAllBools("IsDamageAnim", false);
    }

    private void _Debug_PrintAnim() {
        try {
            int hash = Animator.StringToHash(_bowAnimator.GetNextAnimatorClipInfo(0)[0].clip.name);
            
            print("currentAnim: " + _animHashTable[hash]);
        } catch (System.IndexOutOfRangeException) {
            print("currentAnim: N/A");
        }
    }

    private string _GetCurrentAnimatorStateName() {
        AnimatorStateInfo info = _bowAnimator.GetCurrentAnimatorStateInfo(0);

        if (_animHashTable.TryGetValue(info.shortNameHash, out string stateStr)) {
            return stateStr;
        }
        Debug.LogWarning("Unknown animator state name found.");
        return string.Empty;
    }

    //functions below aid in cleaning up excessive lines of code due 
    //to seperate animators
    private void _SetAllBools(string name, bool value) {
        int hash = Animator.StringToHash(name);
        _bowAnimator.SetBool(hash, value);
        _handAnimator.SetBool(hash, value);
    }
    private void _SetAllFloats(string name, float value) {
        int hash = Animator.StringToHash(name);
        _bowAnimator.SetFloat(hash, value);
        _handAnimator.SetFloat(hash, value);
    }
    private void _SetAllTriggers(string name) {
        int hash = Animator.StringToHash(name);
        _bowAnimator.SetTrigger(hash);
        _handAnimator.SetTrigger(hash);
    }
}

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