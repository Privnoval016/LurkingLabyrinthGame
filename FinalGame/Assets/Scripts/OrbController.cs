using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour
{
    [SerializeField] private float bobSpeed = 0.1f;
    [SerializeField] private float bobHeight = 0.1f;
    private Vector3 initialPosition;
    private Orb orbScript;

    // Start is called before the first frame update
    void Start()
    {
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
            transform.position = orbScript.generateOrbLocation(0.5f);
            initialPosition = transform.position;
            other.GetComponent<FirstPersonController>().IncreaseOrbsCollected(1);
        }
    }
}
