using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.AI;

//Duplicate this file for each state you create, then fill in the methods
public class StalkingState : State
{
    Monster monster;
    NavMeshAgent agent;
    MazeRenderer m_Renderer;
    MazeGenerator m_Generator;

    
    bool checkedForChase;
    float checkTimer;


    Vector3 target;

    //When the state starts for the first time
    public override void OnEnter()
    {
        //doNotRemove = true;
        monster = sc.gameObject.GetComponent<Monster>();
        agent = monster.GetComponent<NavMeshAgent>();
        m_Renderer = monster.m_Renderer;
        m_Generator = monster.m_Generator;

        target = monster.transform.position;
        

         
    }

    //Called during Update()
    public override void OnUpdate()
    {

       //if (Vector3.Distance(target, monster.transform.position) < 1f)
       
        agent.SetDestination(target);

        if (monster.posDistFromPlayer >= monster.stalkRadius)
        {
            /*Debug.Log("Moving to stalk radius");*/

            agent.speed = monster.fastSpeed;
            agent.acceleration = monster.fastSpeed * 1.5f;
            target = FindClosestPositionInRange(monster.player.transform.position, (int)(monster.stalkRadius));
            

        }
        else if (monster.posDistFromPlayer >= monster.chaseRadius && Vector3.Distance(monster.transform.position, target) < 1f)
        {
            /*Debug.Log("Moving around the stalk radius");*/
            agent.speed = monster.slowSpeed;
            agent.acceleration = monster.slowSpeed * 1.5f;
            target = GetRandomPosition(monster.player.transform.position, monster.stalkRadius);
            //Debug.Log("{" + target.x + ", " + target.y + ", " + target.z + "}");
        }
        
        else if (monster.posDistFromPlayer < monster.chaseRadius)
        {
            if (!checkedForChase)
            {
                //Debug.Log("In Range of Player");
                checkedForChase = true;
                int chaseProbability = CalculateChaseProbability();

                int rng = Random.Range(0, 100);

                if (rng < chaseProbability)
                {
                    //Debug.Log("Found Player");
                    sc.ChangeState(new ChasingState());
                }
                else
                {

                    //Debug.Log("Player not Detected");
                    agent.speed = monster.slowSpeed;
                    agent.acceleration = monster.slowSpeed * 1.5f;
                    target = GetRandomPosition(monster.player.transform.position, monster.chaseRadius);

                }
            }
            else
            {
                agent.speed = monster.slowSpeed;
                agent.acceleration = monster.slowSpeed * 1.5f;
                target = GetRandomPosition(monster.player.transform.position, monster.chaseRadius);
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
        float chaseProbability = 1 - monster.posDistFromPlayer/monster.chaseRadius;

        chaseProbability = 
            monster.player.isCrouching ? chaseProbability * monster.crouchMultiplier : 
            monster.player.IsSprinting ? chaseProbability + ((1 - chaseProbability) / monster.sprintMultiplier) : chaseProbability;

        return (int)(chaseProbability * 100);

    }

    private Vector3 GetRandomPosition(Vector3 pos, int radius)
    {
        List<Vector3> validLocations = monster.FindCellsInRange(pos, radius);

        return validLocations[Random.Range(0, validLocations.Count)];
    }

    private Vector3 FindClosestPositionInRange(Vector3 center, int radius)
    {
        /*
        List<Vector2Int> validLocations = monster.FindLocationsInSquare(center, radius);

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
        */

        List<Vector3> validPositions = monster.FindCellsInRange(center, radius);
        
        Vector3 closestLoc = Vector3.zero;
        float closestDistance = 10000000;

        for (int i = 0; i < validPositions.Count; i++)
        {
            float distance = Vector3.Distance(validPositions[i], monster.transform.position);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestLoc = validPositions[i];
            }
        }

        
        return closestLoc;
    }
}
