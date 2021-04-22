/*
Author: Christian Mullins
Date: 4/9/2021
Summary: Utility script that overrides parent-child inheritance in Unity
    in favor of inspector settings of this script.
*/

using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(ParentConstraint))]
public class IgnoreParentTransforms : MonoBehaviour {
    public bool isActive => _isActive;
    [Header("DO NOT EDIT PARENTCONSTRAINT!")]
    [SerializeField] private bool _isActive;

    private ParentConstraint _myPC;
    private Transform _camParent;
    private Transform _newParent;
    private Vector3 _originLocalPos;

    private void Start() {
        _isActive = false;
        _camParent  = Camera.main.transform;
        _originLocalPos = transform.localPosition;
        _myPC = GetComponent<ParentConstraint>();
        _newParent = Instantiate(new GameObject("BowParentObject"), _camParent.parent.parent).transform;
        SetParentInstance(_isActive, false);
    }

    /// <summary>
    /// Swap parents to ignore scaling from crouching.
    /// </summary>
    /// <param name="enabled">Set the parenting instance.</param>
    /// <param name="crouching">Check if the player is trying to crouch or not.</param>
    public void SetParentInstance(in bool enabled, in bool crouching) {
        // prevent multiple calls in Update()
        if (enabled == _isActive) return; // originally uncommented

        transform.parent = (enabled) ? _newParent : _camParent;
        
        _isActive = enabled;
        _myPC.constraintActive = enabled;
        
        // snap position to the appropriate coordinates if we're done crouching
        if (!enabled && !crouching) {
            transform.parent = _camParent;
            transform.localPosition = _originLocalPos;
            transform.localScale = Vector3.one;
        }

    }
}
