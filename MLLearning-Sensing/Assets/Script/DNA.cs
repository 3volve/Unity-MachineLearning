using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    private List<int> genes = new List<int>();
    private List<int> maxValues = new List<int>();
    private readonly int DNALength = 0;
    private readonly int mutationChance = 10;

    public DNA(int l, params int[] v)
    {
        if (v.Length != l) Debug.Log("You messed up assigning max values in the brain.");
        DNALength = l;
        maxValues.AddRange(v);
        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();

        for (int i = 0; i < DNALength; i++)
            genes.Add(Random.Range(0, maxValues[i]));
    }

    public void Combine(DNA d1, DNA d2)
    {
        List<int> randomValues = new List<int>();

        for (int i = 0; i < DNALength / 2.0; i++)
        {
            int newRandomValue;

            do newRandomValue = Random.Range(0, DNALength);
            while (randomValues.Contains(newRandomValue));

            randomValues.Add(newRandomValue);
        }

        for (int i = 0; i < DNALength; i++)
            genes[i] = randomValues.Contains(i) ? d1.genes[i] : d2.genes[i];
    }

    public void Mutate()
    {
        if (Random.Range(0, 100) < mutationChance) {
            int randomGene = Random.Range(0, DNALength);

            genes[randomGene] = Mathf.Clamp(genes[randomGene] + Random.Range(-maxValues[randomGene] / 5, maxValues[randomGene] / 5), 0, maxValues[randomGene]);
        }
    }

    public void SetInt(int pos, int value) => genes[pos] = value;

    public int GetGene(int pos) => genes[pos];
}
