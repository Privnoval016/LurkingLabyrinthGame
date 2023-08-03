using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BeaconScript : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameManager gameManager;
    [SerializeField] MazeRenderer m_renderer;
    [SerializeField] GameObject player;
    [SerializeField] GameObject monster;
    Orb orbScript;
    Charge chargeScript;
    void Start()
    {
        gameManager = GameObject.Find("Maze").GetComponent<GameManager>();
        m_renderer = GameObject.Find("Maze").GetComponent<MazeRenderer>();
        orbScript = GameObject.Find("Maze").GetComponent<Orb>();
        chargeScript = GameObject.Find("Maze").GetComponent<Charge>();
        player = GameObject.Find("FirstPersonController");
        monster = GameObject.Find("Monster");
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
            gameManager.respawned = true;
            gameManager.respawnText.GetComponent<TMP_Text>().color = new Color(1f, 0, 0, 1);
            gameManager.currentAlpha = 1;
            gameManager.currentCharges = 1;
            player.GetComponent<FirstPersonController>().playerState = FirstPersonController.PlayerState.NextLevel;
            monster.transform.position = new Vector3(48, monster.transform.position.y, 48);
            monster.GetComponent<Monster>().levelMultiplier += 0.1f;
            Destroy(gameObject);
        }
    }
}
