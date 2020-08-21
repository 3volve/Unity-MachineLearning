using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 0.15f;
    private string parentTag;
    private float startingHeight;

    void FixedUpdate()
    {
        if (FindObjectOfType<Pause>().Paused) return;
        Vector3 newPosition = transform.position + transform.forward * speed;

        transform.position = newPosition;
        Invoke("OnDestroy", 7);
    }

    private void Start() { startingHeight = transform.position.y; }

    private void OnDestroy()
    {
        foreach (Transform obj in GetComponentsInChildren<Transform>())
            Destroy(obj.gameObject);
    }
}
