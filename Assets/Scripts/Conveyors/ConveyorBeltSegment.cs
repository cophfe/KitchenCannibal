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
   [SerializeField] LayerMask disablePlayerInteractionLayer;

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
        //other.attachedRigidbody.AddForce(direction * speed * Time.deltaTime);
        other.attachedRigidbody.position = other.attachedRigidbody.position + direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
       Order temp = other.GetComponent<Order>();
        if(temp != null)
        {
            temp.gameObject.layer = disablePlayerInteractionLayer;
            temp.OrderComplete();
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
