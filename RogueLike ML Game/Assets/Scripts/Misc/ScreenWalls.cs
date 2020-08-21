using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWalls : MonoBehaviour
{
    public GameObject healthBar;
    public GameObject countDown;
    public GameObject checkmark;
    public Pause pauseManager;
    public ScoreMonitor scoreMonitor;
    public HealthMonitor healthMonitor;

    void Start()
    {
        pauseManager?.countObjects.Add(countDown);
        scoreMonitor.checkMarks.Add(checkmark);
        healthMonitor.healthBars.Add(healthBar);
    }
}
