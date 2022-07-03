using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class RotateInteractable : CustomGrabInteractable
{
	[SerializeField]
	Vector3 rotationAxis = Vector3.right;

	Quaternion startRotationInteractable;
	Quaternion startRotationInteractor;
	protected override void Grab()
	{
		base.Grab();
		startRotationInteractor = interactorsSelecting[0].transform.rotation;
		startRotationInteractable = transform.rotation;
	}
	protected override Quaternion GetWorldAttachRotation(IXRInteractor interactor)
	{
		if (!m_TrackRotation)
			return m_TargetWorldRotation;


		//var interactorAttachTransform = overrideTarget ? overrideTarget : interactor.GetAttachTransform(this);
		
		//Debug.DrawRay(transform.position, startRotationInteractor * Vector3.up * 0.2f, 0.4f * Color.red, Time.deltaTime, false);
		//Debug.DrawRay(transform.position, interactor.transform.rotation * Vector3.up * 0.2f, Color.red, Time.deltaTime, false);

		Quaternion rotateAmount = (interactor.transform.rotation * Quaternion.Inverse(startRotationInteractor));
		Quaternion rot = rotateAmount * startRotationInteractable;
		//Debug.DrawRay(transform.position, startRotationInteractable * Vector3.up * 0.2f, 0.4f* Color.green, Time.deltaTime, false);
		//Debug.DrawRay(transform.position, rot * Vector3.up * 0.2f, Color.green, Time.deltaTime, false);
		return rot;

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(transform.position, transform.TransformDirection(rotationAxis) * 0.1f);
	}
}
