using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.Assertions;

public class PhysicsLocomotion : TeleportationProvider
{
	[SerializeField]
	PlayerController playerController;
	
	[SerializeField]
	[Tooltip("The number of degrees clockwise to rotate when snap turning clockwise.")]
	private float m_TurnAmount = 45f;

	[SerializeField]
	private bool m_EnableTurnLeftRight = true;

	[SerializeField, Range(0, 1)]
	float percentToTurn = 0.8f;
	[SerializeField, Range(0, 1)]
	float percentToReset = 0.2f;

	[SerializeField]
	private InputActionProperty m_LeftHandSnapTurnAction;

	[SerializeField]
	private InputActionProperty m_RightHandSnapTurnAction;

	private float m_CurrentTurnAmount;
	int leftTurnValue = 0;
	int rightTurnValue = 0;

	public InputActionProperty leftHandSnapTurnAction
	{
		get
		{
			return m_LeftHandSnapTurnAction;
		}
		set
		{
			SetInputActionProperty(ref m_LeftHandSnapTurnAction, value);
		}
	}

	public InputActionProperty rightHandSnapTurnAction
	{
		get
		{
			return m_RightHandSnapTurnAction;
		}
		set
		{
			SetInputActionProperty(ref m_RightHandSnapTurnAction, value);
		}
	}

	protected void OnEnable()
	{
		leftHandSnapTurnAction.action.canceled += LCancelled;
		rightHandSnapTurnAction.action.canceled += RCancelled;

		leftHandSnapTurnAction.action.performed += LPerformed;
		rightHandSnapTurnAction.action.performed += RPerformed;
	
	}

	protected override void Update()
	{
		if (!validRequest || !BeginLocomotion())
		{
			return;
		}

		XROrigin xrOrigin = base.system.xrOrigin;
		if (xrOrigin != null)
		{
			switch (currentRequest.matchOrientation)
			{
				case MatchOrientation.WorldSpaceUp:
					xrOrigin.MatchOriginUp(Vector3.up);
					break;
				case MatchOrientation.TargetUp:
					xrOrigin.MatchOriginUp(currentRequest.destinationRotation * Vector3.up);
					break;
				case MatchOrientation.TargetUpAndForward:
					xrOrigin.MatchOriginUpCameraForward(currentRequest.destinationRotation * Vector3.up, currentRequest.destinationRotation * Vector3.forward);
					break;
				default:
					Assert.IsTrue(condition: false, string.Format("Unhandled {0}={1}.", "MatchOrientation", currentRequest.matchOrientation));
					break;
				case MatchOrientation.None:
					break;
			}

			Vector3 b = xrOrigin.Origin.transform.up * xrOrigin.CameraInOriginSpaceHeight;
			Vector3 desiredWorldLocation = currentRequest.destinationPosition + b;
			xrOrigin.MoveCameraToWorldLocation(desiredWorldLocation);

			TeleportHands();
		}

		EndLocomotion();
		validRequest = false;
	}

	void TeleportHands()
	{
		var lTarget = playerController.LeftHand.PhysicsHand.TargetHandModel;
		var rTarget = playerController.RightHand.PhysicsHand.TargetHandModel;
		playerController.LeftHand.PhysicsHand.Teleport(lTarget.position, lTarget.rotation);
		playerController.RightHand.PhysicsHand.Teleport(rTarget.position, rTarget.rotation);
	}

	public override bool QueueTeleportRequest(TeleportRequest teleportRequest)
	{
		playerController.LeftHand.EnableTeleporting(false);
		playerController.RightHand.EnableTeleporting(false);

		if (playerController.CheckValidTeleportPoint(teleportRequest.destinationPosition))
		{
			currentRequest = teleportRequest;
			validRequest = true;
			return true;
		}
		else
			return false;

	}

	private void LPerformed(InputAction.CallbackContext obj)
	{
		Vector2 input = obj.ReadValue<Vector2>();
		float turnAmount = GetTurnAmount(input);

		if (leftTurnValue == 0)
		{
			int sign = System.Math.Sign(input.x);
			if (sign * input.x >= percentToTurn && sign != leftTurnValue)
			{
				leftTurnValue = sign;
				Turn(turnAmount);
			}
		}
		else if (Mathf.Abs(input.x) < percentToReset)
		{
			leftTurnValue = 0;
		}
	}
	private void RPerformed(InputAction.CallbackContext obj)
	{
		Vector2 input = obj.ReadValue<Vector2>();
		float turnAmount = GetTurnAmount(input);

		if (rightTurnValue == 0)
		{
			int sign = System.Math.Sign(input.x);
			if (sign * input.x >= percentToTurn && sign != rightTurnValue)
			{
				rightTurnValue = sign;
				Turn(turnAmount);
			}
		}
		else if (Mathf.Abs(input.x) < percentToReset)
		{
			rightTurnValue = 0;
		}
	}	

	private void LCancelled(InputAction.CallbackContext obj)
	{	leftTurnValue = 0;	}
	private void RCancelled(InputAction.CallbackContext obj)
	{	rightTurnValue = 0;	}


	protected void OnDisable()
	{
		leftHandSnapTurnAction.action.canceled -= LCancelled;
		leftHandSnapTurnAction.action.canceled -= RCancelled;
	}

	private void SetInputActionProperty(ref InputActionProperty property, InputActionProperty value)
	{
		if (Application.isPlaying)
		{
			property.DisableDirectAction();
		}

		property = value;
		if (Application.isPlaying && base.isActiveAndEnabled)
		{
			property.EnableDirectAction();
		}
	}

	public float turnAmount
	{
		get
		{
			return m_TurnAmount;
		}
		set
		{
			m_TurnAmount = value;
		}
	}

	public bool enableTurnLeftRight
	{
		get
		{
			return m_EnableTurnLeftRight;
		}
		set
		{
			m_EnableTurnLeftRight = value;
		}
	}

	protected void Turn(float turnAmount)
	{
		StartTurn(turnAmount);

		if (Mathf.Abs(m_CurrentTurnAmount) > 0f && BeginLocomotion())
		{
			XROrigin xrOrigin = base.system.xrOrigin;
			if (xrOrigin != null)
			{
				xrOrigin.RotateAroundCameraUsingOriginUp(m_CurrentTurnAmount);

				TeleportHands();
			}

			m_CurrentTurnAmount = 0f;
			EndLocomotion();
		}
	}

	protected virtual float GetTurnAmount(Vector2 input)
	{
		if (input == Vector2.zero)
		{
			return 0f;
		}

		Cardinal nearestCardinal = CardinalUtility.GetNearestCardinal(input);
		switch (nearestCardinal)
		{
			case Cardinal.East:
				if (m_EnableTurnLeftRight)
				{
					return m_TurnAmount;
				}

				break;
			case Cardinal.West:
				if (m_EnableTurnLeftRight)
				{
					return 0f - m_TurnAmount;
				}

				break;
			default:
				break;
		}

		return 0f;
	}

	protected void StartTurn(float amount)
	{
		if (CanBeginLocomotion())
		{
			m_CurrentTurnAmount = amount;
		}
	}
}