using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandInfo", menuName = "ScriptableObjects/HandInfo", order = 1)]
public class HandInfo : ScriptableObject
{
	[field: SerializeField]
	public float FingerSpeed { get; private set; } = 20.0f;
	[field: SerializeField]
	public float PoseSpeed { get; private set; } = 20.0f;
	[field: SerializeField, Range(0.0f, 1.0f)] 
	public float InputThreshold { get; private set; } = 0.03f;

	[field: SerializeField] 
	public int PoseLayer { get; private set; } = 5;
	

	[field: SerializeField]
	public PoseInfo[] Poses { get; private set; }

	//assumes is organised the same as the animator
	public int FindPoseIndex(string name)
	{
		for (int i = 0; i < Poses.Length; i++)
		{
			if (Poses[i].Name == name)
			{
				return i + 1;
			}
		}

		return 0;
	}

	//assumes is organised the same as the animator
	public float FindPoseSpeedModifier(int poseIndex)
	{
		poseIndex--;
		if (poseIndex < 0 || poseIndex >= Poses.Length)
			return Poses[poseIndex].SpeedModifier;
		return 1.0f;
	}

	[System.Serializable]
	public struct PoseInfo
	{
		[field: SerializeField]
		public string Name { get; private set; }

		[field: SerializeField]
		public AvatarMask PoseMask { get; private set; }

		[field: SerializeField]
		public float SpeedModifier { get; private set; }
	}
}