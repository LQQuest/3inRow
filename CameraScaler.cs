using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class CameraScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;
    
    void Start()
    {
        board = FindObjectOfType<Board>();
        if (board != null)
        {
            RepositionCamera(board.wight - 1, board.height - 1);
        }
    }

    void RepositionCamera(float x, float y)
    {
        Vector3 tempPosition = new Vector3(x/2, y/2, cameraOffset);
        transform.position = tempPosition;
        if (board.wight >= board.height)
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            Camera.main.orthographicSize = (board.wight / 2 + padding) / aspectRatio;
        }
        else
        {
            Debug.Assert(Camera.main != null, "Camera.main != null");
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
}
