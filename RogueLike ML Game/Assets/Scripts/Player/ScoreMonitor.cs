using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMonitor : MonoBehaviour
{
    public List<GameObject> checkMarks = new List<GameObject>();

    private int currentScore = 0;
    private readonly float scorePosX = -11.75f;
    private readonly float scorePosY = -2.3125f;
    private readonly float xSpacing = 0.375f;
    private readonly float ySpacing = 3.065f - 2.3125f;
    

    public void AddToScore()
    {
        if (currentScore == 0)
        {
            foreach (GameObject checkMark in checkMarks)
                checkMark.SetActive(true);
        }
        else
        {
            foreach (GameObject checkMark in checkMarks)
            {
                GameObject nextCheck = Instantiate(checkMark, checkMark.GetComponentsInParent<RectTransform>()[1]);
                RectTransform rectTrans = nextCheck.GetComponent<RectTransform>();
                rectTrans.localPosition = Vector3.zero;

                rectTrans.localPosition = new Vector3(scorePosX + currentScore * xSpacing, scorePosY - ySpacing * (currentScore / (int)(23.5f / xSpacing)), checkMark.transform.localPosition.z);
            }
        }

        currentScore++;
    }
}
