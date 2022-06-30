using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractablePhysicsData : MonoBehaviour
{
	[field: SerializeField, Tooltip("The hand pose name (defined in the HandInfo scriptable object)")]
	public string HandGrabPose { get; private set; } = "Grab";
	[field: SerializeField, Tooltip("Are physical interactions with the hand enabled (recommended false)")]
	public bool DoPhysicsInteractions  { get; private set; }= false;
	[field: SerializeField, Tooltip("The position the left hand will attach to")]
	public Transform PhysicsLeftHandAttachPoint { get; private set; }
	[field: SerializeField, Tooltip("The position the right hand will attach to")]
	public Transform PhysicsRightHandAttachPoint { get; private set; }

	[field: SerializeField, Tooltip("Colliders that will not collide with hand while interacting")]
	public Collider[] AllColliders { get; private set; }
	[field: SerializeField, Tooltip("Colliders that this interactable ignores (useful for joints sometimes)")]
	public Collider[] PermanantIgnoreColliders { get; private set; }
	[field: SerializeField, Tooltip("Does this interactor have full range of motion or is it limited (by joints/restrictions) ")]
	public bool RestrictedMovement { get; private set; }
	[field: SerializeField, Tooltip("The modifier on the distance when the physics hand starts moving towards the target position")]
	public float MoveToDistanceModifier { get; private set; } = 1.0f;
	[field: SerializeField, Tooltip("The modifier on the force the physics hand uses when travelling towards the target position. Setting this too low can mean the hand never properly connects to the interactable")]
	public float MoveToForceModifier { get; private set; } = 0.5f;
	[field: SerializeField, Tooltip("Is the position of the attach point set based on the physics attach points or not")]
	public bool UniformAttachTransform { get; private set; } = false;

	[field: SerializeField, HideInInspector]
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

		foreach (var igCol in PermanantIgnoreColliders)
		{
			foreach (var col in AllColliders)
			{
				Physics.IgnoreCollision(igCol, col);
			}
		}
	}
}
