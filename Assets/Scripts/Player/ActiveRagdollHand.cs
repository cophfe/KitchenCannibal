using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ActiveRagdollHand : MonoBehaviour
{
	[field: SerializeField]
	public Transform TargetHandModel { get; private set; }

	[Header("Force Data")]
	[SerializeField]
	DynamicSettings fingerSettings
		= new DynamicSettings(0);
	[SerializeField]
	DynamicSettings palmSettings
		= new DynamicSettings(0);

	[SerializeField]
	float maxJointRotationalForce = 1000.0f;
	[SerializeField]
	float maxJointForce = 3.402823e+38f;
	[SerializeField]
	float jointDamping = 0.5f;
	[SerializeField]
	bool applyWorldForceToFingers = true;
	[SerializeField]
	bool applyWorldTorqueToFingers = true;
	[SerializeField]
	bool applyWorldForceToPalm = false;
	[SerializeField]
	float palmDamping = 0.5f;
	[SerializeField]
	float palmOrientStrength = 1.0f;

	//data about children containing joints + rigidbody
	DynamicData[] dynamicBodies = null;
	DynamicData palm;
	bool applyWorldToPalm = true;
	float palmMass;
	Transform palmTargetOverrideTransform;

	public DynamicSettings PalmSettings { get => palmSettings; set => palmSettings = value; }
	public DynamicSettings FingerSettings { get => fingerSettings; set => fingerSettings = value; }
	public float PalmOrientStrength { get => palmOrientStrength; set => palmOrientStrength = value; }

	private void Awake()
	{
		List<DynamicData> dynamicBodiesList = new List<DynamicData>();

		palm = new DynamicData(TargetHandModel, transform);
		palmMass = palm.followBody.mass;

		FindJointDataRecursive(TargetHandModel.GetChild(1), transform.GetChild(1), dynamicBodiesList);

		//so I can set values easier I'm just going to convert these to arrays
		dynamicBodies = dynamicBodiesList.ToArray();

		SetValues();
	}

#if UNITY_EDITOR
	private void OnValidate()
	{
		if (Application.isPlaying && dynamicBodies != null)
		{
			SetValues();
		}
	}
#endif

	void SetValues()
	{
		JointDrive slerpDrive = new JointDrive();
		slerpDrive.positionSpring = maxJointRotationalForce;
		slerpDrive.positionDamper = jointDamping;
		slerpDrive.maximumForce = maxJointForce;

		foreach (var body in dynamicBodies)
		{
			//set initial joint values
			body.followJoint.slerpDrive = slerpDrive;

			//set body values
			body.followBody.angularDrag = fingerSettings.angularDrag;
			body.followBody.drag = fingerSettings.drag;
			body.followBody.maxAngularVelocity = fingerSettings.maxAngularVelocity;
		}

		palm.followBody.angularDrag = palmSettings.angularDrag;
		palm.followBody.drag = palmSettings.drag;
		palm.followBody.maxAngularVelocity = palmSettings.maxAngularVelocity;

		SetWorldForce(applyWorldForceToFingers, applyWorldTorqueToFingers, applyWorldToPalm);
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

	public void SetWorldForce(bool applyForceToFingers, bool applyTorqueToFingers, bool applyToPalm)
	{
		applyWorldToPalm = applyToPalm;
		applyWorldForceToFingers = applyForceToFingers;
		applyWorldTorqueToFingers = applyTorqueToFingers;

		if (!applyForceToFingers)
		{
			palm.followBody.mass = 30 * palmMass;
		}
		else
		{
			palm.followBody.mass = palmMass;
		}
	}

	public void SetPalmOverride(Transform palmOverride)
	{
		palmTargetOverrideTransform = palmOverride;
	}

	public Transform GetPalmOverride()
	{
		return palmTargetOverrideTransform;
	}

	public Transform GetPalmTarget()
	{
		if (palmTargetOverrideTransform != null)
			return palmTargetOverrideTransform;
		else
			return palm.targetTransform;
	}

	public DynamicData GetPalmData()
	{
		return palm;
	}

	public void Teleport(Vector3 newPosition, Quaternion newRotation)
	{
		palm.followTransform.position = newPosition;
		palm.followTransform.rotation = newRotation;
	}

	private void FixedUpdate()
	{
		float iDT = Time.deltaTime == 0 ? 0 : 1 / Time.deltaTime;

		bool applyWorldTorque = applyWorldTorqueToFingers && applyWorldToPalm && palmTargetOverrideTransform == null;
		bool applyWorldForce = applyWorldForceToFingers && applyWorldToPalm;

		if (applyWorldTorque && applyWorldForce)
		{
			for (int i = 0; i < dynamicBodies.Length; i++)
			{
				ActiveRagdollRotation(ref dynamicBodies[i], ref fingerSettings, iDT);
				ActiveRagdollPosition(ref dynamicBodies[i], ref fingerSettings, iDT);
				dynamicBodies[i].followJoint.targetRotation = Quaternion.Inverse(dynamicBodies[i].localToJointSpace) * Quaternion.Inverse(dynamicBodies[i].targetTransform.localRotation) * dynamicBodies[i].anchor;
			}
		}
		else if (applyWorldForce)
		{
			for (int i = 0; i < dynamicBodies.Length; i++)
			{
				ActiveRagdollPosition(ref dynamicBodies[i], ref fingerSettings, iDT);
				dynamicBodies[i].followJoint.targetRotation = Quaternion.Inverse(dynamicBodies[i].localToJointSpace) * Quaternion.Inverse(dynamicBodies[i].targetTransform.localRotation) * dynamicBodies[i].anchor;
			}
		}
		else
		{
			for (int i = 0; i < dynamicBodies.Length; i++)
			{
				dynamicBodies[i].followJoint.targetRotation = Quaternion.Inverse(dynamicBodies[i].localToJointSpace) * Quaternion.Inverse(dynamicBodies[i].targetTransform.localRotation) * dynamicBodies[i].anchor;
			}
		}

		if (applyWorldToPalm)
		{
			if (applyWorldForceToPalm)
				ActiveRagdollPosition(ref palm, ref palmSettings, iDT);
			//ActiveRagdollRotation(ref palm, ref palmSettings, iDT);
			//if (!applyWorldForceToFingers)
				OrientPalm(iDT);
		}

		/*
		for (int i = 0; i < dynamicBodies.Length; i++)
		{
			dynamicBodies[i].followBody.position = dynamicBodies[i].targetTransform.position;
			dynamicBodies[i].followBody.rotation = dynamicBodies[i].targetTransform.rotation;

			dynamicBodies[i].followBody.velocity = Vector3.zero;
			dynamicBodies[i].followBody.angularVelocity = Vector3.zero;
		}
		mainBody.followBody.position = mainBody.targetTransform.position;
		mainBody.followBody.rotation = mainBody.targetTransform.rotation;
		mainBody.followBody.velocity = Vector3.zero;
		mainBody.followBody.angularVelocity = Vector3.zero;
		 */
	}


	void ActiveRagdollRotation(ref DynamicData data, ref DynamicSettings settings, float inverseDeltaTime)
	{
		Quaternion fixRotationalForce = data.targetTransform.rotation * Quaternion.Inverse(data.followTransform.rotation);
		fixRotationalForce.ToAngleAxis(out float angle, out Vector3 axis);
		//fix angle switching randomly
		if (angle > 180.0f)
			angle -= 360.0f;
		angle = Mathf.Clamp(angle, -settings.maxAngleInfluence, settings.maxAngleInfluence);

		Vector3 fixRotationAxis = angle * axis;

		Vector3 angularVelocity;
		if (angle != 360.0f && angle != 0)
		{
			angularVelocity = settings.rotationalStrength * (fixRotationAxis + settings.rotationalStrengthChange * (fixRotationAxis - data.lastFixRotation) * inverseDeltaTime);
			data.lastFixRotation = fixRotationAxis;
		}
		else
			angularVelocity = new Vector3(0f, 0f, 0f);

		angularVelocity = Vector3.ClampMagnitude(angularVelocity, settings.maxRotationalForce);
		data.followBody.AddTorque(angularVelocity, ForceMode.VelocityChange);
	}

	void ActiveRagdollPosition	(ref DynamicData data, ref DynamicSettings settings, float inverseDeltaTime)
	{
		Vector3 targetPlusCOM;
		if (palmTargetOverrideTransform)
		{
			targetPlusCOM = data.targetTransform.TransformPoint(data.followBody.centerOfMass);
			targetPlusCOM = palm.targetTransform.InverseTransformPoint(targetPlusCOM);
			//targetPlusCOM = palmTargetOverrideTransform.TransformPoint(targetPlusCOM);

			//only needs to be done once per joint
			var worldToLocalMatrix = Matrix4x4.TRS(palmTargetOverrideTransform.position, palmTargetOverrideTransform.rotation, Vector3.one);
			targetPlusCOM = worldToLocalMatrix.MultiplyPoint3x4(targetPlusCOM);
		}
		else
			targetPlusCOM = data.targetTransform.TransformPoint(data.followBody.centerOfMass);

		//if (localToPalm) 
		//{
		//	targetPlusCOM = data.targetTransform.TransformPoint(data.followBody.centerOfMass);
		//	targetPlusCOM = mainBody.targetTransform.InverseTransformPoint(targetPlusCOM);
		//	targetPlusCOM = mainBody.followTransform.TransformPoint(targetPlusCOM);
		//
		//}
		Vector3 fixForce = Vector3.ClampMagnitude(targetPlusCOM - data.followBody.worldCenterOfMass, settings.maxDistanceInfluence);

#if UNITY_EDITOR
		Debug.DrawRay(data.followBody.worldCenterOfMass, fixForce, Color.red, Time.fixedDeltaTime, false);
#endif

		Vector3 targetForce = settings.linearStrength * (fixForce + settings.linearStrengthChange * (fixForce - data.lastFixForce) * inverseDeltaTime);
		data.lastFixForce = fixForce;

		targetForce = Vector3.ClampMagnitude(targetForce, settings.maxForce);
		data.followBody.AddForce(targetForce, ForceMode.VelocityChange);
	}

	void OrientPalm(float inverseDeltaTime)
	{
		palm.followBody.angularVelocity *= 1f - palmDamping;
		Quaternion rot = palmTargetOverrideTransform ? palmTargetOverrideTransform.rotation : palm.targetTransform.rotation;
		Quaternion fixRotationalForce = rot * Quaternion.Inverse(palm.followTransform.rotation);
		fixRotationalForce.ToAngleAxis(out float angle, out Vector3 axis);
		//fix angle switching randomly
		if (angle > 180.0f)
			angle -= 360.0f;

		if (angle != 360.0f && angle != 0)
		{
			Vector3 angularVelocity = axis * (angle * Mathf.Deg2Rad) * inverseDeltaTime;
			if (!float.IsNaN(angularVelocity.x))
			{
				palm.followBody.angularVelocity += angularVelocity * palmOrientStrength;
				//Debug.Log(palm.followBody.angularVelocity);

				//for (int i = 0; i < dynamicBodies.Length; i++)
				//{
				//	OrientJointToPalm(ref dynamicBodies[i], ref fingerSettings, inverseDeltaTime);
				//}
			}
		}
	}

	/*void OrientJointToPalm(ref DynamicData data, ref DynamicSettings settings, float inverseDeltaTime)
	{
		Vector3 targetPlusCOM;
		targetPlusCOM = data.targetTransform.TransformPoint(data.followBody.centerOfMass);
		targetPlusCOM = mainBody.targetTransform.InverseTransformPoint(targetPlusCOM);
		targetPlusCOM = mainBody.followTransform.TransformPoint(targetPlusCOM);
		Vector3 fixForce = Vector3.ClampMagnitude(targetPlusCOM - data.followBody.worldCenterOfMass, settings.maxDistanceInfluence);

#if UNITY_EDITOR
		Debug.DrawRay(data.followBody.worldCenterOfMass, fixForce, Color.red, Time.fixedDeltaTime, false);
#endif

		Vector3 targetForce = settings.linearStrength * (fixForce + settings.linearStrengthChange * (fixForce - data.lastFixForce) * inverseDeltaTime);
		data.lastFixForce = fixForce;

		targetForce = Vector3.ClampMagnitude(targetForce, settings.maxForce);
		data.followBody.AddForce(targetForce, ForceMode.VelocityChange);
	}*/
	public struct DynamicData
	{
		public Transform targetTransform;

		public Transform followTransform;
		public Rigidbody followBody;
		public ConfigurableJoint followJoint;

		public Quaternion anchor;
		public Quaternion localToJointSpace;

		public Vector3 lastFixRotation;
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

			lastFixRotation = Vector3.zero;
			lastFixForce = Vector3.zero;
			

		}
	}

	[System.Serializable]
	public struct DynamicSettings
	{
		public float maxRotationalForce;
		public float maxForce;

		public float angularDrag;
		public float drag;
		public float maxAngularVelocity;

		public float maxDistanceInfluence;
		public float maxAngleInfluence;

		public float rotationalStrength;
		public float linearStrength;
		public float rotationalStrengthChange;
		public float linearStrengthChange;

		public DynamicSettings(float parameterless)
		{
			maxRotationalForce = 1000.0f;
			maxForce = 1000.0f;

			angularDrag = 50.0f;
			drag = 0.5f;
			maxAngularVelocity = 1000.0f;

			maxDistanceInfluence = 1.0f;
			maxAngleInfluence = 20.0f;

			rotationalStrength = 2.0f;
			linearStrength = 30.0f;
			rotationalStrengthChange = 0.002f;
			linearStrengthChange = 0.01f;
		}
	}
}
