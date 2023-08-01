using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    [SerializeField] private float bobSpeed = 0.1f;
    [SerializeField] private float bobHeight = 0.1f;
    private Vector3 initialPosition;
    private MazeGenerator m_Generator;
    private MazeRenderer m_Renderer;
    public Vector2Int position;
    private Orb orbScript;

    // Start is called before the first frame update
    void Start()
    {
        m_Generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        m_Renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        orbScript = GameObject.Find("Maze").GetComponent<Orb>();
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(initialPosition.x,
            initialPosition.y + Mathf.Sin(Time.fixedTime * Mathf.PI * bobSpeed) * (bobHeight), initialPosition.z);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2Int validPos = orbScript.generatePosition();
            transform.position = new Vector3(m_Generator.maze[validPos.x, validPos.y].position.x * m_Renderer.CellSize, 0.5f, m_Generator.maze[validPos.x, validPos.y].position.y * m_Renderer.CellSize);
            initialPosition = transform.position;
            other.GetComponent<FirstPersonController>().IncreaseOrbsCollected(1);
        }
    }
}