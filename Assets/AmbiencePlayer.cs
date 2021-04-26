using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbiencePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource _ambience;
    [Tooltip("Between 0.0 and 1.0")]
    [Range(0.0f, 1.0f)] public float maxVolume;
    private void Start()
    {
        _ambience = gameObject.GetComponent<AudioSource>();
        _ambience.volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;     
    }

    private void Update()
    {
        _ambience.volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;
    }

  


}
