using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA
{
    private List<int> genes = new List<int>();
    private List<int> minValues = new List<int>();
    private List<int> maxValues = new List<int>();
    private readonly int DNALength;
    private readonly int mutationChance = 20; //20% chance to mutate

    public DNA(int l, params int[] v)
    {
        if (v.Length != l && v.Length > 1) Debug.Log("You messed up assigning max values in the brain.");
        
        for (int i = 0; i < l; i++)
        {
            int currentValue = v.Length == 1 ? v[0] : v[i];
                
            int min = Mathf.Min(currentValue, 0);
            int max = Mathf.Abs(currentValue);

            minValues.Add(min);
            maxValues.Add(max);
        }

        DNALength = l;
        SetRandom();
    }

    public void SetRandom()
    {
        genes.Clear();

        for (int i = 0; i < DNALength; i++)
            genes.Add(Random.Range(minValues[i], maxValues[i]));
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

    public void Mutate(DNA dna)
    {
        for (int i = 0; i < DNALength; i++)
        {
            dna.SetInt(i, dna.GetGene(i));

            if (Random.Range(0, 100) < mutationChance)
            {
                genes[i] = Mathf.Clamp(
                    genes[i] + Random.Range(-maxValues[i] / 5, maxValues[i] / 5),
                    minValues[i],
                    maxValues[i]
                );
            }
        }
    }

    public void SetInt(int pos, int value) => genes[pos] = value;

    public int GetGene(int pos) => genes[pos];
}
