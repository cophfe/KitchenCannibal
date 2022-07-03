using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandInfo", menuName = "ScriptableObjects/HandInfo", order = 1)]
public class HandInfo : ScriptableObject
{
	[field: SerializeField]
	public string InteractableLayer { get; private set; } = "Interactable";
	[field: SerializeField]
	public string IgnoreHandsLayer { get; private set; } = "HandIgnore";
	[field: SerializeField]
	public string InteractableTravellingLayer { get; private set; } = "InteractableTravel";
	[field: SerializeField]
	public float MoveToInteractableDistance { get; private set; } = 0.05f;
	[field: SerializeField]
	public float InteractableAttachAngle { get; private set; } = 3.0f;
	[field: SerializeField]
	public float InteractableAttachDistance { get; private set; } = 0.05f;
	[field: SerializeField]
	public float InteractableJointMassScale { get; private set; } = 1000.0f;

	[field: SerializeField]
	public float TintStartDistance { get; private set; } = 0.3f;
	[field: SerializeField]
	public float TintEndDistance { get; private set; } = 0.0f;
	[field: SerializeField]
	public float TintAlreadySelectedModifier { get; private set; } = 0.3f;

	[field: SerializeField]
	public float ControllerVisibleStart { get; private set; } = 2.0f;
	[field: SerializeField]
	public float ControllerVisibleRange { get; private set; } = 1.0f;

	[field: SerializeField]
	public float FingerSpeed { get; private set; } = 20.0f;
	
	//thumb gets different speed because it generally moves further than other fingers
	[field: SerializeField]
	public float ThumbSpeed { get; private set; } = 10.0f;
	[field: SerializeField]
	public float PoseSpeed { get; private set; } = 20.0f;
	[field: SerializeField, Range(0.0f, 1.0f)] 
	public float InputThreshold { get; private set; } = 0.03f;

	[field: SerializeField] 
	public int PartHandPoseLayer { get; private set; } = 5;

	[field: SerializeField]
	public int FullHandPoseLayer { get; private set; } = 6;

	[field: SerializeField]
	public PoseInfo[] Poses { get; private set; }

	//assumes is organised the same as the animator
	//returns pose index and layer
	public int FindPoseIndex(string name)
	{
		for (int i = 0; i < Poses.Length; i++)
		{
			if (Poses[i].Name == name)
			{
				return (i + 1);
			}
		}

		return 0;
	}

	public float FindPoseSpeedModifier(int poseIndex)
	{
		poseIndex--;
		if (poseIndex > 0 && poseIndex < Poses.Length)
			return Poses[poseIndex].SpeedModifier;
		return 1.0f;
	}

	public bool IsFullHandPose(int poseIndex)
	{
		poseIndex--;
		if (poseIndex > 0 && poseIndex < Poses.Length)
			return Poses[poseIndex].isFullHandPose;

		return false;
	}

	[System.Serializable]
	public struct PoseInfo
	{
		[field: SerializeField]
		public string Name { get; private set; }

		[field: SerializeField]
		public bool isFullHandPose { get; private set; }

		[field: SerializeField]
		public float SpeedModifier { get; private set; }
	}
}