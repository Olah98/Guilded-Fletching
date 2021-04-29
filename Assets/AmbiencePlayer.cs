using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    //Author Miles Gomez
    //Edited 4/27/2021

    //This script helps manipulate ambient sounds however the user sees fit. See public variables and their tooltips to see what can be adjusted.
  
    private AudioSource _ambience;
    [Tooltip("Between 0.0 and 1.0")]
    [Range(0.0f, 1.0f)] public float maxVolume;
    [Tooltip("Puts a small timer on start timer to avoid stacking same sound")]
    public bool isDelayed;
    private void Start()
    {
        _ambience = gameObject.GetComponent<AudioSource>();
        _ambience.volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;     

        if (isDelayed)
        {
            if (_ambience.isPlaying)
            {
                _ambience.Stop();
            }
            _ambience.PlayDelayed(Random.Range(0f, 5f));
        }
    }

    public void SetVolume(float master, float ambient)
    {
        _ambience.volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;
    }

}
