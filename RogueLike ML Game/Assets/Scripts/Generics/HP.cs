using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HP : MonoBehaviour
{
    public int maxHealth = 10;
    public bool beingDamaged = false;
    private int health;
    private bool dead = false;

    public delegate void JustDied();
    public JustDied justDied;
    public delegate void JustHit();
    public JustHit justHit;

    private void Start() { health = maxHealth; }

    public int Health
    {
        private get { return health; }
        set
        {
            if (value <= 0)
            {
                health = 0;
                Dead = true;
            }
            else
            {
                justHit?.Invoke();
                health = value;
            }
        }
    }

    public bool Dead
    {
        get { return dead; }
        private set
        {
            if (value) justDied?.Invoke();
            else health = maxHealth;

            dead = value;
        }
    }

    public void DoDamage(int damageDealt)
    {
        if (!beingDamaged)
        {
            Health -= damageDealt;
            beingDamaged = true;
            Invoke("ResetDamaged", 1f);
        }
    }


    public void Reset()
    {
        health = maxHealth;
        dead = false;
    }

    private void ResetDamaged() => beingDamaged = false;
}
