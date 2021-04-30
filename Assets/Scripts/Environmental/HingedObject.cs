/*
Author: Christian Mullins
Date: 03/24/2021
Summary: Class inhereted from Door script to manipulate object by rotation
    rather than by position.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Axis {
    PosX, NegX, PosY, NegY, PosZ, NegZ
}

[Serializable]
public class HingedObject : Door {
    [Range(5f, 355f)]
    [Tooltip("Axis change in degrees (denoted by red wirecube).")]
    [SerializeField] protected float _angleChange = 90f;
    [Header("Pivot values")]
    [Tooltip("Local coordinate of where the object will rotate around.")]
    public Vector3 _anchorPoint;
    //rotation values
    [HideInInspector] public Axis pivotPosition;
    [HideInInspector] public Axis rotationAxis;
    private bool _isOpen;
    protected List<Collider> _colliders = new List<Collider>();

    // properties
    public Vector3 currentRotation  { get {
        return Vector3.RotateTowards(transform.eulerAngles, ToDirection(rotationAxis), Mathf.PI, Time.fixedDeltaTime);
    } }

    protected override void Start() {
        base.Start();
        _isOpen = false;
        _AssignColliderValues();
    }
    
    /// <summary>
    /// Rotate this object open with this classes values.
    /// </summary>
    public override IEnumerator Open() {
        if (_isOpen) yield break; //guard clause

        _isOpen = true;
        _moveTo.parent = transform.parent;
        float totalDegrees = 0f;
        do {
            transform.RotateAround(_anchorPoint, currentRotation, Time.fixedDeltaTime * openSpeed);
            // calculate new rotation
            totalDegrees += Time.fixedDeltaTime * openSpeed;
            yield return new WaitForFixedUpdate();
        } while (totalDegrees < _angleChange);
        Destroy(_moveTo.gameObject);
    }

    #region PivotFunctions
    /// <summary>
    /// Snap anchor point on a specific axis to the extreme bound of the collider.
    /// </summary>
    /// <param name="axis">Enum for pivot axis.</param>
    public void SnapPivotTo(Axis axis) {
        Vector3 newPivot = transform.position;
        switch (axis) {
            case Axis.PosX: newPivot.x = Mathf.Infinity;         break;
            case Axis.NegX: newPivot.x = Mathf.NegativeInfinity; break;
            case Axis.PosY: newPivot.y = Mathf.Infinity;         break;
            case Axis.NegY: newPivot.y = Mathf.NegativeInfinity; break;
            case Axis.PosZ: newPivot.z = Mathf.Infinity;         break;
            case Axis.NegZ: newPivot.z = Mathf.NegativeInfinity; break;
            default: break;
        }
        SnapPivotPos(newPivot);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="checkPos"></param>
    /// <returns></returns>
    public void SnapPivotPos(in Vector3 checkPos) {
        // calculate closest from all colliders in GameObject (including children)
        Vector3 closest = _ClampToBounds(checkPos, _colliders[0].bounds);
        foreach (var c in _colliders) {
            Vector3 tempPoint = _ClampToBounds(checkPos, c.bounds);

            if (tempPoint == checkPos){ 
                closest = checkPos;
                break;
            }
            else if (Vector3.Distance(tempPoint, checkPos) <= Vector3.Distance(closest, checkPos)) {
                closest = tempPoint;
            }
        }
        _anchorPoint = closest;
    }

    /// <summary>
    /// Clamp every axis to be inside the parameter bounds.
    /// </summary>
    /// <param name="clamping">Position</param>
    /// <param name="bounds">Boundary classs</param>
    /// <returns></returns>
    protected Vector3 _ClampToBounds(Vector3 clamping, Bounds bounds) {
        for (int i = 0; i < 3; ++i)
            clamping[i] = Mathf.Clamp(clamping[i], bounds.min[i], bounds.max[i]);
        return clamping;
    }
    #endregion

    #region RotationalFunctions
    /// <summary>
    /// Given a parameter enum, translate to a rotation relative to this
    /// GameObject's position.
    /// </summary>
    /// <param name="axis">Enum to convert.</param>
    /// <returns>Parameter enum to Vector3.</returns>
    public Vector3 ToDirection (Axis axis) {
        switch (axis) {
            case Axis.PosX:   return  transform.right;
            case Axis.NegX:   return -transform.right;
            case Axis.PosY:   return  transform.up;
            case Axis.NegY:   return -transform.up;
            case Axis.PosZ:   return  transform.forward;
            case Axis.NegZ:   return -transform.forward;
            default: 
                throw new IndexOutOfRangeException(axis.ToString()
                    + " is not a valid enum in this context");
        }
    }

    /// <summary>
    /// Output appropriate rotation that matches the parameter enum.
    /// </summary>
    /// <param name="rotation">Enum value of rotation.</param>
    /// <returns>Rotational direction</returns>
    protected Vector3 _AssignRotationTo(Axis rotation) {
        return Vector3.RotateTowards(transform.eulerAngles, ToDirection(rotation), Mathf.PI, 1f);
    }
    #endregion

    /// <summary>
    /// Draws in editor space to aid in Level Design
    /// </summary>
    protected override void OnDrawGizmosSelected() {
        if (_colliders.Count == 0) _AssignColliderValues();
        
        if (transform.childCount > 0) {
            _moveTo = transform.GetChild(0);
            // reset each time to refresh values
            _moveTo.SetPositionAndRotation(transform.position, transform.rotation);
            _myMesh = GetComponent<MeshFilter>().sharedMesh;
            _moveTo.RotateAround(_anchorPoint, currentRotation, _angleChange);

            // output target wiremesh
            Gizmos.color = Color.red;
            Gizmos.DrawWireMesh(_myMesh, _moveTo.position, _moveTo.rotation, transform.localScale);
            // output visualization for the anch or point
            Gizmos.color = Color.green;
            //_anchorPoint = SnapPivotPos(_anchorPoint);
            Gizmos.DrawWireSphere(_anchorPoint, 0.5f);
        }
    }

    /// <summary>
    /// Function to dynamically assign Collider components of this GameObject.
    /// </summary>
    protected void _AssignColliderValues() {
        if (GetComponent<Collider>() != null)
            _colliders.Add(GetComponent<Collider>());
        if (GetComponentsInChildren<Collider>() != null)
            _colliders.AddRange(GetComponentsInChildren<Collider>());
        
        if (_colliders.Count == 0)
            throw new NullReferenceException("No colliders found on this GameObject.");
    }
}