using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashScript : MonoBehaviour
{
    // Start is called before the first frame update
    float existTime = 1f;
    float totalTime = 0f;

    Monster monsterScript;
    void Start()
    {
        monsterScript = GameObject.Find("Monster").GetComponentInChildren<Monster>();
    }

    // Update is called once per frame
    void Update()
    {
        totalTime += Time.deltaTime;
        if (totalTime > existTime) 
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Monster"))
        {
            monsterScript.Stun();
            Destroy(gameObject);
        }
    }
}
