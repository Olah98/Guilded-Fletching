using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public static MusicPlayer instance;
    [Tooltip("Between 0.0 and 1.0")]
    [Range(0.0f, 1.0f)] public float maxVolume;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        gameObject.GetComponent<AudioSource>().volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;
       
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<AudioSource>().volume = maxVolume * SavedData.GetStoredOptionsAt(1).masterVol * SavedData.GetStoredOptionsAt(1).musicVol;
    }
}
