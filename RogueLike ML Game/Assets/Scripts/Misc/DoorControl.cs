using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorControl : MonoBehaviour
{
    public Transform door;
    public float doorSpeed;
    public float doorWidth = 1.8f;
    public bool opening = false;
    public bool closing = false;

    private float closedPos;
    private float openPos;

    private void Start()
    {
        closedPos = door.localPosition.x;
        openPos = door.localPosition.x + doorWidth;
    }

    void Update()
    {
        if(opening)
        {
            if (door.localPosition.x <= openPos)
            {
                door.Translate(doorSpeed, 0, 0);
            }
            else
            {
                door.localPosition = new Vector3(openPos, door.localPosition.y, door.localPosition.z);
                opening = false;
            }
        }
        else if(closing)
        {
            if (door.localPosition.x >= closedPos)
            {
                door.Translate(-doorSpeed, 0, 0);
            }
            else
            {
                door.localPosition = new Vector3(closedPos, door.localPosition.y, door.localPosition.z);
                closing = false;
            }
        }
    }
}
