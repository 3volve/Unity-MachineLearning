using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] numberPaddles;
    public Transform player0;
    public Transform player1;
    public Transform player0TensPlace;
    public Transform player1TensPlace;

    private int player0Score = 0;
    private int player0Tens = 0;
    private int player1Score = 0;
    private int player1Tens = 0;

    public void Score(int playerNum)
    {
        if (playerNum == 0)
        {
            player0Score++;

            if (player0Score % 10 == 0)
            {
                player0Tens++;

                if(player0TensPlace.childCount > 0)
                    Destroy(player0TensPlace.GetChild(0).gameObject);

                Instantiate(numberPaddles[player0Tens], player0TensPlace);
            }

            Destroy(player0.GetChild(0).gameObject);
            Instantiate(numberPaddles[player0Score % 10], player0);
        }
        else if (playerNum == 1)
        {
            player1Score++;

            if (player1Score % 10 == 0)
            {
                player1Tens++;

                if (player1TensPlace.childCount > 0)
                    Destroy(player1TensPlace.GetChild(0).gameObject);

                Instantiate(numberPaddles[player1Tens], player1TensPlace);
            }

            Destroy(player1.GetChild(0).gameObject);
            Instantiate(numberPaddles[player1Score % 10], player1);
        }
    }
}
