using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public Material green;
    public Material red;

    Perceptron p;

    void Start()
    {
        p = GetComponent<Perceptron>();
    }

    void Update()
    {
        GameObject obj = null;

        bool k1 = Input.GetKeyDown("1");
        bool k2 = Input.GetKeyDown("2");
        bool k3 = Input.GetKeyDown("3");
        bool k4 = Input.GetKeyDown("4");

        if (k1 || k2)
        {
            obj = Instantiate(spherePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
            obj.GetComponent<Renderer>().material = k1 ? red : green;

            if (k1) p.SendInput(0, 0, 0);
            else p.SendInput(0, 1, 1);
        }
        else if (k3 || k4)
        {
            obj = Instantiate(cubePrefab, Camera.main.transform.position, Camera.main.transform.rotation);
            obj.GetComponent<Renderer>().material = k3 ? red : green;

            if (k3) p.SendInput(1, 0, 1);
            else p.SendInput(1, 1, 1);
        }
        
        obj?.GetComponent<Rigidbody>().AddForce(0, 0, 500);
    }
}
