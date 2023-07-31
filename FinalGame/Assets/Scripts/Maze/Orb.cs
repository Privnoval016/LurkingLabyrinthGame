using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0, 50)]
    [SerializeField] int amount = 10;
    [Range(0, 50)]
    [SerializeField] int range = 5;
    System.Random gen = new System.Random();
    MazeGenerator m_generator;
    MazeRenderer m_renderer;
    List<GameObject> orbs = new List<GameObject>();
    [SerializeField] GameObject orbPrefab;
    void Start()
    {
        m_generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        for (int i = 0; i < amount; i++) 
        {
            orbs.Add(Instantiate(orbPrefab, generateOrbLocation(0.5f), Quaternion.identity));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private bool checkValid(Vector2Int pos)
    {
        bool valid = true;
        if (pos.x > 0 || pos.y > 0 || pos.x < m_generator.mazeWidth - 1 || pos.y < m_generator.mazeHeight - 1)
        {
            for (int i = 0; i < orbs.Count; i++)
            {
                if (isInRange(pos, new Vector2Int((int)orbs[i].transform.position.x, (int)orbs[i].transform.position.y)))
                { 
                    valid = false;
                }
            }
        }
        return valid;
    }

    private bool isInRange(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2) < range * range;
    }
    private Vector2Int generatePosition()
    {
        Vector2Int pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
        while (!checkValid(pos))
        {
            pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
        }
        return pos;
    }

    public Vector3 generateOrbLocation(float yPos)
    {
        Vector2Int gridLoc = generatePosition();
        return new Vector3(gridLoc.x * m_renderer.CellSize, yPos, gridLoc.y * m_renderer.CellSize);
    }
}
