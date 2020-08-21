using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public GameObject eyes;
    public GameObject poof;
    public DNA dna;
    public int crash = 0;
    public float timeAlive;
    public float distanceTravelled = 0;
    public bool alive = true;
    public bool running = true;

    private readonly int DNALength = 9;
    private readonly float forwardForce = 1.0f;
    private Vector3 startingPos;
    private bool seeUpWall = false;
    private bool seeDownWall = false;
    private bool seeUpwardsWall = false;
    private bool seeDownwardsWall = false;
    private bool touchingUpWall = false;
    private bool touchingDownWall = false;
    private bool seeTop = false;
    private bool seeBottom = false;
    private Rigidbody2D rb2d;
    public bool stopped = false;

    private readonly float distanceCheckIn = 0.75f;
    public float requiredDeltaDistance;
    private float currentDistanceCounter = 0;
    private Vector3 lastRecordedPosition;

    private void FixedUpdate()
    {
        if (!alive || !running) return;

        seeUpWall = seeDownWall = seeTop = seeBottom = stopped = false;

        Debug.Log("hi Gabz whats happening in this loop right now?");
        currentDistanceCounter += Time.fixedDeltaTime;
        if (currentDistanceCounter >= distanceCheckIn)
        {
            currentDistanceCounter = 0;

            float deltaDistance = Vector3.Distance(lastRecordedPosition, transform.position);
            lastRecordedPosition = transform.position;

            if (deltaDistance < requiredDeltaDistance)
                stopped = true;
        }
        if (stopped) Debug.DrawRay(transform.position, transform.right, Color.red, 1f);

        RaycastHit2D hit = Physics2D.Raycast(eyes.transform.position, Vector2.right, 1.2f, LayerMask.GetMask("Default"));

        if (hit.collider != null)
        {
            string other = hit.collider.gameObject.tag;

            if (other == "upwall")
                seeUpWall = true;
            else if (other == "downwall")
                seeDownWall = true;
        }

        hit = Physics2D.Raycast(eyes.transform.position, Vector2.up, 1.5f, LayerMask.GetMask("Default"));

        if (hit.collider != null && hit.collider.gameObject.tag == "top")
            seeTop = true;


        hit = Physics2D.Raycast(eyes.transform.position, Vector2.down, 1.5f, LayerMask.GetMask("Default"));

        if (hit.collider != null && hit.collider.gameObject.tag == "bottom")
            seeBottom = true;

        float upForce = 0;

        int activeGene = //just a series of checks, where active gene is assigned to whichever the first one that is true.
            seeUpWall ? dna.GetGene(0) :
            seeDownWall ? dna.GetGene(1) :
            stopped && touchingUpWall ? dna.GetGene(2) :
            stopped && touchingDownWall ? dna.GetGene(3) :
            seeUpwardsWall ? dna.GetGene(4) :
            seeDownwardsWall ? dna.GetGene(5) :
            seeTop ? dna.GetGene(6) :
            seeBottom ? dna.GetGene(7) :
            dna.GetGene(8);

        upForce = activeGene;

        rb2d.AddForce(transform.right * forwardForce);
        rb2d.AddForce(transform.up * upForce * 0.1f);
        rb2d.velocity = new Vector2(Mathf.Min(rb2d.velocity.x, 2f), Mathf.Min(rb2d.velocity.y, 2f));

        distanceTravelled = Vector3.Distance(startingPos, transform.position);
        timeAlive = PopulationManager.elapsed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string other = collision.gameObject.tag;

        if (other == "dead")
        {
            if (alive) PopulationManager.numDead++;
            alive = false;
            distanceTravelled = 0;

            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;

            Destroy(Instantiate(poof, transform), 0.5f);
        }
        else if(other == "motivation")
        {
            running = false;
            rb2d.gravityScale = 0;

            GetComponent<Collider2D>().enabled = false;
            GetComponent<SpriteRenderer>().enabled = false;
            Destroy(Instantiate(poof, transform), 0.5f);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        string other = collision.gameObject.tag;
        touchingUpWall = touchingDownWall = false;

        if (other == "upwall")
        {
            touchingUpWall = true;
            crash++;
        }
        else if(other == "downwall")
        {
            touchingDownWall = true;
            crash++;
        }
    }

    /******************** Local Methods ********************/
    public GameObject Init()
    {
        //initialize DNA
        //0 -> forward; 1 -> updwall; 2 -> downwall; 3 normal upward;
        dna = new DNA(DNALength, -200); //putting in amount of thrust up or down?
        startingPos = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        timeAlive = 0;
        distanceTravelled = 0;
        alive = true;
        running = true;
        requiredDeltaDistance = distanceCheckIn / Time.fixedDeltaTime * 0.0000001f;

        return gameObject;
    }

    public Brain InitBrain() { Init(); return this; }
}
