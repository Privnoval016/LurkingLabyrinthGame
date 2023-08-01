using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public StateController sc;
    public bool doNotRemove = false;

    public void OnStateEnter(StateController stateController)
    {
        sc = stateController;
        OnEnter();
    }

    public virtual void OnEnter()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void OnExit()
    {

    }

    public virtual void OnTriggerEnter(Collider other)
    {

    }

    public virtual void OnCollisionEnter(Collision collision)
    {

    }
}
