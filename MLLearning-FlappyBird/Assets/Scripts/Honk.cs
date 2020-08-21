using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Honk : MonoBehaviour
{
    public AudioClip[] clips;

    void Start()
    {
        GetComponent<AudioSource>().PlayOneShot(clips[Random.Range(0, 2)]);
    }
}
