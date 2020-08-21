using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    public GameObject personPrefab;
    public int populationSize = 10;
    public static float elapsed = 0;
    public static int numDead = 0;

    private List<GameObject> population = new List<GameObject>();
    private int generation = 1;
    private GUIStyle guiStyle = new GUIStyle();
    private readonly float minSize = 0.1f;
    private readonly float maxSize = 0.4f;

    void Start()
    {
        for (int i = 0; i < populationSize; i++)
        {
            GameObject go = InstantiatePerson();

            go.GetComponent<DNA>().rgbs = new float[] {
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f),
                Random.Range(minSize, maxSize)
            };

            population.Add(go);
        }
    }
    
    void Update()
    {
        elapsed += Time.deltaTime;
        
        if (numDead >= populationSize / 2f)
        {
            foreach (GameObject person in population)
                if (!person.GetComponent<DNA>().dead) person.GetComponent<DNA>().timeToDie = elapsed;

            BreedNewPopulation();
            elapsed = 0;
            numDead = 0;
        }
    }

    void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();
        //Get rid of unfit individuals
        List<GameObject> sortedList = population.OrderByDescending(o => o.GetComponent<DNA>().timeToDie).ToList();

        population.Clear();

        //Cull bottom half of sorted list
        for (int i = 0; i <= (int)(sortedList.Count / 2.0f); i++)
        {
            Destroy(sortedList[i]);
            sortedList.RemoveAt(i);
        }

        //Breed the remainder
        for (int i = 0; i < sortedList.Count - 1; i++)
        {
            int otherParent = Random.Range(0, sortedList.Count);

            bool fitWeight = Random.Range(0f, 1f) < 0.20f;

            while (otherParent == i)
            {
                if(fitWeight && sortedList.Count > 2)
                    otherParent = Random.Range((int)(sortedList.Count / 2.0f) - 1, sortedList.Count);
                else
                    otherParent = Random.Range(0, sortedList.Count);
            }

            population.Add(Breed(sortedList[i], sortedList[otherParent], i - otherParent));
            population.Add(Breed(sortedList[otherParent], sortedList[i], otherParent - i));
        }

        for (int i = 0; i < sortedList.Count; i++)
            Destroy(sortedList[i]);

        generation++;
    }

    GameObject Breed(GameObject parent1, GameObject parent2, int indexDiff)
    {
        GameObject offSpring = InstantiatePerson();
        DNA osDNA = offSpring.GetComponent<DNA>();
        DNA dna1 = parent1.GetComponent<DNA>();
        DNA dna2 = parent2.GetComponent<DNA>();

        float firstParentBias = (indexDiff + populationSize / 2f * 1.1f) / (populationSize * 1.1f);
        Debug.Log(indexDiff + " + " + (populationSize / 2f * 1.1f) + " / " + (populationSize * 1.1f) + " = " + firstParentBias);

        osDNA.rgbs = new float[] {
            Random.Range(0f, 1f) < firstParentBias ? dna1.rgbs[0] : dna2.rgbs[0],
            Random.Range(0f, 1f) < firstParentBias ? dna1.rgbs[1] : dna2.rgbs[1],
            Random.Range(0f, 1f) < firstParentBias ? dna1.rgbs[2] : dna2.rgbs[2],
            Random.Range(0f, 1f) < firstParentBias ? dna1.rgbs[3] : dna2.rgbs[3]
        };

        //Chance for mutation to occur
        bool mutation = Random.Range(0f, 1f) < 0.20f;

        if (mutation) {
            int randomColorChange = Random.Range(0, 4);

            if (randomColorChange < 3)
                osDNA.rgbs[randomColorChange] = Mathf.Clamp01(osDNA.rgbs[randomColorChange] + Random.Range(-0.35f, 0.35f));
            else
                osDNA.rgbs[randomColorChange] = Mathf.Clamp(osDNA.rgbs[randomColorChange] + Random.Range(-0.15f, 0.15f), minSize, maxSize);
        }

        return offSpring;
    }

    GameObject InstantiatePerson()
    {
        Vector3 pos = new Vector3(Random.Range(-9, 9), Random.Range(-4.5f, 4.5f), 0);

        GameObject go = Instantiate(personPrefab, pos, Quaternion.identity);

        return go;
    }

    private void OnGUI()
    {
        guiStyle.fontSize = 50;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10, 10, 100, 20), "Generation = " + generation, guiStyle);
        GUI.Label(new Rect(10, 65, 100, 20), "Trial Time = " + (int)elapsed, guiStyle);
    }
}
