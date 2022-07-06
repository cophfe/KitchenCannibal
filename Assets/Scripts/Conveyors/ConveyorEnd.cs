using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorEnd : MonoBehaviour
{

	[SerializeField] private ConveyorStart start;
	
    private void OnTriggerEnter(Collider other)
    {
		var rb = other.attachedRigidbody;
		if (!rb)
			return;

		var order = rb.GetComponent<Order>();
        if (order)
        {
			Destroy(rb.gameObject);
        }
        else
        {
            ConveyerObject obj = rb.gameObject.AddComponent<ConveyerObject>();
            obj.start = start;
            obj.Hide();
			rb.velocity = Vector3.zero;
        }
    }
}
