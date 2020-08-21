using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDelete : MonoBehaviour
{
    public float timeToLive;

    void Start() => Destroy(gameObject, timeToLive);
}
