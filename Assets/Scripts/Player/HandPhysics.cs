using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPhysics : MonoBehaviour
{
	[SerializeField]
	Transform target;
	[SerializeField]
	float maxForce = 20;

	Rigidbody rb;
	new Transform transform;

	Quaternion lastRotation;
    void Start()
    {
		transform = gameObject.transform;
		rb = GetComponent<Rigidbody>();

		transform.position = target.position;
		rb.maxAngularVelocity = 1000000;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		FollowTarget();
	}

	private void Update()
	{
		//transform.rotation = target.rotation;
	}

	void FollowTarget()
	{
		float iDT = Time.deltaTime == 0 ? 0 : (1 / Time.deltaTime);
		Vector3 vel = Vector3.MoveTowards(Vector3.zero, (target.position - transform.position), maxForce);
		rb.velocity = vel * iDT;

		Quaternion rot = (target.rotation * Quaternion.Inverse(transform.rotation));
		rot.ToAngleAxis(out float angle, out Vector3 axis);
		
		rb.angularVelocity = Mathf.Deg2Rad * angle * iDT * axis;
		//lastRotation = transform.rotation;
		//Vector3 angularVelocity = Mathf.Deg2Rad * angle * iDT * axis;

	}
}
