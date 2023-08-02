using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charge : MonoBehaviour
{
    // Start is called before the first frame update
    [Range(0, 50)]
    [SerializeField] int amount = 3;
    [Range(0, 50)]
    [SerializeField] int range = 10;
    System.Random gen = new System.Random();
    MazeGenerator m_generator;
    MazeRenderer m_renderer;
    List<GameObject> charges = new List<GameObject>();
    List<ChargeController> chargeScripts = new List<ChargeController>();
    [SerializeField] GameObject chargePrefab;
    void Start()
    {
        m_generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        clearCharges();
        resetCharges();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private bool checkValid(Vector2Int pos)
    {
        bool valid = true;
        for (int i = 0; i < charges.Count; i++)
        {
            if (Vector2Int.Distance(pos, new Vector2Int((int)chargeScripts[i].position.x, (int)chargeScripts[i].position.y)) < range)
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
    public void resetCharges()
    {
        for (int i = 0; i < amount; i++)
        {
            Vector2Int pos = generatePosition();
            GameObject temp = Instantiate(chargePrefab, new Vector3((float)pos.x * m_renderer.CellSize, 0.5f, (float)pos.y * m_renderer.CellSize), Quaternion.identity);
            charges.Add(temp);
            chargeScripts.Add(temp.GetComponent<ChargeController>());
            chargeScripts[i].position = pos;
        }
    }
    public void clearCharges()
    {
        for (int i = 0; i < charges.Count; i++)
        {
            Destroy(charges[i]);
        }
        charges.Clear();
        chargeScripts.Clear();
    }
}
