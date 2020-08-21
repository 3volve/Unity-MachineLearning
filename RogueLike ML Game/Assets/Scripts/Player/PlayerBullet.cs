using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 0.15f;
    public int damage = 1;
    public ScoreMonitor parent;
    private float startingHeight;

    void FixedUpdate()
    {
        if (FindObjectOfType<Pause>().Paused) return;
        Vector3 newPosition = transform.position + transform.forward * speed;
        newPosition.y = startingHeight;

        transform.position = newPosition;
    }

    private void Start()
    {
        startingHeight = transform.position.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string other = collision.gameObject.tag;

        if (other != "Player" && other != "Wall")
        {
            HP health = collision.gameObject.GetComponent<HP>();

            if (health == null) health = collision.gameObject.GetComponentInChildren<HP>();

            if (health != null) health.DoDamage(damage);

            if (other == "Enemy") parent.AddToScore();
        }
        
        Destroy(gameObject);
    }
}

