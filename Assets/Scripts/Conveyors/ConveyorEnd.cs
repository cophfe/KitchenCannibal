using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{
    [SerializeField] private ConveyorStart start;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Order>() != null)
        {
            // Destroy the object and give the reward
        }
        else
        {
            ConveyerObject obj = other.gameObject.AddComponent<ConveyerObject>();
            obj.start = start;
            obj.Hide();
        }




    }
}
