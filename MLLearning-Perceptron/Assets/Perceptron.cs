using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class TrainingSet
{
	public double[] input;
	public double output;
}

public class Perceptron : MonoBehaviour
{
    public GameObject npc;

    public static readonly char seperator = ',';

	private List<TrainingSet> ts = new List<TrainingSet>();
    private double[] weights = {0,0};
    private double bias = 0;
    private string savePath;

    void Start()
    {
        savePath = Application.dataPath + "/weights.txt";

        if (!LoadWeights()) InitialiseWeights();
    }

    private void Update()
    {
        if (Input.GetKeyDown("s"))
            SaveWeights();
    }

    void Train()
    {
        for (int t = 0; t < ts.Count; t++)
            UpdateWeights(t);
    }

	void InitialiseWeights()
	{
		for(int i = 0; i < weights.Length; i++) 
			weights[i] = Random.Range(-1.0f,1.0f);

		bias = Random.Range(-1.0f,1.0f);
	}

    public void SendInput(double input1, double input2, double output)
    {
        double result = CalcOutput(input1, input2);

        if (result == 0) npc.GetComponent<Animator>().SetTrigger("Crouch");
        npc.GetComponent<Rigidbody>().isKinematic = result != 0;

        TrainingSet set = new TrainingSet();
        set.input = new double[] { input1, input2 };
        set.output = output;
        ts.Add(set);
        Train();
    }

	void UpdateWeights(int j)
	{
		double error = ts[j].output - CalcOutput(j);

		for(int i = 0; i < weights.Length; i++)
			weights[i] = weights[i] + error*ts[j].input[i]; 

		bias += error;
    }

    double DotProductBias(double[] v1, double[] v2)
    {
        if (v1 == null || v2 == null || v1.Length != v2.Length) return -1;

        double d = 0;
        for (int x = 0; x < v1.Length; x++)
            d += v1[x] * v2[x];

        d += bias;

        return d;
    }

    bool LoadWeights()
    {
        if(File.Exists(savePath))
        {
            StreamReader sr = File.OpenText(savePath);

            string line = sr.ReadLine();
            string[] w = line.Split(seperator);

            weights[0] = System.Convert.ToDouble(w[0]);
            weights[1] = System.Convert.ToDouble(w[1]);
            bias = System.Convert.ToDouble(w[2]);

            Debug.Log("loading...");

            return true;
        }

        return false;
    }
    
    void SaveWeights()
    {
        StreamWriter sr = File.CreateText(savePath);

        sr.WriteLine(weights[0] + seperator + weights[1] + seperator + bias);

        sr.Close();
    }

    double CalcOutput(int i) => ActivationFunction(DotProductBias(weights, ts[i].input));

    double CalcOutput(double i1, double i2) => ActivationFunction(DotProductBias(weights, new double[] { i1, i2 }));

    double ActivationFunction(double dp) => dp > 0 ? 1 : 0;
}