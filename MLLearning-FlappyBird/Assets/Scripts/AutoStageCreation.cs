using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoStageCreation : MonoBehaviour
{
    public GameObject upArrowPrefab;
    public GameObject downArrowPrefab;

    private Vector3 startPos;
    private readonly float distanceInbetween = 3;
    private float speed = 0.0075f;
    private float deltaDistance = 0;
    private List<GameObject> arrows = new List<GameObject>();

    private void Start()
    {
        startPos = transform.position;

        Init();
    }

    void Update()
    {
        transform.Translate(speed, 0, 0);

        deltaDistance += speed;

        if (deltaDistance >= distanceInbetween)
        {
            CreateNextArrowSet(18);
            deltaDistance = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        string other = collision.gameObject.tag;

        if (other == "downwall" || other == "upwall")
            Destroy(collision.gameObject);
    }

    /************************* local methods ************************/
    public void Init()
    {
        CreateNextArrowSet(6);
        CreateNextArrowSet(9);
        CreateNextArrowSet(12);
        CreateNextArrowSet(15);
        CreateNextArrowSet(18);
    }
    
    private void CreateNextArrowSet(float distanceAhead)
    {
        float center = Random.Range(0.5f, 2.5f);

        GameObject downArrow = Instantiate(downArrowPrefab);
        GameObject upArrow = Instantiate(upArrowPrefab);

        downArrow.transform.position = transform.position;
        upArrow.transform.position = transform.position;

        downArrow.transform.Translate(-distanceAhead, -(center + 4), 0);
        upArrow.transform.Translate(distanceAhead, center - 4, 0);

        arrows.Add(downArrow);
        arrows.Add(upArrow);
    }

    public void Reset()
    {
        transform.position = startPos;
        foreach (GameObject obj in arrows) Destroy(obj);
        deltaDistance = 0;
        speed = 0.01f;

        Init();
    }

    public void SpeedUp()
    {
        speed = Mathf.Min(1f, speed + 0.001f);
    }

    public void SlowDown()
    {
        speed = Mathf.Max(0.0075f, speed - 0.001f);
    }
}
