using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotorSpinning : MonoBehaviour
{
    public static float spinSpeed = 50f;
    
    void FixedUpdate()
    {
        transform.Rotate(0, 0, spinSpeed);
    }
}
