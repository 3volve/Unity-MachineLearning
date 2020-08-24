using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    public GameObject paddle;
    public GameObject ball;
    public float numSaved = 0;
    public float numMissed = 0;

    Rigidbody2D rb2d;
    float posX;
    RaycastHit2D checker;
    float yvel;
    bool training = true;
    readonly float paddleYClamp = 4.4f;
    readonly float paddleMaxSpeed = 15;

    readonly string[] layers = new string[] { "topwall", "bottomwall", "sidewall" };

    ANN ann;
    //double sumSquareError = double.MaxValue;
    //readonly double errorAchievement = 0.01;
    //readonly int cutOff = 100000;
    //int giveUp = 10;

    void Start()
    {
        ann = new ANN(7, 1, 1, 4, 0.05);
        rb2d = ball.GetComponent<Rigidbody2D>();
        posX = paddle.transform.position.x;
    }

    private void Update()
    {
        float posy = Mathf.Clamp(paddle.transform.position.y +
                                yvel * Time.deltaTime * paddleMaxSpeed,
                                -paddleYClamp, paddleYClamp);

        if(posy.ToString() != "NaN")
            paddle.transform.position = new Vector3(posX, posy, 0);

        List<double> output = new List<double>();

        if (rb2d.velocity.x > 0)
        {
            int layerMask = LayerMask.GetMask(layers);
            Vector2 direction = rb2d.velocity;

            checker = Physics2D.Raycast(ball.transform.position, direction, 1000, layerMask);

            int bounces = 0;
            while (checker.collider?.tag == "topwall" || checker.collider?.tag == "bottomwall")
            {
                if (bounces++ >= 10) break;

                direction = Vector2.Reflect(direction, checker.normal);

                layerMask = (checker.collider.tag == layers[0]) ?
                    LayerMask.GetMask(layers[1], layers[2]) :
                    LayerMask.GetMask(layers[0], layers[2]);

                checker = Physics2D.Raycast(checker.point, direction, 1000, layerMask);
            }

            if (checker.collider?.tag == "backwall")
            {
                float dy = checker.point.y - paddle.transform.position.y;

                output = Run(ball.transform.position.x,
                    ball.transform.position.y,
                    rb2d.velocity.x,
                    rb2d.velocity.y,
                    paddle.transform.position.x,
                    paddle.transform.position.y,
                    bounces,
                    dy,
                    training
                    );
                 
                yvel = (float)output[0];
            }
            else yvel = 0;
        }
    }

    List<double> Run(double bx, double by, double bvx, double bvy, double px, double py, double bb, double pv, bool train)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();
        inputs.Add(bx);
        inputs.Add(by);
        inputs.Add(bvx);
        inputs.Add(bvy);
        inputs.Add(px);
        inputs.Add(py);
        inputs.Add(bb);

        if (train)
        {
            outputs.Add(pv);
            return ann.Train(inputs, outputs);
        }

        return ann.CalcOutput(inputs);
    }

    double SumSquare(int i1, List<double> result) => System.Math.Pow(result[0] - i1, 2);

    double RoundToIntIfClose(double num1, double delta)
    {
        double result = System.Math.Round(num1);
        if (System.Math.Abs(num1 - result) < delta) return result;

        else return num1;
    }

    /*
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
        Debug.Log(" (0, 0): " + RoundToIntIfClose(result[0], 0.1));*/
}
