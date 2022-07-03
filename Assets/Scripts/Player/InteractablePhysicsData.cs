using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
	[field: SerializeField]
	public float MoveToForceModifier { get; private set; } = 0.5f;
	[field: SerializeField]
	public bool UniformAttachTransform { get; private set; } = false;

	[field: SerializeField]
	public Vector3 AttachPosition { get; private set; }
	//[field: SerializeField]
	//public Quaternion AttachRotation { get; private set; }

	public Rigidbody InteractableBody { get; private set; }
	private void Awake()
	{
		InteractableBody = GetComponent<Rigidbody>();
		
		var inter = GetComponent<XRBaseInteractable>();
		if (inter)
		{
			var t = inter.GetAttachTransform(null);
			AttachPosition = t.localPosition;
			//AttachRotation = t.localRotation;
		}
	}
}
