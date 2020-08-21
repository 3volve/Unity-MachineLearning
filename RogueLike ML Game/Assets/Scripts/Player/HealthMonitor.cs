using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthMonitor : MonoBehaviour
{
    public float healthLength;
    public float maxHealthLength;
    public float healthPos;
    public List<GameObject> healthBars = new List<GameObject>();
    public float damageAmount = 0;
    public bool decreasing = false;
    public bool increasing = false;
    public int playerHealth;
    public float hitValue;

    private void Start() { maxHealthLength = healthLength; }

    void Update()
    {
        foreach (GameObject healthBar in healthBars)
        {
            healthBar.transform.localPosition = new Vector2(healthPos, 0);
            healthBar.GetComponent<RectTransform>().sizeDelta = new Vector2(healthLength, 1);
        }

        if (decreasing)
        {
            if (damageAmount >= hitValue)
            {
                decreasing = false;
                damageAmount = 0;
            }
            else
            {
                damageAmount += 0.5f;
                healthLength -= 0.5f;
                healthPos -= 0.25f;
            }
        }

        if (increasing)
        {
            if (damageAmount >= maxHealthLength - hitValue && hitValue != maxHealthLength) //this is how you affect how much my game affects healthgain
            {
                increasing = false;
                damageAmount = 0;
            }
            else if(healthLength >= maxHealthLength && hitValue == maxHealthLength)
            {
                increasing = false;
                damageAmount = 0;
            }
            else
            {
                damageAmount += 0.5f;
                healthLength += 0.5f;
                healthPos += 0.25f;
            }
        }
    }
}
