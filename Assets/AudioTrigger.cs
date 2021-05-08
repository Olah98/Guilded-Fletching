using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    private bool hasPlayed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            if (!hasPlayed)
            {
                gameObject.GetComponentInParent<AudioSource>().Play();
                hasPlayed = true;
            }
        }
    }
}
