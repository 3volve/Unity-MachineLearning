using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBladeMod : MonoBehaviour
{
    public float spinSpeed = 5;

    void Update()
    {
        transform.Rotate(0, 0, spinSpeed);
    }
}
