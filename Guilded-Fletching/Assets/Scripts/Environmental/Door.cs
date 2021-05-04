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
    private MeshRenderer _rend;

    protected virtual void Start()
    {
        _moveTo = transform.GetChild(0);
        //By Warren
        _rend = GetComponent<MeshRenderer>();
        UpdateColor(true);
        IsAllSwitchesFlipped();
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
        Color change;
        //Material m_change;
        if (isActive)
            change = _isOn;
        else
            change = _isOff;
        if (_rend != null)
            _rend.material.SetColor("_Color", change);
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
        Gizmos.DrawWireMesh(_myMesh, _moveTo.position, _moveTo.rotation,
                            transform.lossyScale);
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