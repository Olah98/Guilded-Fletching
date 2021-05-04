/*
Author: Christian Mullins
Date: 02/04/2021
Summary: The bramble arrow that pauses an object in place.
*/
using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

/// <summary>
/// Interface for any object that can be Brambled.
/// </summary>
public interface IBrambleable {
    /// <summary>
    /// Make a GameObject brambleable through the implementation of this function.
    /// (IBrambleable interface function)
    /// </summary>
    /// <param name="enabled">Bool for settings the state of the object.</param>
    void Bramble(in bool enabled);
}

public class BrambleArrow : BaseArrow {
    public float bindTime => _bindTime;
    [SerializeField]
    private float _bindTime = 3f;
    private GameObject _boundObj;

    private static Dictionary<int, BrambledNode> _brambledDict = new Dictionary<int, BrambledNode>();
    private BrambledNode _myNode;
    // internal class to handle dictionary
    private class BrambledNode {
        public int instanceID => _instanceID;
        public float timer    => _timer;
        public bool isTimeUp  => (_timer <= 0f);

        private int _instanceID;
        private Object _myObject;
        private float _timer;

        public BrambledNode(GameObject newObject, float newTime) {
            _instanceID = newObject.GetInstanceID();
            _myObject = newObject.GetComponent(typeof(IBrambleable));
            _timer = newTime;

            if (_myObject == null)
                _myObject = newObject.GetComponent<Rigidbody>();
            if (_myObject == null)
                throw new NullReferenceException("No IBrambleable interface found on this object.");
        }

        public void DecrementTimer() {
            _timer -= Time.deltaTime;
            if (_timer <= 0f) {
                if (_myObject is IBrambleable) {
                    IBrambleable iB = (IBrambleable)_myObject;
                    iB.Bramble(false);
                }
                else if (_myObject is Rigidbody)  {
                    Rigidbody myRB = (Rigidbody)_myObject;
                    myRB.isKinematic = false;
                }
                _brambledDict.Remove(_instanceID);
            }
        }

        public void ResetTime(float newTimer) {
            _timer = newTimer;
        }
    } // end BrambledNode

    protected override void Awake() {
        _myNode = null;
        base.Awake();
    }

    private void Update() {
        if (_myNode == null) return;

        if (!_myNode.isTimeUp) 
            _myNode.DecrementTimer();
        else
            _myNode = null;

    }

    /// <summary>
    /// Handle memory of Node in case it still exists.
    /// </summary>
    private void OnDestroy() {
        if (_myNode != null) {
            _brambledDict.Remove(_myNode.instanceID);
            _myNode = null;
        }
    }
/*
    /// <summary>
    /// Check for appropriate collision, if so execute teleportation.
    /// </summary>
    /// <param name="other">The object the arrow is hitting.</param>
    protected override void OnCollisionEnter(Collision other) {
        if ((other.transform.tag == "Stoppable" || other.transform.tag == "Enemy") && !isAbilityUsed) {
            Bind(other.gameObject);
            isAbilityUsed = true;
        }
    }
*/
    public void Use(GameObject other)
    {
        //Debug.Log("BRAMBLE USING: " + other.name);
        if ((other.transform.tag == "Stoppable" || other.transform.tag == "Enemy")
            && !isAbilityUsed)
        {
            Bind(other.gameObject);
            isAbilityUsed = true;
        }

        if (other.transform.name == "SwingingPendulum")
        {
            other.GetComponent<SwingingPendulum>().Bind(bindTime);
        }
    }

    /// <summary>
    /// Freeze object in space for a limited time.
    /// </summary>
    /// <param name="binding">Object that will be bound.</param>
    private void Bind(GameObject binding) {
        base.Hit();
        _boundObj = binding;
        if (_brambledDict.ContainsKey(binding.GetInstanceID())) {
            _brambledDict[binding.GetInstanceID()].ResetTime(bindTime);
        }
        else {
            //add to dict
            var newNode = new BrambledNode(binding, _bindTime);
            _myNode = newNode;
            _brambledDict.Add(binding.GetInstanceID(), newNode);
            //extract interface
            var inter = (IBrambleable)binding.GetComponents(typeof(Component)).First(c => c is IBrambleable);
            if (inter != null) {
                inter.Bramble(true);
            }
            else if (TryGetComponent<Rigidbody>(out var rb)) {
                rb.isKinematic = true;
            }
        }
    }
}
