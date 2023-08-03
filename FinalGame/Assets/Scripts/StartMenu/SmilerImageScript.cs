using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;

public class SmilerImageScript : MonoBehaviour
{
    // Start is called before the first frame update


    private bool flashed;
    private float currentAlpha = 0f;
    private float footstepTimer = 0;

    void Start()
    {
        gameObject.SetActive(true);
        GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<RawImage>().color = new Color(1f, 1f, 1f, currentAlpha);
        if (!flashed)
        {
            currentAlpha += (0.2f / 255);
            if (currentAlpha >= 1)
            {

                flashed = true;
            }
        }
        if (flashed)
        {
            currentAlpha -= (0.2f / 255);
            if (currentAlpha <= 0)
            {

                flashed = false;
            }
        }
    }
}
