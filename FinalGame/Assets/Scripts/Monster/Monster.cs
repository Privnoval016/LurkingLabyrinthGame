using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private MazeRenderer m_Renderer;
    [SerializeField] private int xCoor;
    [SerializeField] private int yCoor;
    private NavMeshAgent agent;

    private StateController controller;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<StateController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            xCoor = other.GetComponent<MazeCellObject>().xCoor;
            yCoor = other.GetComponent<MazeCellObject>().yCoor;
        }
    }
}
