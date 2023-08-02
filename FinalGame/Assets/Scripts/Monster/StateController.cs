using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using UnityEngine;

public class StateController : MonoBehaviour
{
    //For non-Stack version: https://gamedevbeginner.com/state-machines-in-unity-how-and-when-to-use-them/

    Stack<State> currentState = new Stack<State>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(currentState.Count);
        if (currentState.Count > 0)
        {
            currentState.Peek().OnUpdate();
        }

    }

    public void ChangeState(State newState)
    {
        RemoveTop();
        AddNewState(newState);
    }

    public void Interrupt(State newState)
    {
        AddNewState(newState);
    }

    public void ResumePrevious()
    {
        RemoveTop();
    }

    public void RemoveTop()
    {
        if (currentState.Count > 0 && !currentState.Peek().doNotRemove)
        {
            currentState.Peek().OnExit();
            currentState.Pop();
        }
    }

    public void AddNewState(State newState)
    {
        currentState.Push(newState);
        currentState.Peek().OnStateEnter(this);
    }

    public void ClearStates()
    {
        //Clear all states, except the doNotRemove at the bottom
        while (currentState.Count > 0 && !currentState.Peek().doNotRemove)
        {
            //Call exit, or just close it all down?
            currentState.Pop();
        }
    }

    public GameObject FindClosestTarget(string tag, float maxDistance)
    {
        GameObject closest = null;                          //result: starts at null just in case we don't find anything in range

        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(tag);  //Fill array with all gameobjects with the tag

        float distance = maxDistance * maxDistance;         //square the distance
        Vector3 position = transform.position;              //our current position

        foreach (GameObject obj in gameObjects)
        {
            Vector3 difference = obj.transform.position - position; //calculate the difference to the object from our current position
            float curDistance = difference.sqrMagnitude;    //distance requires a square root, which is slow, just using the squared magnitued

            if (curDistance < distance)                     //comparing the squared distances
            {
                closest = obj;                              //new closest object set
                distance = curDistance;                     //distance to the object saved for next comparison in loop
            }
        }

        //Return the obejct we found, or null if we didn't find anything
        return closest;
    }

    public void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState.Count > 0)
        {
            currentState.Peek().OnTriggerEnter(other);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (currentState.Count > 0)
        {
            currentState.Peek().OnCollisionEnter(collision);
        }
    }
}
