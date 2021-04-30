/*
Author: Christian Mullins
Date: 02/15/2021
Summary: The class called from the Switch to open the Door's GameObject.
*/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Door : MonoBehaviour
{
    [SerializeField]
    protected bool _isModular;
    [Range(1f, 50f)]
    public float openSpeed = 10f;
    [HideInInspector]
    public List<Switch> mySwitches;

    protected Transform _moveTo;
    protected Mesh _myMesh;
    //By Warren
    //Edit to change whether color or material is being affected.
    private Color _isOn = Color.green;
    private Color _isOff = Color.red;
    // _rend converted to array to accomodate modular assets
    private List<MeshRenderer> _rendList = new List<MeshRenderer>();

    protected virtual void Start()
    {
        _moveTo = transform.GetChild(0);
        try {
            //By Warren
            if (_isModular)
                _rendList = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
            UpdateColor(true);
            IsAllSwitchesFlipped();
        } catch (MissingComponentException ) {
            throw new MissingComponentException("If this is a modular piece select 'isModular'.");
        }
    }

    /// <summary>
    /// Iterate through every Switch to check if they are all flipped.
    /// </summary>
    /// <returns>True if all switches are flipped.</returns>
    public bool IsAllSwitchesFlipped()
    {
        foreach (var s in mySwitches)
        {
            if (!s.isFlipped)
            {
                UpdateColor(false);
                return false;
            }
        }
        UpdateColor(true);
        return true;
    }

    /// <summary>
    /// Coroutine to move the Door outside of using Update().
    /// </summary>
    public virtual IEnumerator Open()
    {
        // detach _moveTo child
        _moveTo.parent = transform.parent;
        do
        {
            Vector3 moveTo = (_moveTo.position - transform.position).normalized;
            transform.Translate(moveTo * openSpeed * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        } while (!_IsPositionApproximateTo(_moveTo.position));
        // re-attach _moveTo child
        _moveTo.parent = transform;
        yield return null;
    }


    /// <summary>
    /// Function to call when changing color or material.
    /// Edit to change whether color or material is being affected.
    /// By Warren
    /// </summary>
    public void UpdateColor(bool isActive)
    {
        Color change = (isActive) ? _isOn : _isOff;
        if (_isModular)
            foreach (var r in _rendList)
                r.material.SetColor("_Color", change);
        else
            GetComponent<MeshRenderer>().material.SetColor("_Color", change); // error here 

    }

    /// <summary>
    /// Draws in editor space to aid in Level Design
    /// </summary>
    protected virtual void OnDrawGizmosSelected()
    {
        // get and set values of _moveTo
        _moveTo = transform.GetChild(0);
        Gizmos.color = Color.red;
        _myMesh = GetComponent<MeshFilter>().sharedMesh;
        // draw _moveTo as a wiremesh
        Gizmos.DrawWireMesh(_myMesh, _moveTo.position, _moveTo.rotation, transform.lossyScale);
    }

    /// <summary>
    /// Substitute this in for "==" between Vector3s, operator is too exact.
    /// </summary>
    /// <param name="compareTo">Destination position.</param>
    /// <returns>True if the distance is almost 0f.</returns>
    protected bool _IsPositionApproximateTo(Vector3 compareTo)
    {
        if (Vector3.Distance(transform.position, compareTo) < 0.25f)
            return true;

        return false;
    }
}