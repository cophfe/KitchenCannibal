using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketInteractor : XRSocketInteractor
{
	[SerializeField]
	Collider[] colliders;
	[SerializeField]
	float whileSelectingDetectDistanceModifier = 0.5f;

	CustomGrabInteractable attachedInt;
	float grabMultiplier;

	protected override void OnEnable()
	{
		attachedInt = GetComponent<CustomGrabInteractable>();
		if (attachedInt)
			grabMultiplier = attachedInt.HandDetectDistanceModifer;

		if (colliders == null || colliders.Length == 0)
		{
			colliders = GetComponentsInChildren<Collider>();
		}

		base.OnEnable();
	}
	protected override void OnSelectEntered(SelectEnterEventArgs args)
	{
		if (attachedInt)
			attachedInt.HandDetectDistanceModifer = grabMultiplier * whileSelectingDetectDistanceModifier;

		var physData = args.interactableObject.transform.GetComponent<InteractablePhysicsData>();
		if (physData)
		{
			foreach (var col in physData.AllColliders)
			{
				foreach (var socketCol in colliders)
				{
					if (!socketCol.isTrigger)
						Physics.IgnoreCollision(col, socketCol);
				}
			}
		}
			
		base.OnSelectEntered(args);
	}

	protected override void OnSelectExited(SelectExitEventArgs args)
	{
		if (attachedInt)
			attachedInt.HandDetectDistanceModifer = grabMultiplier;

		var physData = args.interactorObject.transform.GetComponent<InteractablePhysicsData>();
		if (physData)
		{
			foreach (var col in physData.AllColliders)
			{
				foreach (var socketCol in colliders)
				{
					if (!socketCol.isTrigger)
						Physics.IgnoreCollision(col, socketCol, false);
				}
			}
		}
		base.OnSelectExited(args);
	}

}
