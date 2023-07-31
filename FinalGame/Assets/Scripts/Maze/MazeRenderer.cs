using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * b3agz's video: https://www.youtube.com/watch?v=TMOEYdV4Ot4&t=1495s&ab_channel=b3agz
 * This is just the program that instantiates and draws all the cells and stuff. All of the maze logic is in the maze generator class.
 */

public class MazeRenderer : MonoBehaviour
{
    System.Random gen = new System.Random();
    [SerializeField] MazeGenerator mazeGenerator;
    [SerializeField] GameObject[] MazeCellPrefabs = new GameObject[4];
    [SerializeField] int[] weights = new int[4];  //last value always at 0
    public float CellSize = 1f;
    private void Awake()
    {
        MazeCell[,] maze = mazeGenerator.GetMaze();
        for (int x = 0; x < mazeGenerator.mazeWidth; x++)
        { 
            for (int y = 0; y < mazeGenerator.mazeHeight; y++) 
            {
                GameObject prefab = null;
                int rand = gen.Next(1, 101);
                for (int i = 0; i < weights.Length; i++)
                {
                    if (rand > weights[i])
                    {
                        prefab = MazeCellPrefabs[i];
                        break;
                    }
                }
                
                GameObject newCell = Instantiate(prefab, new Vector3((float)x * CellSize, 0f, (float)y * CellSize), Quaternion.identity, transform);
                MazeCellObject mazeCell = newCell.GetComponent<MazeCellObject>();
                bool top = maze[x, y].topWall;
                bool left = maze[x, y].leftWall;
                bool right = false;
                bool bottom = false;
                if (x == mazeGenerator.mazeWidth - 1) right = true;
                if (y == 0) bottom = true;
                mazeCell.Init(top, bottom, right, left);
            }
        }
    }
}
