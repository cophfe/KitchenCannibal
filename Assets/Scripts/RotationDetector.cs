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
	UnityEvent onRotated;
	[SerializeField]
	UnityEvent onStopRotated;
	[SerializeField]
	UnityEvent whileRotated;

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
			whileRotated?.Invoke();
		}
		//if is rotated and wasn't previously rotated
		else if (isRotated)
		{
			onRotated?.Invoke();
			whileRotated?.Invoke();
			rotated = true;
		}
		//if is previously rotated and is no longer rotated
		else if (rotated)
		{
			onStopRotated?.Invoke();
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

		//this is wrong for some joint axis but eh i don't care this is used for one small thing
		Vector3 rot = Vector3.right;

		Gizmos.color = Color.cyan;
		Vector3 vec = Quaternion.AngleAxis(rotationAmount, joint.axis) * rot;
		Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.14f);

		if (joint.useLimits)
		{
			Gizmos.color = Color.red;

			vec = Quaternion.AngleAxis(joint.limits.max, joint.axis) * rot;
			Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.1f);
			vec = Quaternion.AngleAxis(joint.limits.min, joint.axis) * rot;
			Gizmos.DrawRay(transform.position, transform.TransformDirection(vec) * 0.1f);
		}
		
	}
}
