/*
Author: Christian Mullins
Date: 4/11/2021
Summary: 
*/
using System.Collections;
using UnityEngine;

public class MovingBridge : HingedObject {
    public bool isStopped;

    private bool _moveForward;
    private Vector3 _startPos;

    protected override void Start() {
        _moveForward = true;
        _startPos = transform.position;
        base.Start();
    }

    protected void FixedUpdate() {
        if (!isStopped) {
            Vector3 _rotDir = (_moveForward) ? transform.right : -transform.right;
            transform.RotateAround(_anchorPoint, _rotDir, openSpeed * Time.fixedDeltaTime);
            
            if (_IsPositionApproximateTo((_moveForward) ? _moveTo.position : _startPos))
                _moveForward = !_moveForward;
        }
    }

    public IEnumerator StopFor(float timer) {
        isStopped = true;
        yield return new WaitForSeconds(timer);
        isStopped = false;
    }
}
