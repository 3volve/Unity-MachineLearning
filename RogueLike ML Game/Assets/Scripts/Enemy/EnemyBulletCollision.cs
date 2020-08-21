using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletCollision : MonoBehaviour
{
    public int damage = 1;
    public Brain parent;

    private void OnCollisionStay(Collision collision)
    {
        string other = collision.gameObject.tag;

        if (other != "Enemy" && other != "Wall")
        {
            HP health = collision.gameObject.GetComponent<HP>();

            if (health == null) health = collision.gameObject.GetComponentInChildren<HP>();

            health?.DoDamage(damage);

            if (other == "Player") parent.damageDealt += damage;
        }
        if (other != "Enemy")
        {
            if (other != "Player") Debug.Log("Spawned?");

            Destroy(GetComponentInParent<Transform>().gameObject);
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        parent.numActiveBullets--;
    }
}
