/*
Author: Warren Rose II
Data: 02/21/2021
Summary: Enables the Level Designers to keep track of elapsed time in a level
* as well as reporting the time at which all puzzles are completed.
Refs: Unity - https://docs.unity3d.com/ScriptReference/ExecuteAlways.html
* Timer - https://answers.unity.com/questions/905990/
** ** Troubleshooting may be helped by looking at ExcAlways docs in particular
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class LevelManager : MonoBehaviour
{
    private float timer;
    private float timerPuzzles;
    private bool runTimer;
    private bool complete;
    public Text timerLabel;

    List<PuzzleManager> index = new List<PuzzleManager>();

    /*
    * Start
    * Initializing loadout. Enabling Editor mode.
    */
    void Start()
    {
        if (Application.IsPlaying(gameObject))
        {
            // Play logic
            ResetTime();
            StartTimer();
        }
        else
        {
            // Editor logic
        }
    }//Start


    /*
    * Update 
    * Called once per frame
    * Advances active timers, starting with the time elapsed since level start
    * If the level has not been flagged as complete, it checks if it should be,
    * then advances the timer keeping track of how long it is taking the player
    * to solve puzzles. Having this many calls to CheckStatus() seems bad, so
    * I may switch it so CheckStatus only runs if called by a completed puzzle
    */
    void Update()
    {
        if (runTimer)
        {
            timer += Time.deltaTime;
            if (!complete)
            {
                CheckStatus();
                if (!complete)
                {
                    timerPuzzles += Time.deltaTime;
                }
            }
        }
        var minutes = timer / 60;
        var seconds = timer % 60;
        //var fraction = (timer * 100) % 100;

        //update the label value
        //timerLabel.text = string.Format("{0:00} : {1:00} 
        //: {2:000}", minutes, seconds, fraction);
        if (timerLabel != null)
        {
            timerLabel.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }

    }//Update

    /*
    * Add Puzzle Manager
    * If there's no duplicate entry, this adds a new node to the list
    * holding the PuzzleManager in question.
    */
    public void AddPuzzleManager(PuzzleManager item)
    {
        foreach (PuzzleManager entry in index)
        {
            if (entry.GetInstanceID() == item.GetInstanceID())
            {
                return;
            }
        }
        index.Add(item);
    }//AddPuzzleManager

   /*
    * Check Status
    * If the level as a whole has not been marked as complete, this searches
    * the index for any untriggered non-optional nodes. If none are found,
    * the level is marked complete and the puzzles timer is stopped.
    * 
    * This will incidentally flag a level with an empty index as complete.
    */
    public void CheckStatus()
    {
        if (!complete)
        {
            foreach (PuzzleManager entry in index)
            {
                if ((entry.IsComplete() == false)
                    && (entry.IsOptional() == false))
                {
                    return;
                }
            }
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
     * Report Time
     * Outside facing function, reports the float holding the timer.
     */
    public float ReportTime()
    {
        return timer;
    }//ReportTime

    /*
    * Report Puzzles Time
    * Outside facing function, reports the float holding the puzzles timer.
    */
    public float ReportTimePuzzles()
    {
        return timerPuzzles;
    }//ReportTimePuzzles

    /*
     * Reset Time
     * Outside facing function, zeroes out the timer's recorded float value.
     */
    public void ResetTime()
    {
        timer = 0;
        timerPuzzles = 0;
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
}//LevelManager