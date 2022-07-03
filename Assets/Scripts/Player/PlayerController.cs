using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerController : MonoBehaviour
{
	[field: SerializeField]
	public XROrigin Origin { get; private set; }

	[field: SerializeField]
	public HandManager LeftHand { get; private set; }

	[field: SerializeField]
	public HandManager RightHand{ get; private set; }

	[field: SerializeField]
	public PhysicsLocomotion Locomotor { get; private set; }

	[field: SerializeField]
	public float PlayerRadius { get; private set; } = 0.5f;
	[field: SerializeField]
	public float PlayerHeadRadius { get; private set; } = 0.2f;
	[field: SerializeField]
	public LayerMask TeleportCollisionMask { get; private set; }
	[field: SerializeField]
	public LayerMask TeleportSurfaceMask { get; private set; }

	private void Awake()
	{
		RightHand.PlayerController = this;	
		LeftHand.PlayerController = this;

		
	}

	public bool CheckValidTeleportPoint(Vector3 requestPosition)
	{
		Vector3 pos = requestPosition;
		float height = Origin.CameraInOriginSpacePos.y + PlayerHeadRadius;
		Vector3 a = pos + Vector3.up * PlayerRadius;
		a.y += 0.01f;
		Vector3 b = pos + Vector3.up * (height - PlayerRadius);

		return !Physics.CheckCapsule(a, b, PlayerRadius, TeleportCollisionMask, QueryTriggerInteraction.Ignore);
	}

	public HandManager GetOppositeHand(HandManager hand)
	{
		return hand == LeftHand ? RightHand : LeftHand;
	}
	private void OnEnable()
	{
		
	}

	private void OnDisable()
	{
		
	}
}
