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
    List<OrbController> orbScripts = new List<OrbController>();
    [SerializeField] GameObject orbPrefab;
    void Start()
    {
        m_generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        clearOrbs();
        resetOrbs();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool checkValid(Vector2Int pos)
    {
        bool valid = true;
        for (int i = 0; i < orbs.Count; i++)
        {
            if (Vector2Int.Distance(pos, new Vector2Int((int)orbScripts[i].position.x, (int)orbScripts[i].position.y)) < range)
            {
                valid = false;
            }
        }
        return valid;
    }

    private bool isInRange(Vector2Int pos1, Vector2Int pos2)
    {
        return Mathf.Sqrt(Mathf.Pow((pos1.x - pos2.x), 2) + Mathf.Pow((pos1.y - pos2.y), 2)) < range;
    }
    public Vector2Int generatePosition()
    {
        Vector2Int pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
        while (!checkValid(pos))
        {
            pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
        }
        return pos;
    }
    public void resetOrbs()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2Int pos = generatePosition();
            GameObject temp = Instantiate(orbPrefab, new Vector3((float)pos.x * m_renderer.CellSize, 0.5f, (float)pos.y * m_renderer.CellSize), Quaternion.identity);
            orbs.Add(temp);
            orbScripts.Add(temp.GetComponent<OrbController>());
            orbScripts[i].position = pos;
        }
    }
    public void clearOrbs()
    {
        for (int i = 0; i < orbs.Count; i++)
        {
            Destroy(orbs[i]);
        }
        orbs.Clear();
        orbScripts.Clear();
    }
}