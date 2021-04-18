/*
Author: Christian Mullins
Date: 4/16/2021
Summary:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingObject : HingedObject {
    [Header("Pause values")]
    [HideInInspector]
    public float stopTimer = 3f;
    private bool _isStopped;
    private bool _movingForward;
    private float _deltaAngle;

    private float _timer;

    protected override void Start() {
        base.Start();
        _timer = 0f;
        _deltaAngle = 0f;
        _isStopped = false;
        _movingForward = true;
        Destroy(_moveTo.gameObject);
    }

    private void FixedUpdate() {
        // swinging physics
        if (!_isStopped) {
            float torque = Time.fixedDeltaTime * openSpeed;
            if (!_movingForward) torque = -torque;
            transform.RotateAround(_anchorPoint, currentRotation, torque);
            _deltaAngle += torque;

            if (Mathf.Abs(_deltaAngle) > _angleChange) {
                _movingForward = !_movingForward;
                _deltaAngle = 0f;
            }
        }
        if (_timer > 0) _timer -= Time.fixedDeltaTime;
    }

    public override IEnumerator Open() {
        return StopFor(stopTimer);
    }

    public IEnumerator StopFor(float time) {
        if (_isStopped) yield break;

        _isStopped = true;
        _timer = time;
        yield return new WaitUntil(() => { return _timer <= 0; });
        _isStopped = false;
        mySwitches.ForEach(s => { s.ResetSwitch(); });
        IsAllSwitchesFlipped();
        yield return null;
    }
}
