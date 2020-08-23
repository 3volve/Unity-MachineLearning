<<<<<<< Updated upstream
﻿using System.Collections;
using System.Collections.Generic;
=======
﻿using System.Collections.Generic;
>>>>>>> Stashed changes
using UnityEngine;

public class Brain : MonoBehaviour
{
<<<<<<< Updated upstream
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
=======
    ANN ann;
    double sumSquareError = double.MaxValue;
    readonly double errorAchievement = 0.01;
    readonly int cutOff = 100000;
    int giveUp = 10;

    void Start()
    {
        ann = new ANN(2, 1, 1, 2, 0.8);

        List<double> result;

        for (int count = 0; sumSquareError > errorAchievement && giveUp > 0; count++)
        {
            if (count >= cutOff)
            {
                ann.Reset();
                count = 0;
                giveUp--;
            }

            sumSquareError = 0;
            result = Train(1, 1, 0);
            sumSquareError += SumSquare(0, result);
            result = Train(1, 0, 1);
            sumSquareError += SumSquare(1, result);
            result = Train(0, 1, 1);
            sumSquareError += SumSquare(1, result);
            result = Train(0, 0, 0);
            sumSquareError += SumSquare(0, result);
        }
        Debug.Log("SSE: " + sumSquareError);

        result = Train(1, 1, 0);
        Debug.Log(" (1, 1): " + RoundToIntIfClose(result[0], 0.1));
        result = Train(1, 0, 1);
        Debug.Log(" (1, 0): " + RoundToIntIfClose(result[0], 0.1));
        result = Train(0, 1, 1);
        Debug.Log(" (0, 1): " + RoundToIntIfClose(result[0], 0.1));
        result = Train(0, 0, 0);
        Debug.Log(" (0, 0): " + RoundToIntIfClose(result[0], 0.1));
    }

    List<double> Train(int i1, int i2, int o)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(i1);
        inputs.Add(i2);
        outputs.Add(o);

        return ann.Go(inputs, outputs);
    }

    double SumSquare(int i1, List<double> result) => Mathf.Pow((float)result[0] - i1, 2);

    double RoundToIntIfClose(double num1, double delta)
    {
        double result = System.Math.Round(num1);
        if (System.Math.Abs(num1 - result) < delta) return result;

        else return num1;
>>>>>>> Stashed changes
    }
}
