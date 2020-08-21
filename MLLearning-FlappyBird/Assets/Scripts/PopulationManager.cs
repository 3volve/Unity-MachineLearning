using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;

/*
 * This is for personal learning and testing use only.
 *
 */

public class PopulationManager : MonoBehaviour
{
    public GameObject botPrefab;
    public AutoStageCreation creationDestruction;
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public static int numDead = 0;
    public float trialTime = 5;
    public Rect spawnBox;

    private int generation = 1;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle buttonStyle = new GUIStyle();

    void Start()
    {
        botPrefab.GetComponent<Brain>().poof.GetComponent<AudioSource>().volume = 0.075f;

        while (population.Count < populationSize)
            population.Add(InstantiateBot().GetComponent<Brain>().Init());
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (numDead == populationSize || elapsed >= trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
            numDead = 0;
        }

        float furthestPosition = 0;

        foreach (GameObject obj in population)
        {
            Brain b = obj.GetComponent<Brain>();

            if (b.alive && b.running && b.transform.position.x > furthestPosition)
                furthestPosition = b.transform.position.x;
        }

        if (furthestPosition - creationDestruction.transform.position.x > 14)
            creationDestruction.SpeedUp();
        if (furthestPosition - creationDestruction.transform.position.x < 13)
            creationDestruction.SlowDown();
    }

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;

        buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 20;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = Texture2D.blackTexture;

        GUI.BeginGroup(new Rect(10, 30, 250, 300));

        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);

        GUI.Label(new Rect(10, 50, 200, 30), "Population: " + population.Count, guiStyle);
        if (GUI.Button(new Rect(20, 75, 150, 25), "Population++", buttonStyle)) populationSize += 10;
        if (GUI.Button(new Rect(20, 100, 150, 25), "Population-- ", buttonStyle)) populationSize -= 10;
        GUI.Label(new Rect(10, 125, 200, 30), "Still Alive: " + (populationSize - numDead), guiStyle);

        GUI.Label(new Rect(10, 150, 200, 30), string.Format("Time: {0:0.00}" + " out of " + trialTime, elapsed), guiStyle);
        if (GUI.Button(new Rect(20, 175, 150, 25), "TrialTime++", buttonStyle)) trialTime++;
        if (GUI.Button(new Rect(20, 200, 150, 25), "TrialTime-- ", buttonStyle)) trialTime--;
        if (GUI.Button(new Rect(25, 240, 200, 25), "Turn Off Sounds", buttonStyle))
            botPrefab.GetComponent<Brain>().poof.GetComponent<AudioSource>().volume = 0;

        GUI.EndGroup();

        //GUI.BeginGroup(new Rect(10, 240, 375, 200));

        //GUI.Box(new Rect(0, 0, 140, 140), "Agent's \"Goal\":", guiStyle);
        //GUI.Label(new Rect(10, 25, 200, 30), "To travel as far as possible from", guiStyle);
        //GUI.Label(new Rect(10, 50, 200, 30), "their starting point.", guiStyle);

        //GUI.EndGroup();
        //GUI.BeginGroup(new Rect(10, 330, 375, 200));

        //GUI.Box(new Rect(0, 0, 140, 140), "Agent's Senses:", guiStyle);
        //GUI.Label(new Rect(10, 25, 200, 30), "- A short raycast in front of them tellin them", guiStyle);
        //GUI.Label(new Rect(10, 50, 200, 30), "- Detection if they've stopped", guiStyle);

        //GUI.EndGroup();
    }

    /************************ Local Methods **************************/

    private void BreedNewPopulation()
    {
        
        //Get rid of unfit individuals
        List<Brain> sortedList = population.Select(o => o.GetComponent<Brain>()).ToList();
        sortedList = sortedList.OrderBy(b => b.distanceTravelled - b.crash * 0.25f)
            .ToList();
        population.Clear();

        foreach (Brain b in sortedList)
            Debug.Log("sorted: (" + sortedList.IndexOf(b) + ") time: " + Mathf.Round(b.timeAlive * 10) / 10 + ", distance: " + b.distanceTravelled + ", w/ crashes: " + (b.distanceTravelled - b.crash * 0.25f));

        int quarterOfList = Mathf.Min((int)(sortedList.Count / 4.0f), (int)(populationSize / 4.0f)); //50
        List<Brain> culledList = new List<Brain>();
        sortedList.Reverse();

        culledList = sortedList;
        //Cull dead half of sorted list
        //for (int i = sortedList.Count - 1; i >= Mathf.Max(sortedList.Count - populationSize, 0); i--) //199 -> 0
        //    if (sortedList[i].alive)
        //        culledList.Add(sortedList[i]);

        List<Brain> finalList = new List<Brain>();

        //Cull until only half left
        for (int i = 0; i < Mathf.Min(quarterOfList, culledList.Count); i++)
            finalList.Add(culledList[i]);
        
        //Just in case there's only one or less left
        if (finalList.Count == 0)
            finalList.Add(InstantiateBot().GetComponent<Brain>().InitBrain());
        if (finalList.Count == 1)
            finalList.Add(InstantiateBot().GetComponent<Brain>().InitBrain());

        foreach (Brain b in finalList)
            Debug.Log("final: (" + finalList.IndexOf(b) + ") time: " + Mathf.Round(b.timeAlive * 10) / 10 + ", distance: " + b.distanceTravelled + ", w/ crashes: " + (b.distanceTravelled - b.crash * 0.5f));
        //Breed the remainder
        for (int i = finalList.Count - 1; i > 0; i--)
        {
            int otherParent = i - 1;
            bool potentialClassGapLove = Random.Range(0f, 1f) < 0.25f;

            //give a (currently 25%) chance to jump classes of successful
            //since there's always the possability of the princess with the peasant
            if (potentialClassGapLove)
            {
                otherParent = Random.Range(0, finalList.Count);
                
                //even then still give 20% preference to upper echelons of successful
                bool fitWeight = Random.Range(0f, 1f) < 0.20f;

                //but only if it by chance it tries to breed the agent with itself
                while (otherParent == i)
                {
                    if (fitWeight && finalList.Count > 2)
                        otherParent = Random.Range(0, (int)(finalList.Count / 2.0f));
                    else
                        otherParent = Random.Range(0, finalList.Count);
                }
            }

            population.Add(Breed(finalList[i], finalList[otherParent], i - otherParent));
            population.Add(Breed(finalList[otherParent], finalList[i], otherParent - i));
            population.Add(Breed(finalList[i], finalList[otherParent], i - otherParent));
            population.Add(Breed(finalList[otherParent], finalList[i], otherParent - i));
        }
        

        //Breeds more if too many died
        while (population.Count < populationSize)
        {
            int parent1 = Random.Range(0, finalList.Count);
            int parent2 = Random.Range(0, finalList.Count);
            int counter = 0;

            while (parent1 == parent2)
            {
                parent2 = Random.Range(0, finalList.Count);
                if (++counter > 10) break;
            }

            population.Add(Breed(finalList[parent1], finalList[parent2], parent1 - parent2));
            population.Add(Breed(finalList[parent2], finalList[parent1], parent2 - parent1));
        }
        

        //Destroy everything
        for (int i = 0; i < sortedList.Count; i++)
            Destroy(sortedList[i]?.gameObject);

        generation++;

        creationDestruction.Reset();
    }

    private GameObject Breed(Brain parent1, Brain parent2, int indexDiff)
    {
        GameObject offSpring = InstantiateBot();
        Brain b = offSpring.GetComponent<Brain>().InitBrain();

        //if I want to add bias towards more fit agents
        float firstParentBias = (indexDiff + populationSize / 2f * 1.1f) / (populationSize * 1.1f);

        b.dna.Combine(parent1.dna, parent2.dna);

        //My mutate has the mutation chance decided by the DNA and not the population
        b.dna.Mutate();
        
        return offSpring;
    }


    /************************ Helper Methods ***********************/
    private GameObject InstantiateBot()
    {
        Vector3 startingPos = new Vector3(
                transform.position.x + Random.Range(spawnBox.xMin, spawnBox.xMax),
                transform.position.y + Random.Range(spawnBox.yMin, spawnBox.yMax),
                transform.position.z
            );

        //Quaternion startingRot = transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

        return Instantiate(botPrefab, startingPos, transform.rotation);
    }
}
