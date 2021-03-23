/*
Author: Christian Mullins & Warren Rose II
Date: 03/23/2021
Summary: Based on combining Door and MovingPlatform. Platform halts after time.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CountingPlatform : MonoBehaviour {
    //Edit to change whether color or material is being affected.
    //public Material m_isOn;
    //public Material m_isOff;
    private Color isOn = Color.green;
    private Color isOff = Color.red;
    private MeshRenderer rend;


    public float speed;
    public bool isBrambled;
    [Header("Waypoints to travel between")]
    public int maxRounds;
    private int curRounds;

    private LinkedList<Transform> movePoints;
    private LinkedListNode<Transform> curNode;
    //velocity tracker
    public Vector3 GetVelocity { get { return velocity; } }
    private Vector3 velocity = Vector3.zero;
    private Vector3 posLastFrame;

    [HideInInspector]
    public List<SwitchPlatform> mySwitches;

    void Awake()
    {
        curRounds = maxRounds;
    }

    void Start()
    {
        var waypoints = from point in GetComponentsInChildren<Transform>()
                        where point.tag == "Waypoint"
                        select point;
        //only store children with the "Waypoint" tag in a LL
        movePoints = new LinkedList<Transform>(waypoints);
        curNode = movePoints.First;
        isBrambled = false;
        posLastFrame = transform.position;
        //unparent waypoints
        foreach (var w in waypoints) w.parent = transform.parent;

        //By Warren
        rend = GetComponent<MeshRenderer>();
        UpdateColor(true);
        IsAllSwitchesFlipped();
    }

    void FixedUpdate()
    {
        if (!isBrambled && IsAllSwitchesFlipped())
        {
            //move platform
            Vector3 moveTo = (curNode.Value.position - transform.position).normalized;
            transform.position += moveTo * speed * Time.fixedDeltaTime;
        }

        velocity = (transform.position - posLastFrame) / Time.fixedDeltaTime;
        posLastFrame = transform.position;
    }

    /// <summary>
    /// Waypoints are checked through trigger system.
    /// </summary>
    /// <param name="other">Colliding object.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Waypoint" && other.transform == curNode.Value)
        {
            //if the next node is null, return to the first in loop
            curNode = curNode.Next ?? movePoints.First;
            //Debug.Log(curRounds);
            if (curRounds > 0)
            {
                curRounds--;

                if (curRounds == 0)
                {
                    ResetAllSwitches();
                }
            }
        }
    }

    /// <summary>
    /// Iterate through every Switch to check if they are all flipped.
    /// </summary>
    /// <returns>True if all switches are flipped.</returns>
    public bool IsAllSwitchesFlipped() {
        foreach (var s in mySwitches) {
            if (!s.isFlipped)
            {
                UpdateColor(false);
                return false;
            }
        }
        UpdateColor(true);
        if (curRounds == 0)
        {
            curRounds = maxRounds;
        }
        return true;
    }

    /// <summary>
    /// Iterate through every Switch to reset them.
    /// </summary>
    public void ResetAllSwitches()
    {
        foreach (var s in mySwitches)
        {
            if (s.isFlipped)
            {
                s.ResetSwitch();
            }
        }
        UpdateColor(false);
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
        {
            change = isOn;
            //m_change = m_isOn;
        }
        else
        {
            change = isOff;
            //m_change = m_isOff;
        }
        if (rend != null)
        {
            rend.material.SetColor("_Color", change);
            //rend.material = m_change;
        }
    }
}