using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public int weight;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;
    //private BackgroundTile[,] allTiles;
    
    void Start()
    {
        //allTiles = new BackgroundTile[weight, height];
        allDots = new GameObject[weight, height];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < weight; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i,j);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition,quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";
                int dotToUse = Random.Range(0, dots.Length);
                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";
                allDots[i, j] = dot;
            }
        }
    }
   
    void Update()
    {
        
    }
}
