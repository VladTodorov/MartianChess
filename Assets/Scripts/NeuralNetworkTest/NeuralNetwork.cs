using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class NeuralNetwork
{
    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;

    private Random random;

    public NeuralNetwork(int[] _layers)
    {
        layers = new int[_layers.Length]; //redundant?
        layers = _layers.ToArray();

        random = new Random(System.DateTime.Today.Millisecond);

        InitNurons();
        InitWeights();
    }

    private void InitNurons()
    {
        List<float[]> nuronsList = new();

        for (int i = 0; i < layers.Length; ++i)
        {
            nuronsList.Add(new float[layers[i]]);
        }

        neurons = nuronsList.ToArray();
    }

    private void InitWeights()
    {
        List<float[][]> weightsList = new();

        for (int i = 1; i < layers.Length; ++i)
        {
            List<float[]> layerWeightsList = new();
            int neuronsInPreviousLayer = layers[i - 1]; //num

            for (int j = 0; j < neurons[i].Length; ++j)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                for (int k = 0; k < neuronsInPreviousLayer; ++k)
                {
                    neuronWeights[k] = (float)random.NextDouble() - 0.5f;  // (-.5, .5)
                }

                layerWeightsList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray();
    }


    public float[] FeedForward(float[] inputs)
    {
        //neurons[0] = inputs.ToArray();     // maybe better?
        for (int i = 0; i < layers.Length; ++i)
        {
            neurons[0][i] = inputs[i];
        }

        for (int i = 1; i < layers.Length; ++i)
        {
            for (int j = 0; j < neurons[i].Length; ++j)
            {
                float value = 0.25f;

                for (int k = 0; k < neurons[i-1].Length; ++k)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }


        return neurons[neurons.Length - 1];
    }


}
