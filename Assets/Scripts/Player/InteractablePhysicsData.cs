using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePhysicsData : MonoBehaviour
{
	[field: SerializeField]
	public string HandGrabPose { get; private set; } = "Grab";
	[field: SerializeField]
	public bool DoPhysicsInteractions  { get; private set; }= false;
	[field: SerializeField]
	public Transform PhysicsLeftHandAttachPoint { get; private set; }
	[field: SerializeField]
	public Transform PhysicsRightHandAttachPoint { get; private set; }

	[field: SerializeField]
	public Collider[] AllColliders { get; private set; }
	[field: SerializeField]
	public bool RestrictedMovement { get; private set; }
	[field: SerializeField]
	public float MoveToDistanceModifier { get; private set; } = 1.0f;

	public HandManager CurrentlyInteractingHand { get; set; } = null;
	public HandManager QueuedHand { get; set; } = null;

	public Rigidbody InteractableBody { get; private set; }
	private void Awake()
	{
		InteractableBody = GetComponent<Rigidbody>();
	}
}