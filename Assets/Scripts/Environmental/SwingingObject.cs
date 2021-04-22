/*
Author: Christian Mullins
Date: 4/16/2021
Summary:
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingingObject : HingedObject, IBrambleable {
    private bool _isStopped;
    private bool _movingForward;
    private float _deltaAngle;
    [System.Obsolete]
    private float _timer;

    protected override void Start() {
        base.Start();
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
        //if (_timer > 0) _timer -= Time.fixedDeltaTime;
    }

    public void Bramble(in bool enabled) {
        _isStopped = enabled;
        if (!enabled) {
            mySwitches.ForEach(s => { s.ResetSwitch(); });
            IsAllSwitchesFlipped();
        }
    }
/*
    [System.Obsolete]
    public override IEnumerator Open() {
        return StopFor(stopTimer);
    }
*/
    [System.Obsolete]
    public IEnumerator StopFor(float time) {
        _timer = time;
        if (_isStopped) yield break;

        _isStopped = true;
        yield return new WaitUntil(() => { return _timer <= 0; });
        _isStopped = false;
        mySwitches.ForEach(s => { s.ResetSwitch(); });
        IsAllSwitchesFlipped();
    }
}
