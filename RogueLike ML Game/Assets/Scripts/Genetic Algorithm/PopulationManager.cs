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
    public GameObject player;
    public int populationSize = 10;
    List<GameObject> population = new List<GameObject>();
    public static float elapsed = 0;
    public static int numDead = 0;
    public Rect spawnBox;

    private int generation = 1;
    private GUIStyle guiStyle = new GUIStyle();
    private GUIStyle buttonStyle = new GUIStyle();

    private readonly float maxSurvivalPercentage = 0.5f;

    /************************* Unity Functions *************************/

    void Start()
    {
        while (population.Count < populationSize)
            population.Add(InstantiateBot().GetComponent<Brain>().Init(player.transform));
    }

    void Update()
    {
        if (FindObjectOfType<Pause>().Paused) return;
        elapsed += Time.deltaTime;

        if (numDead == populationSize /*|| player.GetComponent<Player>().Dead*/)
        {
            BreedNewPopulation();
            elapsed = 0;
            numDead = 0;
        }


    }

    /*************** GUI ***************
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

        GUI.Label(new Rect(10, 150, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);

        GUI.EndGroup();

        /* For Descriptions if I make it into a demonstration game
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
        /*
    }

    /************************ Main Breeding Functions **************************/

    private void BreedNewPopulation()
    {
        List<GameObject> agentList = FitnessFunction();
        List<Brain> fitList = agentList.Select(o => o.GetComponent<Brain>()).ToList();
        population.Clear();

        //add back population that is being kept
        population.AddRange(agentList);
        foreach (GameObject obj in population)
            obj.GetComponent<Brain>().Reset();

        //Just in case there's only one or less left
        if (fitList.Count == 0)
            fitList.Add(InstantiateBot().GetComponent<Brain>().InitBrain(player.transform));
        if (fitList.Count == 1)
            fitList.Add(InstantiateBot().GetComponent<Brain>().InitBrain(player.transform));

        /* Printing List Debug Statement 
        foreach (Brain b in fitList)
            Debug.Log("sorted: (" + sortedList.IndexOf(b) + ") damage: " + b.damageDealt + ", time alive: " + b.timeAlive);
        /**/
        int counter = 0;

        //Breed the best performers
        for (; counter < Mathf.Max((int)(populationSize * maxSurvivalPercentage * 0.25f), 1); counter++)
        {
            //give a (currently 25%) chance to jump classes of successful
            //since there's always the possability of the princess with the peasant
            bool potentialClassGapLove = Random.Range(0f, 1f) < 0.20f;
            
            Brain parent1 = fitList[counter];
            Brain parent2 = potentialClassGapLove ? fitList[ClassGap(counter + 1)] : fitList[counter + 1];

            population.Add(Breed(parent1, parent2));
        }

        //mutate some as well
        for (; counter < fitList.Count - 1; counter++)
        {
            population.Add(Mutate(fitList[counter]));
        }

        //Add extra Randoms until it has a full population again
        while (population.Count < populationSize)
        {
            population.Add(InstantiateBot().GetComponent<Brain>().Init(player.transform));
        }
        
        //Destroy everything
        for (int i = 0; i < fitList.Count; i++)

        generation++;
    }

    private GameObject Breed(Brain parent1, Brain parent2)
    {
        GameObject offSpring = InstantiateBot();
        Brain b = offSpring.GetComponent<Brain>().InitBrain(player.transform);

        //if I want to add bias towards more fit agents
        //float firstParentBias = (indexDiff + populationSize / 2f * 1.1f) / (populationSize * 1.1f);
        
        b.preferenceDNA.Combine(parent1.preferenceDNA, parent2.preferenceDNA);
        b.shootingDNA.Combine(parent1.shootingDNA, parent2.shootingDNA);
        b.movingDNA.Combine(parent1.movingDNA, parent2.movingDNA);
        b.strafingDNA.Combine(parent1.strafingDNA, parent2.strafingDNA);

        return offSpring;
    }

    private GameObject Mutate(Brain parent)
    {
        GameObject offSpring = InstantiateBot();
        Brain b = offSpring.GetComponent<Brain>().InitBrain(player.transform);

        b.preferenceDNA.Mutate(parent.preferenceDNA);
        b.shootingDNA.Mutate(parent.shootingDNA);
        b.movingDNA.Mutate(parent.movingDNA);
        b.strafingDNA.Mutate(parent.strafingDNA);

        return offSpring;
    }

    /**************************** Support Functions ******************************/

    private List<GameObject> FitnessFunction()
    {
        //grab all the brains out of the population
        List<GameObject> sortedList = population;

        //sort unfit agents towards the bottom (descending: most damage dealt -> least damage dealt)
        sortedList = sortedList.OrderByDescending(b => b.GetComponent<Brain>().damageDealt)
            .ThenByDescending(b => b.GetComponent<Brain>().timeAlive)
            .ToList();

        /* Printing List Debug Statement
        foreach (GameObject obj in sortedList)
            Debug.Log("sorted: (" + sortedList.IndexOf(obj) + ") damage: " + obj.GetComponent<Brain>().damageDealt + ", time alive: " + obj.GetComponent<Brain>().timeAlive);
        /**/
        int survivors = (int)(populationSize * maxSurvivalPercentage);

        DeleteFailures(sortedList, survivors);

        return sortedList;
    }

    private void DeleteFailures(List<GameObject> list, int survivors)
    {
        for(int i = list.Count - 1; i >= survivors; i--)
        {
            Destroy(list[i]);
            list.Remove(list[i]);
        }
    }

    private int ClassGap(int currentParent)
    {
        int otherParent = Random.Range(0, (int)(populationSize * maxSurvivalPercentage));

        //even then still give 20% preference to upper echelons of successful
        bool fitWeight = Random.Range(0f, 1f) < 0.20f;

        //but only if it by chance it tries to breed the agent with itself
        while (otherParent == currentParent)
        {
            if (fitWeight && (int)(populationSize * maxSurvivalPercentage) > 2)
                otherParent = Random.Range(0, (int)(populationSize * maxSurvivalPercentage * 0.25f));
            else
                otherParent = Random.Range(0, (int)(populationSize * maxSurvivalPercentage));
        }

        return otherParent;
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
