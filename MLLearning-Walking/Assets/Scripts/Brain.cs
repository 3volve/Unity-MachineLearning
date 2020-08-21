using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class Brain : MonoBehaviour
{
    public int DNALength = 1;
    public float timeAlive;
    public float distanceFromStart;
    public DNA dna;

    private ThirdPersonCharacter m_Character;
    private Vector3 m_StartPos;
    private Vector3 m_Move;
    private bool m_Jump;
    public bool alive = true;

    private void FixedUpdate()
    {
        float h = 0;
        float v = 0;
        bool crouch = false;
        
        switch(dna.GetGene(0))
        {
            case 0: v = 1;
                break;
            case 1: v = -1;
                break;
            case 2: h = -1;
                break;
            case 3: h = 1;
                break;
            case 4: m_Jump = true;
                break;
            case 5: crouch = true;
                break;
        }

        m_Move = v * Vector3.forward + h * Vector3.right;
        m_Character.Move(m_Move, crouch, m_Jump);
        m_Jump = false;

        if (alive)
        {
            timeAlive += Time.deltaTime;
            distanceFromStart = Vector3.Distance(m_StartPos, transform.position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "dead")
        {
            alive = false;
            PopulationManager.numDead++;
        }
    }


    /******************** Local Methods ********************/
    public void Init()
    {
        //initialize DNA
        //0 -> forward; 1 -> back; 2 -> left; 3 -> right; 4 -> jump; 5 -> crouch
        dna = new DNA(DNALength, 6);
        m_Character = GetComponent<ThirdPersonCharacter>();
        m_StartPos = transform.position;
        timeAlive = 0;
        alive = true;
    }

    public static int Compare(Brain b1, Brain b2) =>
        b1.timeAlive.CompareTo(b2.timeAlive) != 0 ?             //if timeAlive is not equal
        b1.timeAlive.CompareTo(b2.timeAlive) :                  //then return just the timeAlive comparison
        b1.distanceFromStart.CompareTo(b2.distanceFromStart);   //otherwise if timeAlive is equal, then compare the distance from start as well
}
