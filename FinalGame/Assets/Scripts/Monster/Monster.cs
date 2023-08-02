using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class Monster : MonoBehaviour
{
    public MazeRenderer m_Renderer;
    public MazeGenerator m_Generator;

    public float distanceFromPlayer => Vector2Int.Distance(new Vector2Int(xCoor, yCoor), new Vector2Int(player.xCoor, player.yCoor));
    public float posDistFromPlayer => Vector3.Distance(transform.position, player.transform.position);


    public FirstPersonController player;


    [Header("Monster Attributes")]
    public int xCoor;
    public int yCoor;
    public float chaseSpeed = 10f;
    public float fastSpeed = 16f;
    public float slowSpeed = 8f;
    public bool stunned;
    public GameObject fullMonster;

    [Header("Distances")]
    public int stalkRadius;
    public int chaseRadius;

    [Header("Chase Weights")]
    public float sprintMultiplier;
    public float crouchMultiplier;

    public float checkTime;

    [Range(5, 45)]
    public int minChaseTime = 15, maxChaseTime = 25;




    public StateController controller;

    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        m_Generator = GameObject.Find("Maze").GetComponent<MazeGenerator>();
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        gameObject.GetComponent<NavMeshAgent>().enabled = true;
        player = GameObject.Find("FirstPersonController").GetComponent<FirstPersonController>();

        stalkRadius *= (int)m_Renderer.CellSize;
        chaseRadius *= (int)m_Renderer.CellSize;

        controller = GetComponent<StateController>();

        controller.AddNewState(new StalkingState());

    }

    // Update is called once per frame
    void Update()
    {


        //Debug.Log("Stalk Radius: " + inStalkRadius);
        //Debug.Log("Chase Radius: " + inChaseRadius);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cell"))
        {
            xCoor = other.GetComponent<MazeCellObject>().xCoor;
            yCoor = other.GetComponent<MazeCellObject>().yCoor;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Add Jumpscare
            player.playerState = FirstPersonController.PlayerState.Death;
            fullMonster.SetActive(false);
        }
    }


    public List<Vector3> FindCellsInRange(Vector3 center, int radius)
    {
        List<Vector3> validPositions = new List<Vector3>();
        for (int i = 0; i < m_Generator.mazeWidth; i++)
        {
            for (int j = 0; j < m_Generator.mazeHeight; j++)
            {
                if (i >= 0 && j >= 0 && i < m_Generator.mazeWidth && j < m_Generator.mazeHeight)
                {
                    Vector3 cellPos = m_Renderer.getCellPosition(new Vector2Int(i, j));
                    if (Vector3.Distance(cellPos, center) < radius)
                        validPositions.Add(cellPos);
                }
            }
        }
        return validPositions;
    }
    public void Stun()
    {
        gameObject.GetComponent<NavMeshAgent>().speed = 0f;
        stunned = true;
    }
    public void UnStun()
    {
        stunned = false;
        gameObject.GetComponent<NavMeshAgent>().speed = 3.5f;
    }
}
