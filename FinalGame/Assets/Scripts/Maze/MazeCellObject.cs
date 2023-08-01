using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * b3agz's video: https://www.youtube.com/watch?v=TMOEYdV4Ot4&t=1495s&ab_channel=b3agz
 * This is just the cell object that the maze generator and renderer will use
 */

public class MazeCellObject : MonoBehaviour
{
    [SerializeField] GameObject topWall;
    [SerializeField] GameObject bottomWall;
    [SerializeField] GameObject leftWall;
    [SerializeField] GameObject rightWall;

    public int xCoor;
    public int yCoor;

    public void Init(bool top, bool bottom, bool right, bool left, int x, int y)
    {
        topWall.SetActive(top);
        bottomWall.SetActive(bottom);
        leftWall.SetActive(left);
        rightWall.SetActive(right);
        xCoor = x;
        yCoor = y;
    }
}
