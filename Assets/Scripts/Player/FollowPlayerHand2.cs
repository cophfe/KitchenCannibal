using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//based on animfollow package

public class FollowPlayerHand2 : MonoBehaviour
{
	[SerializeField]
	Transform playerHand;
	[SerializeField]
	float maxForce = 0.5f;
	Rigidbody main;
	List<JointData> jointDatas;
	List<(Transform, Transform)> transformPairs;

	private void Start()
	{
		jointDatas = new List<JointData>();
		main = GetComponent<Rigidbody>();
		main.maxAngularVelocity = 20;

		
		FindJointDataRecursive(playerHand.GetChild(1), transform.GetChild(1));
	}

	void FindJointDataRecursive(Transform targetParent, Transform physicsParent)
	{
		var joint = physicsParent.gameObject.GetComponent<ConfigurableJoint>();
		if (joint != null)
		{
			jointDatas.Add(	new JointData(targetParent, joint));
		}
		transformPairs.Add((targetParent, physicsParent));

		int count = targetParent.childCount;
		for (int i = 0; i < count; i++)
		{
			FindJointDataRecursive(targetParent.GetChild(i), physicsParent.GetChild(i));
		}
	}

	private void Update()
	{
		//main.transform.position = playerHand.position;
		//main.transform.rotation = playerHand.rotation;
	}

	private void FixedUpdate()
	{
		float iDT = Time.deltaTime == 0 ? 0 : 1 / Time.deltaTime;
		for (int i = 0; i < jointDatas.Count; i++)
		{
			var rb = jointDatas[i].followBody;
			//Vector3 vel = Vector3.MoveTowards(Vector3.zero, (jointDatas[i].target.position - rb.position), maxForce);
			//rb.velocity = vel * iDT;
			Quaternion rot =  (jointDatas[i].targetTransform.localRotation * Quaternion.Inverse(jointDatas[i].followTransform.localRotation));
			//rot = jointDatas[i].joint.transform.parent.rotation * (rot * jointDatas[i].joint.transform.localRotation);
			Quaternion localRot = (rot * jointDatas[i].followTransform.localRotation);
			Quaternion worldRot = (jointDatas[i].followTransform.parent.rotation * localRot);
			Quaternion delta = worldRot * Quaternion.Inverse(jointDatas[i].followTransform.rotation);

			//rb.transform.localRotation = rb.transform.localRotation * rot;// Quaternion.RotateTowards(Quaternion.identity, rot, Time.deltaTime);
			delta.ToAngleAxis(out float angle, out Vector3 axis);
			if (angle > 180)
				angle = angle - 360;	
			if (angle != 0)
			{
				rb.angularVelocity = Mathf.Deg2Rad * angle * iDT * axis;
			}
			//jointDatas[i].joint.transform.rotation = localRot * jointDatas[i].joint.transform.parent.rotation;
			//jointDatas[i].joint.transform.rotation = jointDatas[i].joint.transform.parent.rotation * localRot;
		}

		{
			Vector3 vel = Vector3.MoveTowards(Vector3.zero, (playerHand.position - main.position), maxForce);
			main.velocity = vel * iDT;
		
			Quaternion rot = (playerHand.rotation * Quaternion.Inverse(transform.rotation));
			rot.ToAngleAxis(out float angle, out Vector3 axis);
			if (angle > 180)
				angle = angle - 360;
			if (angle != 0)
				main.angularVelocity = Mathf.Deg2Rad * angle * iDT * axis;
		}
	}

	struct JointData
	{
		public Transform targetTransform;

		public Transform followTransform;
		public Rigidbody followBody;
		public ConfigurableJoint followJoint;

		public Quaternion anchor;
		public Quaternion localToJointSpace;

		public Vector3 rbPosToCOM;
		public JointData(Transform target, ConfigurableJoint followJoint)
		{
			this.targetTransform = target;
			this.followJoint = followJoint;
			this.followTransform = followJoint.transform;
			this.followBody = followJoint.GetComponent<Rigidbody>();

			var right = followJoint.axis;
			var forward = Vector3.Cross(followJoint.axis, followJoint.secondaryAxis).normalized;
			var up = Vector3.Cross(forward, right).normalized;
			localToJointSpace = Quaternion.LookRotation(forward, up);
			//anchor in joint space
			anchor = followTransform.localRotation * localToJointSpace;

			rbPosToCOM = Quaternion.Inverse(followTransform.rotation) * (followBody.worldCenterOfMass - followBody.position);
		}
	}
}
