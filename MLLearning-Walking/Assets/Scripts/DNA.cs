using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    List<int> genes = new List<int>();
    int dnaLength = 0;
    int maxValues = 0;

    public DNA(int l, int v)
    {
        dnaLength = l;
        maxValues = v;
        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();

        for(int i = 0; i < dnaLength; i++)
            genes.Add(Random.Range(0, maxValues));
    }

    public void Combine(DNA d1, DNA d2)
    {
        List<int> randomValues = new List<int>();

        for (int i = 0; i < dnaLength / 2.0; i++) {
            int newRandomValue;

            do newRandomValue = Random.Range(0, dnaLength);
            while (randomValues.Contains(newRandomValue));

            randomValues.Add(newRandomValue);
        }

        for(int i = 0; i < dnaLength; i++)
            genes[i] = randomValues.Contains(i) ? d1.genes[i] : d2.genes[i];
    }

    public void Mutate()
    {
        int randomGene = Random.Range(0, dnaLength);

        genes[randomGene] = Mathf.Clamp(genes[randomGene] + Random.Range(-maxValues / 10, maxValues / 10), 0, maxValues);
    }

    public void SetInt(int pos, int value) => genes[pos] = value;

    public int GetGene(int pos) => genes[pos];
}
