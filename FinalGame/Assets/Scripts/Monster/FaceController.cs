using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject monster;
    [SerializeField] GameObject player;
    private Vector3 monsterPos;
    private Vector3 monsterRot;
    [SerializeField] float xOffset = 0;
    [SerializeField] float yOffset = 0.25f;
    [SerializeField] float zOffset = 0;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        monsterPos = monster.transform.position;
        monsterRot = monster.transform.rotation.eulerAngles;
        transform.position = new Vector3(monsterPos.x + xOffset, monsterPos.y + yOffset, monsterPos.z + zOffset);
        transform.rotation = Quaternion.Euler(monsterRot.x, 0, monsterRot.z);
    }
}
