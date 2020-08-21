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
    public int populationSize = 50;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public static int numDead = 0;
    public float trialTime = 10;

    private int generation = 1;
    private float cutoffDivisor = 10;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle buttonStyle = new GUIStyle();

    void Start()
    {
        while(population.Count < populationSize)
        {
            GameObject bot = InstantiateBot();
            bot.GetComponent<Brain>().Init();
            population.Add(bot);
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed >= trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }

    private void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;

        buttonStyle = GUI.skin.button;
        buttonStyle.fontSize = 20;
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.normal.background = Texture2D.blackTexture;

        GUI.BeginGroup(new Rect(10, 10, 250, 200));

        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);

        GUI.Label(new Rect(10, 50, 200, 30), "Population: " + population.Count, guiStyle);
        if (GUI.Button(new Rect(20, 75, 150, 25), "Population++", buttonStyle)) populationSize += 10;
        if (GUI.Button(new Rect(20, 100, 150, 25), "Population-- ", buttonStyle)) populationSize -= 10;

        GUI.Label(new Rect(10, 125, 200, 30), string.Format("Time: {0:0.00}" + " out of " + trialTime, elapsed), guiStyle);
        if (GUI.Button(new Rect(20, 150, 150, 25), "TrialTime++", buttonStyle)) trialTime++;
        if (GUI.Button(new Rect(20, 175, 150, 25), "TrialTime-- ", buttonStyle)) trialTime--;

        GUI.EndGroup();

        GUI.BeginGroup(new Rect(10, 240, 375, 200));

        GUI.Box(new Rect(0, 0, 140, 140), "Agent's \"Goal\":", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "To travel as far as possible from", guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), "their starting point.", guiStyle);

        GUI.EndGroup();
        GUI.BeginGroup(new Rect(10, 330, 375, 200));

        GUI.Box(new Rect(0, 0, 140, 140), "Agent's Senses:", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "- A short raycast in front of them tellin them", guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), "- Detection if they've stopped", guiStyle);

        GUI.EndGroup();
    }

    /************************ Local Methods **************************/

    private void BreedNewPopulation()
    {
        //Get rid of unfit individuals
        List<Brain> sortedList = population.Select(o => o.GetComponent<Brain>()).ToList();
        sortedList = sortedList.OrderByDescending(b => b.timeAlive).ThenByDescending(b => b.maxDistance).ToList();
        population.Clear();

        int halfOfList = Mathf.Min((int)(sortedList.Count / 2.0f), (int)(populationSize / 2.0f));
        List<Brain> culledList = new List<Brain>();

        //Cull dead half of sorted list
        for (int i = sortedList.Count - 1; i >= Mathf.Max(sortedList.Count - populationSize, 0); i--)
        {
            if (sortedList[i].alive && sortedList[i].maxDistance > 1) culledList.Add(sortedList[i]);
            else Destroy(sortedList[i].gameObject);
        }

        halfOfList = culledList.Count - halfOfList;
        //Debug.Log("0 index: " + culledList[0].alive + "/" + culledList[0].maxDistance + ", last index: " + culledList[culledList.Count - 1].alive + "/" + culledList[culledList.Count - 1].maxDistance);

        foreach (Brain b in culledList)
            if (b.alive == false) Debug.Log("Found a dead one");
        
        //Cull until only half left
        for (int i = 0; i < halfOfList; i++)
        {
            Destroy(culledList[i].gameObject);
            culledList.RemoveAt(i);
        }

        //Breed the remainder
        for (int i = culledList.Count - 1; i > 0; i--)
        {
            int otherParent = i - 1;
            bool potentialClassGapLove = Random.Range(0f, 1f) < 0.25f;

            if (potentialClassGapLove)
            {
                otherParent = Random.Range(0, culledList.Count);

                bool fitWeight = Random.Range(0f, 1f) < 0.20f;

                while (otherParent == i)
                {
                    if (fitWeight && culledList.Count > 2)
                        otherParent = Random.Range((int)(culledList.Count / 2.0f) - 1, culledList.Count);
                    else
                        otherParent = Random.Range(0, culledList.Count);
                }
            }

            population.Add(Breed(culledList[i], culledList[otherParent], i - otherParent));
            population.Add(Breed(culledList[otherParent], culledList[i], otherParent - i));
        }

        while (population.Count < populationSize)
        {
            GameObject bot = InstantiateBot();
            bot.GetComponent<Brain>().Init();
            population.Add(bot);
        }

        for (int i = 0; i < culledList.Count; i++)
            Destroy(culledList[i].gameObject);
        for (int i = 0; i < sortedList.Count; i++)
            if (sortedList[i] != null) Destroy(sortedList[i].gameObject);
        generation++;
    }

    private GameObject Breed(Brain parent1, Brain parent2, int indexDiff)
    {
        GameObject offSpring = InstantiateBot();
        Brain b = offSpring.GetComponent<Brain>();
        b.Init();

        float firstParentBias = (indexDiff + populationSize / 2f * 1.1f) / (populationSize * 1.1f);

        b.dna.Combine(parent1.dna, parent2.dna);

        b.dna.Mutate();
        
        return offSpring;
    }


    /************************ Helper Methods ***********************/
    private GameObject InstantiateBot()
    {
        Vector3 startingPos = new Vector3(
                transform.position.x + Random.Range(-17, 17),
                transform.position.y,
                transform.position.z + Random.Range(-17, 17)
            );

        Quaternion startingRot = transform.rotation * Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up);

        return Instantiate(botPrefab, startingPos, startingRot);
    }
}
