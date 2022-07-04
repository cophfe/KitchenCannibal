using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//can be selected by only two things at once
[CanSelectMultiple(true)]
public class TwoHandedGrabInteractable : CustomGrabInteractable
{
	[SerializeField]
	Transform rightHandAttach;
	//m_AttachTransform is leftHandAttach
	[SerializeField, Range(0, 1)]
	float singleHandVelocityDamping = 1;
	[SerializeField]
	float singleHandVelocityScale = 1;
	[SerializeField, Range(0,1)]
	float singleHandRotationDamping = 1;
	[SerializeField]
	float singleHandRotationScale = 1;
	[SerializeField]
	float singleHandPullMaxDistance = 1;
	[SerializeField]
	float singleHandGravityModifier = 0.8f;

	bool LeftHandIsFirstAttached => isSelected && (interactorsSelecting[0] as HandInteractor).AttachedManager.IsLeftHand;
	bool TwoHandSelected => interactorsSelecting.Count == 2;

	InteractablePhysicsData data;
	protected override void Awake()
	{
		data = GetComponent<InteractablePhysicsData>();

		if (data.PhysicsLeftHandAttachPoint)
			m_AttachTransform.position = data.PhysicsLeftHandAttachPoint.position;
		if (data.PhysicsRightHandAttachPoint)
			rightHandAttach.position = data.PhysicsRightHandAttachPoint.position;

		selectMode = InteractableSelectMode.Multiple;
		base.Awake();
	}

	enum HandAttached
	{
		LeftRight,
		RightLeft,
		Left,
		Right
	}
	public override bool IsSelectableBy(IXRSelectInteractor interactor)
	{
		return (interactor is HandInteractor) && (interactorsSelecting.Count < 2 || interactorsSelecting.Contains(interactor));
	}

	public override Transform GetAttachTransform(IXRInteractor interactor)
	{
		if (interactor != null && (interactor is HandInteractor)
			&& !(interactor as HandInteractor).AttachedManager.IsLeftHand)
		{
			return rightHandAttach != null ? rightHandAttach : base.GetAttachTransform(interactor); 
		}
		else 
			return m_AttachTransform != null ? m_AttachTransform : base.GetAttachTransform(interactor);
	
	}

	protected override void Grab()
	{
		if (interactorsSelecting.Count == 2)
			SecondHandGrab();
		else
		{
			base.Grab();
		}
	}
	
	protected override void Drop()
	{
		if (interactorsSelecting.Count == 2)
			SecondHandDrop();
		else
			base.Drop();
	}

	void SecondHandGrab()
	{
	}

	void SecondHandDrop()
	{
	}

	
	public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
		base.ProcessInteractable(updatePhase);
	}

	protected override Vector3 GetWorldAttachPosition(IXRInteractor interactor)
	{
		if (TwoHandSelected)
		{
			Transform tA = interactorsSelecting[0].GetAttachTransform(this);
			Transform tB = interactorsSelecting[1].GetAttachTransform(this);

			Vector3 pA = tA.position;
			Vector3 pB = tB.position;
			return (pA + pB) / 2.0f;
		}
		else
			return base.GetWorldAttachPosition(interactor);
	}

	protected override Quaternion GetWorldAttachRotation(IXRInteractor interactor)
	{
		if (TwoHandSelected)
		{
			Transform tA = interactorsSelecting[0].GetAttachTransform(this);
			Transform tB = interactorsSelecting[1].GetAttachTransform(this); 
			
			Vector3 pA = tA.position;
			Vector3 pB = tB.position;
			//return Quaternion.Slerp((interactorsSelecting[0] as XRBaseInteractor).GetAttachTransform(this).rotation,  (interactorsSelecting[1] as XRBaseInteractor).GetAttachTransform(this).rotation, 0.5f) * m_InteractorLocalRotation;

			Vector3 avForward = ((tA.forward + tB.forward) / 2.0f).normalized;
			Vector3 up = ((tA.up + tB.up) / 2.0f).normalized;
			//Vector3 forward = Vector3.Cross(up, (pB - pA).normalized);
			Vector3 right = (pB - pA).normalized;
			Vector3 forward = Vector3.Cross(right, Vector3.Cross(right, avForward)).normalized;
			return Quaternion.LookRotation(forward, up);
			
		}
		else
			return base.GetWorldAttachRotation(interactor);
	}

	protected override void PerformVelocityTrackingUpdate(float timeDelta, XRInteractionUpdateOrder.UpdatePhase updatePhase)
	{
		if (TwoHandSelected)
			base.PerformVelocityTrackingUpdate(timeDelta, updatePhase);
		else
		{
			if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Fixed)
			{
				if (m_TrackPosition)
				{
					// Scale initialized velocity by prediction factor
					m_Rigidbody.velocity *= (1f - singleHandVelocityDamping);
					var positionDelta = m_AttachPointCompatibilityMode == AttachPointCompatibilityMode.Default
						? m_TargetWorldPosition - transform.position
						: m_TargetWorldPosition - m_Rigidbody.worldCenterOfMass;
					positionDelta = Vector3.ClampMagnitude(positionDelta, singleHandPullMaxDistance);
					var velocity = positionDelta / timeDelta;

					if (!float.IsNaN(velocity.x))
						m_Rigidbody.velocity += (velocity * singleHandVelocityScale);
				}

				// Do angular velocity tracking
				if (m_TrackRotation)
				{
					// Scale initialized velocity by prediction factor
					m_Rigidbody.angularVelocity *= (1f - singleHandRotationDamping);
					var rotationDelta = m_TargetWorldRotation * Quaternion.Inverse(transform.rotation);
					rotationDelta.ToAngleAxis(out var angleInDegrees, out var rotationAxis);
					if (angleInDegrees > 180f)
						angleInDegrees -= 360f;

					if (Mathf.Abs(angleInDegrees) > Mathf.Epsilon)
					{
						var angularVelocity = (rotationAxis * (angleInDegrees * Mathf.Deg2Rad)) / timeDelta;
						if (!float.IsNaN(angularVelocity.x))
							m_Rigidbody.angularVelocity += (angularVelocity * singleHandRotationScale);
					}
				}

				Vector3 start = (LeftHandIsFirstAttached ? data.PhysicsRightHandAttachPoint : data.PhysicsLeftHandAttachPoint).position;
				Vector3 force = Physics.gravity * singleHandGravityModifier;
				m_Rigidbody.AddForceAtPosition(force, start);
			}
		}
	}
}
