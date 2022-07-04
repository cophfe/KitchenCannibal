using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HingeJoint))]
public class RotationDetector : MonoBehaviour
{
	[SerializeField]
	float rotationAmount;
	[SerializeField]
	Vector3 secondaryAxis = Vector3.right;

	[field: SerializeField]
	public UnityEvent OnRotated {  get; private set; }
	[field: SerializeField]
	public UnityEvent OnStopRotated { get; private set; }
	[field: SerializeField]
	public UnityEvent WhileRotated { get; private set; }

	bool rotated = false;

	bool inverseComparison;

	HingeJoint joint;

	private void Start()
	{
		inverseComparison = rotationAmount < 0;
	}

	private void FixedUpdate()
	{
		CheckRotated();
	}

	void CheckRotated()
	{
		if (inverseComparison)
			CallEvents(joint.angle < rotationAmount);
		else
			CallEvents(joint.angle > rotationAmount);
	}

	void CallEvents(bool isRotated)
	{
		//if is rotated and was previously rotated
		if (isRotated && rotated)
		{
			WhileRotated?.Invoke();
		}
		//if is rotated and wasn't previously rotated
		else if (isRotated)
		{
			OnRotated?.Invoke();
			WhileRotated?.Invoke();
			rotated = true;
		}
		//if is previously rotated and is no longer rotated
		else if (rotated)
		{
			OnStopRotated?.Invoke();
			rotated = false;
		}
	}

	private void OnEnable()
	{
		joint = GetComponent<HingeJoint>();
	}

	private void OnDisable()
	{
		CallEvents(false);
	}

	private void OnDrawGizmosSelected()
	{
		var joint = GetComponent<HingeJoint>();

		Gizmos.color = Color.cyan;
		Vector3 vec = Quaternion.AngleAxis(rotationAmount, joint.axis) * secondaryAxis;
		Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.14f);

		if (joint.useLimits)
		{
			Gizmos.color = Color.red;

			vec = Quaternion.AngleAxis(joint.limits.max, joint.axis) * secondaryAxis;
			Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.1f);
			vec = Quaternion.AngleAxis(joint.limits.min, joint.axis) * secondaryAxis;
			Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.1f);
		}
		
	}
}
