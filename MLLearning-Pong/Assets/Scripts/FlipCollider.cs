using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipCollider : MonoBehaviour
{
    void Start()
    {
        PolygonCollider2D collider = GetComponent<PolygonCollider2D>();

        for (int i = 0; i < collider.points.Length; i++)
        {
            collider.points[i].y *= -1;
        }
    }
}
