/*
Author: Christian Mullins
Date: 03/24/2021
Summary: Class inhereted from Door script to manipulate how the 
*/
using System.Collections;
using UnityEngine;

public class Bridge : Door {
    [Range(40f, 160f)]
    [Tooltip("Axis change in degrees (denoted by red wirecube).")]
    public float angleChange = 90f;

    public Vector3 targetRotation { get; private set; }

    private bool _isBridgeAtTarget = false;
    private Vector3? _anchorPoint = null;

    protected override void Start() {
        var rotation = transform.localEulerAngles;
        rotation.z += angleChange;
        targetRotation = rotation;
        _anchorPoint = _GetBridgeAnchor();
        base.Start();
    }

    //open by rotation
    public override IEnumerator Open() {
        _moveTo.parent = transform.parent;
        //rotate in opposite direction if the angle is too large
        do {
            transform.RotateAround((Vector3)_anchorPoint, 
                                    transform.right, 
                                    Time.fixedDeltaTime * openSpeed);

            yield return new WaitForFixedUpdate();
        } while (!_IsPositionApproximateTo(_moveTo.position));
        Destroy(_moveTo.gameObject);
        yield return null;
    }

    /// <summary>
    /// Draws in editor space to aid in Level Design
    /// </summary>
    protected override void OnDrawGizmosSelected() {
        if (transform.childCount > 0 && _anchorPoint != null) {
            _moveTo = transform.GetChild(0);
            // reset each time to refresh values
            _moveTo.SetPositionAndRotation(transform.position, 
                                           transform.rotation);
            _myMesh = GetComponent<MeshFilter>().sharedMesh;
            Gizmos.color = Color.red;
        
            //apply rotation to _moveTo and output it as the wiremesh
            _moveTo.RotateAround((Vector3)_anchorPoint, transform.right, angleChange);
        
            // output target wiremesh
            Gizmos.DrawWireMesh(_myMesh, _moveTo.position, _moveTo.rotation,
                                transform.localScale);
        }
        // _anchorPoint must be defined once in this context
        else if (_anchorPoint == null)
            _anchorPoint = _GetBridgeAnchor();
    }

    /// <summary>
    /// Intializing getter of _anchorPoint to maintain DRY code.
    /// </summary>
    /// <returns>Nullable Vector3 to match the _anchorPoint variable.</returns>
    private Vector3? _GetBridgeAnchor() {
        return transform.position + new Vector3(0f, -transform.localScale.y/2f, 0f);
    }
}
