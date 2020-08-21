using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public GameObject eyes;

    public float timeAlive;
    public float maxDistance;
    public DNA dna;
    public bool alive = true;

    private readonly int DNALength = 5;
    private Vector3 startingPos;
    public bool seeWall = false;
    public bool afterWall = false;
    public bool stopped = false;

    private readonly float distanceCheckIn = 0.75f;
    public float requiredDeltaDistance;
    private float currentDistanceCounter = 0;
    private Vector3 lastRecordedPosition;

    private void Start()
    {
        requiredDeltaDistance = distanceCheckIn / Time.fixedUnscaledDeltaTime * 0.075f * 0.25f;
    }

    private void FixedUpdate()
    {
        if (!alive) return;
        
        stopped = false;
        currentDistanceCounter += Time.fixedDeltaTime;
        if(currentDistanceCounter >= distanceCheckIn)
        {
            currentDistanceCounter = 0;

            float deltaDistance = Vector3.Distance(lastRecordedPosition, transform.position);
            lastRecordedPosition = transform.position;

            if (deltaDistance < requiredDeltaDistance)
                stopped = true;
        }
        
        if (Physics.SphereCast(eyes.transform.position, 0.2f, eyes.transform.forward, out RaycastHit hit, 0.5f, LayerMask.GetMask("Default")))
        {
            seeWall = false;

            if (hit.collider.gameObject.tag == "wall")
                seeWall = true;
        }
        else if (seeWall == true)
        {
            seeWall = false;
            afterWall = true;
        }
        else if (afterWall == true)
            afterWall = false;
        
        float turn = 0;
        float move = 0;

        int activeGene = seeWall ? dna.GetGene(0) : dna.GetGene(1);

        if (afterWall)
        {
            activeGene = dna.GetGene(2);
            afterWall = false;
        }
        else if (stopped)
            activeGene = dna.GetGene(3);
        
        switch (activeGene)
        {
            case 0: move = 1;
                break;
            case 1: turn = -dna.GetGene(4);
                break;
            case 2: turn = dna.GetGene(4);
                break;
        }

        transform.Translate(0, 0, move * 0.075f);
        transform.Rotate(0, turn, 0);

        float newDistance = Vector3.Distance(startingPos, transform.position);
        if (newDistance > maxDistance) maxDistance = newDistance;
        timeAlive = PopulationManager.elapsed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string other = collision.gameObject.tag;

        if (other == "dead")
        {
            if (alive) PopulationManager.numDead++;
            alive = false;
            maxDistance = 0;
        }
    }

    /******************** Local Methods ********************/
    public void Init()
    {
        //initialize DNA
        //0 -> forward; 1 -> left; 2 -> right;
        dna = new DNA(DNALength, 3, 3, 3, 3, 90);
        timeAlive = 0;
        startingPos = transform.position;
        lastRecordedPosition = startingPos;
        maxDistance = 0;
        alive = true;
    }
}
