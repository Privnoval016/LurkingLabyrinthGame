using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Duplicate this file for each state you create, then fill in the methods
public class HidingState : State
{
    //Worker worker;

    //When the state starts for the first time
    public override void OnEnter()
    {
        //doNotRemove = true;   //If you don't want the state to be removed
        //Only have one of them, added before any others
        //worker = sc.gameObject.GetComponent<Worker>();    //Grab the game object and save it for later
    }

    //Called during Update()
    public override void OnUpdate()
    {

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
