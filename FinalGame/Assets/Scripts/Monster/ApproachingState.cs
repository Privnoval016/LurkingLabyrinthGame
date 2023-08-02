using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;

//Duplicate this file for each state you create, then fill in the methods
public class ApproachingState : State
{
    Monster monster;
    NavMeshAgent agent;
    MazeRenderer m_Renderer;
    MazeGenerator m_Generator;


    bool checkedForChase;
    float checkTimer;

    //When the state starts for the first time
    public override void OnEnter()
    {
        //doNotRemove = true;
        monster = sc.gameObject.GetComponent<Monster>();
        agent = monster.GetComponent<NavMeshAgent>();
        m_Renderer = monster.m_Renderer;
        m_Generator = monster.m_Generator;

    }

    //Called during Update()
    public override void OnUpdate()
    {
        if (monster.distanceFromPlayer > monster.stalkRadius)
        {
            Debug.Log("Moving to stalk radius");
            agent.SetDestination(FindClosestPositionInRange(new Vector2Int(monster.player.xCoor, monster.player.yCoor), (int)(monster.stalkRadius)));
        }
        else if (monster.distanceFromPlayer > monster.chaseRadius)
        {
            Debug.Log("Moving around the stalk radius");
            agent.SetDestination(GetRandomAdjacentPosition(new Vector2Int(monster.xCoor, monster.yCoor)));
        }
        else
        {
            if (!checkedForChase)
            {
                Debug.Log("In Range of Player");
                checkedForChase = true;
                int chaseProbability = CalculateChaseProbability();

                int rng = Random.Range(0, 100);

                if (rng < chaseProbability)
                {
                    Debug.Log("Found Player");
                    //sc.ChangeState(new ChasingState());
                }
                else
                {
                    Debug.Log("Player Not Detected");
                    agent.SetDestination(GetRandomAdjacentPosition(new Vector2Int(monster.xCoor, monster.yCoor)));
                }
            }

        }

        if (checkTimer > monster.checkTime)
        {
            checkTimer = 0;
            checkedForChase = false;
        }

        checkTimer += Time.deltaTime;
    }

    //When state is turned off
    public override void OnExit()
    {

    }

    //When the object hits a trigger (or is a trigger)
    public override void OnTriggerEnter(Collider other)
    {

    }

    //When the object touches a RigidBody
    public override void OnCollisionEnter(Collision collision)
    {

    }

    private int CalculateChaseProbability()
    {
        float chaseProbability = 1 - monster.distanceFromPlayer / monster.chaseRadius;

        chaseProbability =
            monster.player.isCrouching ? chaseProbability * monster.crouchMultiplier :
            monster.player.IsSprinting ? chaseProbability + ((1 - chaseProbability) / monster.sprintMultiplier) : chaseProbability;

        return (int)(chaseProbability * 100);

    }

    private Vector3 GetRandomAdjacentPosition(Vector2Int pos)
    {
        List<Vector2Int> validLocations = new List<Vector2Int>();

        for (int i = -1; i < 1; i++)
        {
            for (int j = -1; j < 1; j++)
            {
                if (i != 0 || j != 0)
                {
                    Vector2Int loc = new Vector2Int(pos.x + i, pos.y + j);
                    if (loc.x >= 0 && loc.y >= 0 && loc.x < m_Generator.mazeWidth && loc.y < m_Generator.mazeHeight)
                    {
                        validLocations.Add(loc);
                    }
                }
            }
        }

        return m_Renderer.getCellPosition(validLocations[Random.Range(0, validLocations.Count)]);
    }

    private Vector3 FindClosestPositionInRange(Vector2Int center, int radius)
    {
        List<Vector2Int> validLocations = new List<Vector2Int>();

        for (int i = -radius; i < radius; i++)
        {
            for (int j = -radius; j < radius; j++)
            {
                Vector2Int loc = new Vector2Int(center.x + i, center.y + j);
                if (loc.x >= 0 && loc.y >= 0 && loc.x < m_Generator.mazeWidth && loc.y < m_Generator.mazeHeight)
                {
                    validLocations.Add(loc);
                }
            }

        }

        Vector2Int closestLoc = Vector2Int.zero;
        float closestDistance = float.MaxValue;

        foreach (Vector2Int loc in validLocations)
        {
            float distance = Vector2Int.Distance(loc, new Vector2Int(monster.xCoor, monster.yCoor));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLoc = loc;
            }
        }

        return m_Renderer.getCellPosition(closestLoc);
    }
}
