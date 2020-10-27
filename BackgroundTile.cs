using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTile : MonoBehaviour
{
    public GameObject[] dots;
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
       int kaktak = 3;
    }

    void Initialize()
    {
        int dotToUse = Random.Range(0,1);
    }
}
