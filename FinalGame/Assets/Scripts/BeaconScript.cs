using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameManager gameManager;
    [SerializeField] MazeRenderer m_renderer;
    [SerializeField] GameObject player;
    Orb orbScript;
    Charge chargeScript;
    void Start()
    {
        gameManager = GameObject.Find("Maze").GetComponent<GameManager>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        orbScript = GameObject.Find("Maze").GetComponent<Orb>();
        chargeScript = GameObject.Find("Maze").GetComponent<Charge>();
        player = GameObject.Find("FirstPersonController");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_renderer.DestroyMaze();
            m_renderer.DrawMaze();
            orbScript.resetOrbs();
            chargeScript.resetCharges();
            gameManager.spawned = false;
            gameManager.currentCharges = 0;
            player.GetComponent<FirstPersonController>().playerState = FirstPersonController.PlayerState.NextLevel;
            Destroy(gameObject);
        }
    }
}
