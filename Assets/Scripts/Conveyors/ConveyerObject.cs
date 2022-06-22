using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerObject : MonoBehaviour
{
    public int timeHidden = 10;
    public float timeLeft = 0;
    private bool startCountdown = false;
    public ConveyorStart start = null;

    private void Update()
    {
        if(startCountdown)
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft <= 0)
                Show();
        }
    }

    public void Hide()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        timeLeft = (float)timeHidden;
        startCountdown = true;
    }

    public void Show()
    {
        transform.position = start.transform.position;
        gameObject.GetComponent<MeshRenderer>().enabled = true;
       startCountdown = false;
       Destroy(gameObject.GetComponent<ConveyerObject>());
    }
}
