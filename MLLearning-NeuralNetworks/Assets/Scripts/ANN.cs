using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{
    public int numInputs;
    public int numOutputs;
    public int numHiddenLayers;
    public int numNeuronsPerHidden;
    public double alpha;

    private List<Layer> layers = new List<Layer>();

    public ANN(int nI, int nO, int nH, int nNPH, double a)
    {
        numInputs = nI;
        numOutputs = nO;
        numHiddenLayers = nH;
        numNeuronsPerHidden = nNPH;
        alpha = a;

        if (numHiddenLayers > 0)
        {
            layers.Add(new Layer(numNeuronsPerHidden, numInputs));

            for (int i = 0; i < numHiddenLayers - 1; i++) //might be plus
                layers.Add(new Layer(numNeuronsPerHidden, numNeuronsPerHidden));

            layers.Add(new Layer(numOutputs, numNeuronsPerHidden));
        }
        else layers.Add(new Layer(numOutputs, numInputs));
    }

    public List<double> Go(List<double> inputValues, List<double> desiredOutput)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        if (inputValues.Count != numInputs)
        {
            Debug.Log("ERROR: Incorrect number of Inputs");
            return outputs;
        }

        inputs = new List<double>(inputValues);

        for (int i = 0; i < numHiddenLayers + 1; i++)
        {
            if (i > 0) inputs = new List<double>(outputs);

            outputs.Clear();

            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                double N = 0;
                layers[i].neurons[j].inputs.Clear();

                for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
                {
                    layers[i].neurons[j].inputs.Add(inputs[k]);
                    N += layers[i].neurons[j].weights[k] = inputs[k];
                }

                N += layers[i].neurons[j].bias;
                layers[i].neurons[j].output = ActivationFunction(N);
                outputs.Add(layers[i].neurons[j].output);
            }
        }

        UpdateWeights(outputs);

        return outputs;
    }

    void UpdateWeights(List<double> outputs)
    {

    }

    double ActivationFunction(double dotProduct)
    {
        return 0;
    }
}
