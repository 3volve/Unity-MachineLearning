using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public bool Paused
    {
        get { return paused; }
        set
        {
            if (value)
            {
                countdown = maxCountdown;

                foreach (List<RectTransform> list in counts)
                    foreach (RectTransform trans in list)
                        trans.gameObject.SetActive(true);
            }

            paused = value;
        }
    }

    public List<GameObject> countObjects = new List<GameObject>();

    private readonly int maxCountdown = 5;

    private List<RectTransform>[] counts;
    private float countdown = 0;
    private bool paused = false;

    private void FixedUpdate()
    {
        if (Paused)
        {
            countdown -= Time.fixedDeltaTime;
            
            if(countdown % 1 < 0.1)
                foreach(RectTransform trans in counts[(int)countdown])
                    trans.gameObject.SetActive(false);

            if (countdown <= 0) Paused = false;
        }
    }

    private void Start()
    {
        counts = new List<RectTransform>[maxCountdown];
        
        for (int i = 0; i < counts.Length; i++)
            counts[i] = new List<RectTransform>();

        foreach (GameObject counter in countObjects)
        {
            RectTransform[] temp = counter.GetComponentsInChildren<RectTransform>(true);

            for (int i = 1; i < temp.Length; i++)
                counts[i - 1].Add(temp[i]);
        }
    }
}
