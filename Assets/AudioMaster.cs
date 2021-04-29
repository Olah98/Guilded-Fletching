using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMaster : MonoBehaviour
{
    // Start is called before the first frame update
    public static AudioMaster AudioM;
    private GameObject[] _AudioObjects;
    void Start()
    {
        AudioM = this;

        _AudioObjects = GameObject.FindGameObjectsWithTag("Audio");
        StartCoroutine(LateStart());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVolumes(float master, float ambient)
    {
       foreach(GameObject audio in _AudioObjects)
       {
            if (audio!=null)
            {
                audio.GetComponent<AmbiencePlayer>().SetVolume(master, ambient);
            }
       }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(.1f);
        SetVolumes(SavedData.GetStoredOptionsAt(1).masterVol, SavedData.GetStoredOptionsAt(1).musicVol);
    }
}
