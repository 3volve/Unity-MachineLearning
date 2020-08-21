using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public static int numDead = 0;
    public float trialTime = 10;
    int generation = 1;

    GUIStyle guiStyle = new GUIStyle();
    void Start()
    {
        for(int i = 0; i < populationSize; i++)
        {
            GameObject ethan = InstantiateEthan();
            ethan.GetComponent<Brain>().Init();
            population.Add(ethan);
        }
    }
    
    void Update()
    {
        elapsed += Time.deltaTime;

        if(/*numDead >= populationSize / 2f - 1 || */elapsed >= trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
            numDead = 0;
        }
    }
    
    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.Label(new Rect(10, 100, 200, 30), "Left Alive: " + (population.Count - numDead), guiStyle);
        GUI.EndGroup();
    }

    /************************ Local Methods **************************/
    
    private void BreedNewPopulation()
    {
        //Get rid of unfit individuals
        List<Brain> sortedList = population.Select(o => o.GetComponent<Brain>()).ToList();
        sortedList = sortedList.OrderBy(b => b.timeAlive).ThenBy(b => b.distanceFromStart).ToList();
        population.Clear();

        int halfOfList = (int)(sortedList.Count / 2.0f);
        List<Brain> culledList = new List<Brain>();

        //Cull bottom half of sorted list
        for (int i = 0; i < sortedList.Count; i++)
        {
            if (sortedList[i].alive) culledList.Add(sortedList[i]);
            else Destroy(sortedList[i].gameObject);
        }

        halfOfList = culledList.Count - halfOfList;

        for (int i = 0; i < halfOfList; i++)
        {
            Destroy(culledList[i].gameObject);
            culledList.RemoveAt(i);
        }

        foreach (Brain b in culledList)
            Debug.Log("(" + culledList.IndexOf(b) + ") Time: " + b.timeAlive + ", Distance: " + b.distanceFromStart + ", alive? " + b.alive);
        
        //Breed the remainder
        for (int i = 0; i < culledList.Count; i++)
        {
            int otherParent = Random.Range(0, culledList.Count);

            bool fitWeight = Random.Range(0f, 1f) < 0.20f;

            while (otherParent == i)
            {
                if (fitWeight && culledList.Count > 2)
                    otherParent = Random.Range((int)(culledList.Count / 2.0f) - 1, culledList.Count);
                else
                    otherParent = Random.Range(0, culledList.Count);
            }

            population.Add(Breed(culledList[i], culledList[otherParent], i - otherParent));
            population.Add(Breed(culledList[otherParent], culledList[i], otherParent - i));
        }

        while(population.Count < populationSize)
        {
            population.Add(Breed(culledList[0], culledList[1], 0));
            population.Add(Breed(culledList[1], culledList[0], 0));
        }

        for (int i = 0; i < culledList.Count; i++)
            Destroy(culledList[i].gameObject);

        generation++;
    }

    private GameObject Breed(Brain parent1, Brain parent2, int indexDiff)
    {
        GameObject offSpring = InstantiateEthan();
        Brain b = offSpring.GetComponent<Brain>();
        b.Init();

        float firstParentBias = (indexDiff + populationSize / 2f * 1.1f) / (populationSize * 1.1f);

        b.dna.Combine(parent1.dna, parent2.dna);

        if (Random.Range(0, 100) == 0) b.dna.Mutate();


        return offSpring;
    }


    /************************ Helper Methods ***********************/
    private GameObject InstantiateEthan()
    {
        Vector3 startingPos = new Vector3(
                transform.position.x + Random.Range(-2, 2),
                transform.position.y,
                transform.position.z + Random.Range(-2, 2)
            );

        return Instantiate(botPrefab, startingPos, transform.rotation);
    }
}
