using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ActiveRagdollHand : MonoBehaviour
{
	[SerializeField]
	Transform targetHandModel;

	[Header("Force Data")]
	[SerializeField]
	float maxRotationalForce = 100.0f;
	[SerializeField]
	float maxForce = 100.0f;
	[SerializeField]
	float maxJointRotationalForce = 1000.0f;
	[SerializeField]
	float jointDamping = 0.5f;

	[SerializeField]
	float drag = 0.5f;
	[SerializeField]
	float angularDrag = 50.0f;
	[SerializeField]
	float maxAngularVelocity = 1000.0f;
	[SerializeField]
	float maxWorldDistance = 1.0f;
	[SerializeField]
	float maxWorldAngle = 20.0f;

	[Header("PD")]
	[SerializeField]
	float rotationalStrength = 0.2f;
	[SerializeField]
	float linearStrength = 30.0f;
	[SerializeField]
	public float rotationalStrengthDerivitive = .002f;
	[SerializeField]
	public float linearStrengthDerivitive = .01f;


	//data about children containing joints + rigidbody
	DynamicData[] dynamicBodies;

	private void Start()
	{
		List<DynamicData> dynamicBodiesList = new List<DynamicData>();

		dynamicBodiesList.Add(new DynamicData(targetHandModel, transform));

		FindJointDataRecursive(targetHandModel.GetChild(1), transform.GetChild(1), dynamicBodiesList);
		
		//so I can set values easier I'm just going to convert these to arrays
		dynamicBodies = dynamicBodiesList.ToArray();


		JointDrive slerpDrive = new JointDrive();

		foreach (var body in dynamicBodies)
		{
			//set initial joint values
			if (body.followJoint)
			{
				slerpDrive.positionSpring = maxJointRotationalForce;
				slerpDrive.positionDamper = jointDamping;
				body.followJoint.slerpDrive = slerpDrive;
			}

			//set body values
			body.followBody.angularDrag = angularDrag;
			body.followBody.drag = drag;
			body.followBody.maxAngularVelocity = maxAngularVelocity;
		}
	}

	void FindJointDataRecursive(Transform targetParent, Transform physicsParent, List<DynamicData> dList)
	{
		var joint = physicsParent.gameObject.GetComponent<ConfigurableJoint>();
		if (joint)
		{
			dList.Add(new DynamicData(targetParent, physicsParent));
		}

		int count = targetParent.childCount;
		for (int i = 0; i < count; i++)
		{
			FindJointDataRecursive(targetParent.GetChild(i), physicsParent.GetChild(i), dList);
		}
	}

	private void FixedUpdate()
	{
		float iDT = Time.deltaTime == 0 ? 0 : 1 / Time.deltaTime;

		float angle;
		Vector3 axis;
		for (int i = 0; i < dynamicBodies.Length; i++)
		{
			//Do rotational velocity
			{
				Quaternion fixRotationalForce = dynamicBodies[i].targetTransform.rotation * Quaternion.Inverse(dynamicBodies[i].followTransform.rotation);
				fixRotationalForce.ToAngleAxis(out angle, out axis);
				//fix angle switching randomly
				if (angle > 180.0f)
					angle -= 360.0f;
				angle = Mathf.Clamp(angle, -maxWorldAngle, maxWorldAngle);

				Vector3 fixRotationAxis = angle * axis;

				Vector3 targetRotation;
				if (angle != 360.0f && angle != 0)
				{
					targetRotation = rotationalStrength * (fixRotationAxis + rotationalStrengthDerivitive * (fixRotationAxis - dynamicBodies[i].lastFixRotationalForce) * iDT);
					dynamicBodies[i].lastFixRotationalForce = fixRotationAxis;
				}
				else
					targetRotation = new Vector3(0f, 0f, 0f);

				targetRotation = Vector3.ClampMagnitude(targetRotation, maxRotationalForce);
				//apply rotational impulse
				dynamicBodies[i].followBody.AddTorque(targetRotation, ForceMode.VelocityChange);
			}

			{
				Vector3 targetPlusCOM = dynamicBodies[i].targetTransform.position + dynamicBodies[i].targetTransform.rotation * dynamicBodies[i].rbPosToCOM;
				Vector3 fixForce = Vector3.ClampMagnitude(targetPlusCOM - dynamicBodies[i].followBody.worldCenterOfMass, maxWorldDistance);

					 
				Vector3 targetForce = linearStrength * (fixForce + linearStrengthDerivitive * (fixForce - dynamicBodies[i].lastFixForce) * iDT);
				dynamicBodies[i].lastFixForce = fixForce;

				targetForce = Vector3.ClampMagnitude(targetForce, maxForce);
				dynamicBodies[i].followBody.AddForce(targetForce, ForceMode.VelocityChange);
			}

			if (dynamicBodies[i].followJoint)
				dynamicBodies[i].followJoint.targetRotation = Quaternion.Inverse(dynamicBodies[i].localToJointSpace) * Quaternion.Inverse(dynamicBodies[i].followTransform.localRotation) * dynamicBodies[i].anchor;
		}
	}

	struct DynamicData
	{
		public Transform targetTransform;

		public Transform followTransform;
		public Rigidbody followBody;
		public ConfigurableJoint followJoint;

		public Quaternion anchor;
		public Quaternion localToJointSpace;

		public Vector3 rbPosToCOM;

		public Vector3 lastFixRotationalForce;
		public Vector3 lastFixForce;
		
		public DynamicData(Transform target, Transform follow)
		{
			targetTransform = target;
			followJoint = follow.GetComponent<ConfigurableJoint>();
			followTransform = follow;
			followBody = follow.GetComponent<Rigidbody>();

			if (followJoint)
			{
				var right = followJoint.axis;
				var forward = Vector3.Cross(followJoint.axis, followJoint.secondaryAxis).normalized;
				var up = Vector3.Cross(forward, right).normalized;
				localToJointSpace = Quaternion.LookRotation(forward, up);
			}
			else
			{
				localToJointSpace = Quaternion.identity;
			}
			//anchor in joint space
			anchor = followTransform.localRotation * localToJointSpace;

			rbPosToCOM = Quaternion.Inverse(followTransform.rotation) * (followBody.worldCenterOfMass - followBody.position);
			lastFixRotationalForce = Vector3.zero;
			lastFixForce = Vector3.zero;
			

		}
	}
}
