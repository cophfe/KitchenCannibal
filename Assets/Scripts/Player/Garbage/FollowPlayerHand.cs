using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerHand : MonoBehaviour
{
	[SerializeField]
	Transform playerHand = null;
	[SerializeField]
	float maxForce = 0.5f;
	[SerializeField]
	public bool flip = false;
	Rigidbody main;
	List<JointData> jointDatas;
	private void Start()
	{
		jointDatas = new List<JointData>();
		main = GetComponent<Rigidbody>();
		main.maxAngularVelocity = 20;

		AddRigidbodyChildrenRecursive(playerHand.GetChild(1), transform.GetChild(1));
	}

	void AddRigidbodyChildrenRecursive(Transform targetParent, Transform physicsParent)
	{
		var joint = physicsParent.gameObject.GetComponent<ConfigurableJoint>();
		if (joint != null)
		{
			jointDatas.Add(	new JointData(targetParent, joint));
		}

		int count = targetParent.childCount;
		for (int i = 0; i < count; i++)
		{
			AddRigidbodyChildrenRecursive(targetParent.GetChild(i), physicsParent.GetChild(i));
		}
	}

	private void Update()
	{
		//main.transform.position = playerHand.position;
		//main.transform.rotation = playerHand.rotation;
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < jointDatas.Count; i++)
		{
			//WHY DO CONFIGURABLE JOINTS HAVE THEIR OWN COORDINATE SPACE?????
			Quaternion rotation = Quaternion.Inverse(jointDatas[i].jointSpace) * (Quaternion.Inverse(jointDatas[i].target.localRotation) * jointDatas[i].anchor);
			rotation *= jointDatas[i].jointSpace;
			jointDatas[i].joint.targetRotation = rotation;
		}

		float iDT = Time.deltaTime == 0 ? 0 : 1 / Time.deltaTime;
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
		public Transform target;
		public ConfigurableJoint joint;
		public Quaternion anchor;
		public Quaternion jointSpace;

		public JointData(Transform target, ConfigurableJoint joint)
		{
			this.target = target;
			this.joint = joint;
			anchor = joint.transform.localRotation;


			var right = joint.axis;
			var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
			var up = Vector3.Cross(forward, right).normalized;
			jointSpace = Quaternion.LookRotation(forward, up);

			joint.GetComponent<Rigidbody>().maxAngularVelocity = 3000;
		}
	}
}
