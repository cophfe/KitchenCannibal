using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
	[SerializeField]
	Transform target;

	Rigidbody rb;
	new Transform transform;

    void Start()
    {
		transform = gameObject.transform;
		rb = GetComponent<Rigidbody>();

		transform.position = target.position;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		FollowTarget();
	}

	void FollowTarget()
	{
		float iDT = 1 / (Time.deltaTime == 0 ? Mathf.Infinity : Time.deltaTime);
		rb.velocity = (target.position - transform.position) * iDT;

		(target.rotation * Quaternion.Inverse(transform.rotation)).ToAngleAxis(out float angle, out Vector3 axis);
		rb.angularVelocity = angle * Mathf.Deg2Rad * iDT * axis;
	}
}
