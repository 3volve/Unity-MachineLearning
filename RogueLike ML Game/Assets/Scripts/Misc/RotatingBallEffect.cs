using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingBallEffect : MonoBehaviour
{
    public float spinningSpeed = 30;
    public float upDownMaxSpeed = 0.5f;
    public float smoothTime = 0.3f;
    public float initializeTime;
    public Transform top;
    public Transform bottom;
    public Vector3 rotationPoint;

    private bool up = true;
    private bool initialized = false;
    private float velocity = 0.0f;

    private void Start()
    {
        Invoke("WaitToStart", initializeTime);
    }

    void FixedUpdate()
    {
        if (initialized)
        {
            transform.RotateAround(rotationPoint, Vector3.up, spinningSpeed);

            if (up)
            {
                float newYPos = Mathf.SmoothDamp(transform.position.y, top.position.y, ref velocity, smoothTime);
                transform.Translate(0, newYPos / 100, 0);

                if (transform.position.y >= top.position.y)
                    up = false;
            }
            else
            {
                float newYPos = Mathf.SmoothDamp(-transform.position.y, -bottom.position.y, ref velocity, smoothTime);
                transform.Translate(0, newYPos / 100, 0);

                if (transform.position.y <= bottom.position.y)
                    up = true;
            }
        }
    }

    private void WaitToStart() => initialized = true;
}
