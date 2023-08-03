using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

//Duplicate this file for each state you create, then fill in the methods
public class ChasingState : State
{
    //Worker worker;

    Monster monster;
    NavMeshAgent agent;
    MazeRenderer m_Renderer;
    MazeGenerator m_Generator;

    private float timer;
    [SerializeField] private float chaseTime;

    //When the state starts for the first time
    public override void OnEnter()
    {
        Debug.Log("chasing");
        monster = sc.gameObject.GetComponent<Monster>();
        agent = monster.GetComponent<NavMeshAgent>();
        monster.chaseAudioSource.PlayOneShot(monster.chaseScreech);
        m_Renderer = monster.m_Renderer;
        m_Generator = monster.m_Generator;

        agent.speed = monster.chaseSpeed;
        agent.acceleration = monster.chaseSpeed * 1.5f;
        agent.SetDestination(monster.player.transform.position);

        chaseTime = Random.Range(monster.minChaseTime, monster.maxChaseTime);
        
    }

    //Called during Update()
    public override void OnUpdate()
    {
        agent.SetDestination(monster.player.transform.position);
        timer += Time.deltaTime;

        if (timer > chaseTime)
        {
            sc.ChangeState(new HidingState());
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
}