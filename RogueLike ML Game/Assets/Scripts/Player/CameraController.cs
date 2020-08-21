using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float distanceFromPlayer = 10f;

    private void Start()
    {
        //player = GetComponentInParent<Transform>();
    }
    void FixedUpdate()
    {
        Vector3 playerPosition = player.position + Vector3.up * 0.5f;

        Vector3 defaultTranslation = Vector3.zero;

        defaultTranslation = WallCheck(defaultTranslation, -transform.forward);

        if (Physics.Linecast(transform.position, playerPosition, out RaycastHit hit, LayerMask.GetMask("Wall")))
        {
            defaultTranslation.z += hit.distance;
        }

        defaultTranslation.y -= defaultTranslation.z * 0.23f;

        transform.localPosition += defaultTranslation;

        if (transform.localPosition.magnitude > 100)
            transform.localPosition = new Vector3(0, 4, -10);
    }

    private Vector3 WallCheck(Vector3 defaultTranslation, Vector3 direction)
    {
        float playerSpeed = player.GetComponent<Player>().movementSpeed;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, 5, LayerMask.GetMask("Wall")))
        {
            if (hit.distance < 1)
            {
                defaultTranslation.z += playerSpeed;

                Debug.DrawRay(transform.position, direction * 5, Color.red, 0.1f);
            }
            else if (hit.distance > 1.5f)
            {
                defaultTranslation.z = transform.localPosition.z - playerSpeed >= -distanceFromPlayer ? defaultTranslation.z - playerSpeed * 2 : defaultTranslation.z;

                Debug.DrawRay(transform.position, direction * 5, Color.green, 0.1f);
            }
        }
        else
        {
            defaultTranslation.z = transform.localPosition.z - playerSpeed >= -distanceFromPlayer ? defaultTranslation.z - playerSpeed : defaultTranslation.z;

            Debug.DrawRay(transform.position, direction * 5, Color.yellow, 0.1f);
        }

        return defaultTranslation;
    }

    private void OnTriggerStay(Collider other)
    {
        Vector3 defaultTranslation = Vector3.zero;
        float playerSpeed = player.GetComponent<Player>().movementSpeed;

        if(transform.localPosition.z + 0.25f < 0)
            defaultTranslation.z += 0.25f;

        if(transform.localPosition.y - defaultTranslation.z * 0.23f > 0)
            defaultTranslation.y -= defaultTranslation.z * 0.23f;

        transform.localPosition += defaultTranslation;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.tag == "Ground")
        {
            Vector3 defaultTranslation = Vector3.zero;
            float playerSpeed = player.GetComponent<Player>().movementSpeed;

            defaultTranslation.z -= 0.25f;
            defaultTranslation.y += defaultTranslation.z * 0.23f;
            transform.localPosition += defaultTranslation;
        }
    }
}
