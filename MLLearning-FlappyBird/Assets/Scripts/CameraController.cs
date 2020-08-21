using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject following;

    void Update() => transform.position = following != null ?
        new Vector3(following.transform.position.x + 6, transform.position.y, transform.position.z) :
        transform.position;

    public void SetLead(GameObject newFollow)
    {
        following = newFollow;
    }
}
