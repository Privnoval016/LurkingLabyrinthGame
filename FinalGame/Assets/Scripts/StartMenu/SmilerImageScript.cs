using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

public class SmilerImageScript : MonoBehaviour
{
    // Start is called before the first frame update
    float timeElapsed;
    bool visible = false;
    void Start()
    {
        gameObject.SetActive(visible);
        timeElapsed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > 5f)
        {
            gameObject.SetActive(!visible);
            visible = !visible;
            timeElapsed = 0f;
        }
    }
}
