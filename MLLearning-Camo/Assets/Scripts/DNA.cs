using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA : MonoBehaviour
{
    //gene for color and/size
    public float[] rgbs;

    //primary output measure
    public float timeToDie = 0;
    public bool dead = false;

    private SpriteRenderer sRenderer;
    private Collider2D sCollider;

    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        sCollider = GetComponent<Collider2D>();

        sRenderer.color = new Color(rgbs[0], rgbs[1], rgbs[2]);
        transform.localScale = new Vector3(rgbs[3], rgbs[3]);
    }

    private void OnMouseDown()
    {
        dead = true;
        timeToDie = PopulationManager.elapsed;
        PopulationManager.numDead++;
        sRenderer.enabled = false;
        sCollider.enabled = false;
    }
}
