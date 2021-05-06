/*
Author: Warren Rose II
Data: 02/16/2021
Summary: Enables the Level Designers to handle nodes that carry bools and ids.
This will be updated with a better Editor script in the next assigned task.
Refs: Struct - https://forum.unity.com/threads/how-to-use-structs-in-c.333682/
** Unity, such as https://docs.unity3d.com/ScriptReference/ExecuteAlways.html
** Fix for List:
 https://forum.unity.com/threads/public-list-not-appearing-in-inspector.520160/
** ** Troubleshooting may be helped by looking at ExcAlways docs in particular
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PuzzleManager : MonoBehaviour
{
    private float timer;
    private bool runTimer;
    public bool complete;
    public bool optional;
    public int maxFalseNodesAllowed;
    public GameObject dragPuzzlePieceHere;

    public List<PuzzleNode> index = new List<PuzzleNode>();
    private PuzzleNode nullNode = new PuzzleNode(null, false);


    //struct for handling puzzle node data
    [System.Serializable]
    public struct PuzzleNode
    {
        //public string name;
        [SerializeField]
        public GameObject item;
        public bool triggered;

        //Constructors
        /* public PuzzleNode(GameObject isItem)
         {
             //this.name = "";
             this.item = isItem;
             this.triggered = false;
         }*/

        //commenting out the name field at the moment
        public PuzzleNode(//string hasName, 
            GameObject isItem, bool isTriggered)
        {
            //this.name = hasName;
            this.item = isItem;
            this.triggered = isTriggered;
        }
    }//PuzzleNode

    /*
    * Start
    * Initializing loadout. Enabling Editor mode.
    */
    void Start()
    {
        if (Application.IsPlaying(gameObject))
        {
            // Play logic
        }
        else
        {
            // Editor logic
        }
    }//Start


    /*
    * Update 
    * Called once per frame
    * Advances active timers
    */
    void Update()
    {
        if (runTimer)
        {
            timer += Time.deltaTime;
        }
    }//Update

    /*
    * Add Node True
    * If there's no duplicate entry, this adds a new node to the list
    * holding the GameObject's id and a triggered value of true -
    * then the function returns true. If unsuccessful, it returns false.
    */
    public bool AddNodeTrue(GameObject hasItem)
    {
        if (!NoDuplicates(hasItem)){
            return false;
        }
        PuzzleNode newNode = new PuzzleNode(hasItem, true);
        index.Add(newNode);
        return true;
    }//AddNodeTrue

    /*
    * Add Node False
    * If there's no duplicate entry, this adds a new node to the list
    * holding the GameObject's id and a triggered value of false -
    * then the function returns true. If unsuccessful, it returns false.
    */
    public bool AddNodeFalse(GameObject hasItem)
    {
        if (!NoDuplicates(hasItem)){
            return false;
        }
        PuzzleNode newNode = new PuzzleNode(hasItem, false);
        index.Add(newNode);
        return true;
    }//AddNodeFalse

    /*
    * No Duplicates
    * This returns true if a scan of the list shows the GameObject's id
    * isn't already being tracked. Otherwise returns false.
    */
    bool NoDuplicates(GameObject hasItem)
    {
        PuzzleNode result = ScanList(hasItem);
        if (result.item == null)
        {
            return true;
        }
        return false;
    }//NoDuplicates

    /*
    * Flip Node
    * This returns true if a scan of the list shows the GameObject's id
    * is being tracked - then flips the target node's triggered boolean.
    * Otherwise returns false.
    * 
    * If the timer is off, a successful flip starts it - so a puzzle will
    * start counting time from the moment the player interacts with its
    * first piece.
    * 
    * A successful node flip to triggered will also start a scan that will
    * mark the puzzle as complete if all values in the list are true.
    */
    public bool FlipNode(GameObject hasItem)
    {
        PuzzleNode result = ScanList(hasItem);
        if (result.item == null)
        {
            return false;
        }
        result.triggered = !result.triggered;
        if (runTimer == false)
        {
            StartTimer();
        }
        if (result.triggered)
        {
            CheckStatus();
        }
        return true;
    }//FlipNode

    /*
    * Scan List
    * This returns the PuzzleNode with a matching item id, if one exists
    * in index. Otherwise, this returns 'nullNode' a placeholder holding a
    * null value instead of a GameObject id and a triggered value of false.
    */
    public PuzzleNode ScanList(GameObject hasItem)
    {
        PuzzleNode result = nullNode;
        if (hasItem == null)
        {
            return result;
        }
        //Debug.Log(hasItem);
        foreach (PuzzleNode entry in index)
        {
            if (entry.item == hasItem)
            {
                result = entry;
                break;
            }
        }
        return result;
    }//ScanList

   /*
    * Check Status
    * If the puzzle as a whole has not been marked as complete, this searches
    * the index for any untriggered nodes. If none are found, the puzzle is
    * marked complete and the timer is stopped.
    * 
    * The value maxFalse may be set so that Level Designers can fine tune
    * puzzle pieces that are optional to solving the puzzle. This may be
    * swapped out for a curTrue value in a later iteration.
    * 
    * This will incidentally flag a puzzle with an empty index as complete.
    */
    public void CheckStatus()
    {
        if (!complete)
        {
            int curFalse = 0;
            foreach (PuzzleNode entry in index)
            {
                if (entry.triggered == false)
                {
                    curFalse++;
                    if (curFalse > maxFalseNodesAllowed)
                    {
                        return;
                    }
                }
            }
            runTimer = false;
            complete = true;
        }
    }//Check Status

   /*
    * Is Complete
    * Outside facing function, reports status of the boolean complete variable.
    */
    public bool IsComplete()
    {
        return complete;
    }//IsComplete

    /*
    * Is Optional
    * Outside facing function, reports status of the boolean optional variable.
    */
    public bool IsOptional()
    {
        return optional;
    }//IsOptional

    /*
     * Report Time
     * Outside facing function, reports the float holding the timer.
     */
    public float ReportTime()
    {
        return timer;
    }//ReportTime

    /*
     * Reset Time
     * Outside facing function, zeroes out the timer's recorded float value.
     */
    public void ResetTime()
    {
        timer = 0;
    }//ResetTime

    /*
     * Start Timer
     * Outside facing function, sets a deactivated timer in motion.
     */
    public void StartTimer()
    {
        if (!runTimer)
        {
            runTimer = true;
        }
    }//StartTimer
}//PuzzleManager