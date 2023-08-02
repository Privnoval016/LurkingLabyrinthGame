using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

//Duplicate this file for each state you create, then fill in the methods
public class HidingState : State
{
    Monster monster;
    NavMeshAgent agent;
    MazeRenderer m_Renderer;
    MazeGenerator m_Generator;

    Vector3 target;

    //When the state starts for the first time
    public override void OnEnter()
    {
        Debug.Log("hiding");
        monster = sc.gameObject.GetComponent<Monster>();
        agent = monster.GetComponent<NavMeshAgent>();
        m_Renderer = monster.m_Renderer;
        m_Generator = monster.m_Generator;
        target = FindFarthestPositionInRange(monster.player.transform.position, monster.stalkRadius);
        agent.speed = monster.fastSpeed;
        agent.acceleration = monster.fastSpeed * 1.5f;
        agent.SetDestination(target);
    }

    //Called during Update()
    public override void OnUpdate()
    {
        Debug.Log("Hiding");
        if (Vector3.Distance(monster.transform.position, target) < 1f)
        {
            sc.ChangeState(new StalkingState());
        }
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

    private Vector3 FindFarthestPositionInRange(Vector3 center, int radius)
    {
        

        List<Vector3> validPositions = monster.FindCellsInRange(center, radius);

        Vector3 farthestLoc = Vector3.zero;
        float farthestDistance = -1;

        for (int i = 0; i < validPositions.Count; i++)
        {
            float distance = Vector3.Distance(validPositions[i], monster.transform.position);

            if (distance < farthestDistance)
            {
                farthestDistance = distance;
                farthestLoc = validPositions[i];
            }
        }


        return farthestLoc;
    }
}

