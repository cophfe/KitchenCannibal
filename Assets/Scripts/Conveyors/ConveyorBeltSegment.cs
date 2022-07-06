using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConveyorDirection
{
    forward,
    left,
    right,
    back
}

public class ConveyorBeltSegment : MonoBehaviour
{
    public float speed = 0.5f;
    public ConveyorDirection conveyorDirection;
    private Vector3 direction;
	[SerializeField] string disableLayer;
	List<Order> ordersComplete = new List<Order>();

	List<Rigidbody> moving = new List<Rigidbody>();

    private void OnValidate()
    {
        switch (conveyorDirection)
        {
            case ConveyorDirection.forward:
                direction = Vector3.forward;
                break;

            case ConveyorDirection.left:
                direction = -Vector3.right;
                break;

            case ConveyorDirection.right:
                direction = Vector3.right;
                break;

            case ConveyorDirection.back:
                direction = Vector3.back;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
		if (other.attachedRigidbody)
		{
			
		}
    }

    private void OnTriggerEnter(Collider other)
    {
		if (!other.attachedRigidbody)
			return;

		if (!moving.Contains(other.attachedRigidbody))
		{
			moving.Add(other.attachedRigidbody);
		}

		Order temp = other.attachedRigidbody.GetComponent<Order>();

        if(temp != null && !ordersComplete.Contains(temp))
        {
			ordersComplete.Add(temp);

			temp.gameObject.layer = LayerMask.NameToLayer("HandIgnore");

			temp.OrderComplete();
        }
	}

	private void OnTriggerExit(Collider other)
	{
		moving.Remove(other.attachedRigidbody);
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < moving.Count; i++)
		{
			if (moving[i])
			{
				moving[i].position = moving[i].position - Vector3.right * 0.2f * Time.fixedDeltaTime;
			}
			else
			{
				moving.RemoveAt(i);
				i--;
			}
			
		}
	}


	private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + (direction * 0.5f));
        Vector3 rightDir = Vector3.Cross(transform.up, direction);

        Vector3 leftPos = Vector3.Lerp(transform.position + (direction * 0.5f), transform.position + (-rightDir.normalized * 0.5f), 0.5f);
        Vector3 rightPos = Vector3.Lerp(transform.position + (direction * 0.5f), transform.position + (rightDir.normalized * 0.5f), 0.5f);
        Gizmos.DrawLine(transform.position + (direction * 0.5f), leftPos);
        Gizmos.DrawLine(transform.position + (direction * 0.5f), rightPos);
        Gizmos.DrawLine(leftPos, rightPos);
    }
}
