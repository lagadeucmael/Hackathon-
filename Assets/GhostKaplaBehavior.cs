using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostKaplaBehavior : MonoBehaviour
{

    protected GameObject[] allGOs;
    protected List<GameObject> kaplas;

    public int KaplasLayer = 8;

    public double distanceThreshold = 0.1;
    public double angleThreshold = 0.1;

    public bool success = false;

    protected Color myGreen;
    protected Color myBlue;
    // Start is called before the first frame update
    void Start()
    {
        myGreen.r = 0.0f;
        myGreen.g = 1.0f;
        myGreen.b = 0.0f;
        myGreen.a = 0.1f;

        myBlue.r = 0.0f;
        myBlue.g = 0.0f;
        myBlue.b = 1.0f;
        myBlue.a = 0.1f;
    }

    protected bool Filled()
    {
        allGOs = FindObjectsOfType<GameObject>();
        kaplas = new List<GameObject>();
        for (var i = 0; i < allGOs.Length; i++)
        {
            if (allGOs[i].layer == KaplasLayer)
            {
                kaplas.Add(allGOs[i]);
            }
        }
        var tab = kaplas.ToArray();
        
        for (var i = 0; i < tab.Length; i++)
        {
            Vector3 v = this.transform.position - tab[i].transform.position;
            if (v.magnitude < distanceThreshold)
            {
                float angle = Vector3.Dot(this.transform.up, tab[i].transform.up) + Vector3.Dot(this.transform.forward, tab[i].transform.forward);
                if (angle > angleThreshold)
                {
                    return true;
                }
            }
        }
        return false;
    }

    // Update is called once per frame
    void Update()
    {
        success = this.Filled();
        var cubeRenderer = this.GetComponent<Renderer>();
        if (success)
        {
            cubeRenderer.material.SetColor("_Color", myGreen);
        } else
        {
            cubeRenderer.material.SetColor("_Color", myBlue);

        }
    }
}
