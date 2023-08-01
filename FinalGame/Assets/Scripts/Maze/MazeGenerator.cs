using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;

using UnityEngine;

/*
 * b3agz's video: https://www.youtube.com/watch?v=TMOEYdV4Ot4&t=1495s&ab_channel=b3agz
 * This is the generator for the maze. The maze is generated by:
 * - Creating a grid of cells
 * - Having an algorithm randomly select directions to go in until it hits a dead end
 * - Backtrack to a spot that has available cells around it, and goes that way
 * - Repeat the two steps above this until there is no other paths left
 */

public class MazeGenerator : MonoBehaviour
{
    [Range(5, 100)]
    public int mazeWidth = 5, mazeHeight = 5;
    //Where the maze algorithm will start from
    public int startX, startY;
    public MazeCell[,] maze; //2d array of cells

    Vector2Int currentCell; //The current cell we are looking at
    public MazeCell[,] GetMaze()
    {
        maze = new MazeCell[mazeWidth, mazeHeight];
        for (int i = 0; i < mazeWidth; i++)
        {
            for (int j = 0; j < mazeHeight; j++)
            {
                maze[i, j] = new MazeCell(i, j);
            }
        }
        CarvePath(startX, startY);
        return maze;
    }
    List<Direction> directions = new List<Direction> {
        Direction.Up, Direction.Down, Direction.Left, Direction.Right,
    };

    List<Direction> GetRandomDirections()
    {
        //Make a copy of our directions list that we can mess around with
        List<Direction> dir = new List<Direction>(directions);
        //Make a directions list to put our randomised directions into.
        List<Direction> rndDir = new List<Direction>();
        while (dir.Count > 0) 
        {
            int rnd = Random.Range(0, dir.Count);
            rndDir.Add(dir[rnd]);
            dir.RemoveAt(rnd);
        }
        return rndDir;
    }
    bool IsCellValid(int x, int y)
    {
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1 || maze[x, y].visited) return false;
        else return true;
    }
    Vector2Int CheckNeighbours() //The guy I got the tutorial from was probably British or something, hence the spelling
    { 
        //returns the nearest neighbor that was not visited; if there is none, it returns itself, thus we know that it is a dead end
        List<Direction> rndDir = GetRandomDirections();
        for (int i = 0; i < rndDir.Count; i++) 
        {
            Vector2Int neighbour = currentCell;
            switch (rndDir[i])
            {
                case Direction.Up:
                    neighbour.y++;
                    break;
                case Direction.Down:
                    neighbour.y--;
                    break;
                case Direction.Left:
                    neighbour.x--;
                    break;
                case Direction.Right:
                    neighbour.x++;
                    break;
            }
            if (IsCellValid(neighbour.x, neighbour.y)) return neighbour;
        }
        return currentCell;
    }
    void BreakWalls(Vector2Int primaryCell, Vector2Int secondaryCell)
    {
        //Pass in two cells, and the algorithm tells us which walls to break down; using two cells just to make life easier
        if (primaryCell.x > secondaryCell.x) //Primary cell is on the right, so its left wall needs to be broken down
        {
            maze[primaryCell.x, primaryCell.y].leftWall = false;
        }
        else if (primaryCell.x < secondaryCell.x)
        {
            maze[secondaryCell.x, secondaryCell.y].leftWall = false;
        }
        else if (primaryCell.y < secondaryCell.y)
        {
            maze[primaryCell.x, primaryCell.y].topWall = false;
        }
        else if (primaryCell.y > secondaryCell.y)
        {
            maze[secondaryCell.x, secondaryCell.y].topWall = false;
        }

    }
    void CarvePath(int x, int y)
    {
        //x and y are starting pos
        if (x < 0 || y < 0 || x > mazeWidth - 1 || y > mazeHeight - 1)
        {
            x = y = 0;
            Debug.LogWarning("Starting position is out of bounds, defaulting to 0, 0");
        }
        currentCell = new Vector2Int(x, y);
        List<Vector2Int> path = new List<Vector2Int>();
        bool deadEnd = false;
        while (!deadEnd) 
        {
            // Get the next cell we're going to try
            Vector2Int nextCell = CheckNeighbours();
            //If that cell has no valid neighbours, set deadend to true so we break out of the loop
            if (nextCell == currentCell)
            {
                for (int i = path.Count - 1; i >= 0; i--) //have to loop backward; the current cell is at the end of the list
                {
                    currentCell = path[i];
                    path.RemoveAt(i);
                    nextCell = CheckNeighbours();
                    if (nextCell != currentCell) break; //we backtracked to a cell that has a current neighbour

                }
                if (nextCell == currentCell)
                {
                    deadEnd = true;
                }
            }
            else
            {
                BreakWalls(currentCell, nextCell);
                maze[currentCell.x, currentCell.y].visited = true;
                currentCell = nextCell;
                path.Add(currentCell);
            }
        }
    }
}

public enum Direction
{ 
    Up,
    Down,
    Left,
    Right
}

public class MazeCell
{
    //Tells if the navigator went through this cell or not
    public bool visited;
    // coords
    public int x, y;
    // we only need 2 walls; the top and left wall of one cell are the bottom and right of another
    public bool topWall;
    public bool leftWall;
    //Return x and y as a Vector2Int for convenience
    public Vector2Int position
    {
        get { return new Vector2Int(x, y); }
    }
    public MazeCell(int x, int y)
    {
        //Coords of the cell in the maze grid
        this.x = x;
        this.y = y;
        //Whether the algorithm has visited this cell
        visited = false;
        //All walls are present until the algorithm removes them
        topWall = leftWall = true;
    }
}