/*
Author: Christian Mullins
Date:03/11/2021
Summary: Test audio sound FX
*/
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTest : MonoBehaviour {
    private AudioSource _mySource;

    void Start() {
        _mySource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other) {
        string tagStr = other.transform.tag;
        if (tagStr == "Arrow" || tagStr == "Player") {
            _mySource.Play();
        }
    }
}
