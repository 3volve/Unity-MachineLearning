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

            for (int i = 0; i < numHiddenLayers - 1; i++)
                layers.Add(new Layer(numNeuronsPerHidden, numNeuronsPerHidden));

            layers.Add(new Layer(numOutputs, numNeuronsPerHidden));
        }
        else layers.Add(new Layer(numOutputs, numInputs));
    }

    public List<double> Train(List<double> inputValues, List<double> desiredOutput)
    {
        List<double> outputs = new List<double>();

        outputs = CalcOutput(inputValues);
        UpdateWeights(outputs, desiredOutput);

        return outputs;
    }

    public List<double> CalcOutput(List<double> inputValues)
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
                Neuron curNeuron = layers[i].neurons[j];
                double N = 0;

                curNeuron.inputs.Clear();

                for (int k = 0; k < curNeuron.numInputs; k++)
                {
                    curNeuron.inputs.Add(inputs[k]);
                    N += curNeuron.weights[k] * inputs[k];
                }

                N -= curNeuron.bias;

                if (i == numHiddenLayers)
                    curNeuron.output = ActivationFunctionO(N);
                else curNeuron.output = ActivationFunctionH(N);

                outputs.Add(curNeuron.output);
            }
        }

        return outputs;
    }

    void UpdateWeights(List<double> outputs, List<double> desiredOutputs)
    {
        double error;

        for (int i = numHiddenLayers; i >= 0; i--)
        {
            for (int j = 0; j < layers[i].numNeurons; j++)
            {
                Neuron curNeuron = layers[i].neurons[j];

                if (i == numHiddenLayers)
                {
                    error = desiredOutputs[j] - outputs[j];
                    curNeuron.errorGradient = outputs[j] * (1 - outputs[j]) * error;
                    //errorGradient calculated with Delta Rule.
                }
                else
                {
                    curNeuron.errorGradient = curNeuron.output * (1 - curNeuron.output);

                    double errorGradSum = 0;
                    
                    for (int p = 0; p < layers[i + 1].numNeurons; p++)
                        errorGradSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];

                    curNeuron.errorGradient *= errorGradSum;
                }

                for (int k = 0; k < curNeuron.numInputs; k++)
                {
                    double curInputValue = curNeuron.inputs[k];

                    if (i == numHiddenLayers)
                    {
                        error = desiredOutputs[j] - outputs[j];
                        curNeuron.weights[k] += alpha * curInputValue * error;
                    }
                    else curNeuron.weights[k] += alpha * curInputValue * curNeuron.errorGradient;
                }

                curNeuron.bias += alpha * -1 * curNeuron.errorGradient;
            }
        }
    }

    public void Reset()
    {
        foreach (Layer layer in layers)
            layer.Reset();
    }

    double ActivationFunctionH(double value) => TanH(value);

    double ActivationFunctionO(double value) => TanH(value);

    double Step(double value) => value < 0 ? 0 : 1;

    double ReLu(double value) => value < 0 ? 0 : value;

    double LeakyReLu(double value) => value < 0 ? value * 0.01 : value;

    double Sigmoid(double value)
    {
        double k = System.Math.Exp(value);
        return k / (1.0f + k);
    }

    double Sinusoid(double value) => System.Math.Sin(value);

    double TanH(double value) => 2 * Sigmoid(2 * value) - 1;

    double ArcTan(double value) => System.Math.Atan(value);

    double Softsign(double value) => value / (1 + System.Math.Abs(value));
}
