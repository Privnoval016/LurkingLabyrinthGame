using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    System.Random gen;
    public int orbsCollected = 0;
    public int currentCharges = 1;
    public int currentOrbs = 0;
    public bool spawned = false;
    public GameObject orbTextArea;
    public GameObject chargeTextArea;
    TextMeshProUGUI orbText;
    TextMeshProUGUI chargeText;
    [SerializeField] GameObject beacon;
    [SerializeField] MazeGenerator m_generator;
    [SerializeField] MazeRenderer m_renderer;
    [SerializeField] Orb orbScript;
    
    [SerializeField] FirstPersonController playerScript;
    void Start()
    {
        gen = new System.Random();
        orbText = orbTextArea.GetComponent<TextMeshProUGUI>();
        chargeText = chargeTextArea.GetComponent<TextMeshProUGUI>();
        m_generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        playerScript = GameObject.Find("FirstPersonController").GetComponent<FirstPersonController>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        orbScript = GameObject.Find("Maze").GetComponent<Orb>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned)
        {
            if (currentOrbs % 10 == 0 && currentOrbs > 0)
            {
                currentOrbs = 0;
                orbScript.clearOrbs();
                Vector2Int pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
                while (Vector2Int.Distance(pos, new Vector2Int(playerScript.xCoor, playerScript.yCoor)) < 10)
                {
                    pos = m_generator.maze[gen.Next(0, m_generator.mazeWidth - 1), gen.Next(0, m_generator.mazeHeight - 1)].position;
                }
                Instantiate(beacon, new Vector3(pos.x*m_renderer.CellSize, 0f, pos.y*m_renderer.CellSize), Quaternion.identity);
                spawned = true;
            }
        }
        orbText.text = "" + orbsCollected;
        chargeText.text = "" + currentCharges;
    }
}
