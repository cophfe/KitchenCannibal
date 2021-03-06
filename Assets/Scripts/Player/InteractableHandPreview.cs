using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class InteractableHandPreview : MonoBehaviour
{
	[HideInInspector, SerializeField]
	InteractablePreviewData previewData = null;
	[SerializeField]
	bool enablePreview = true;
	[SerializeField]
	bool enableController = true;
	[SerializeField]
	bool enableHand = true;
	[SerializeField]
	bool handInGrabPosition = true;

	[SerializeField]
	Hand hand = Hand.Right;
	[SerializeField]
	bool reload;
	[SerializeField]
	bool constantReloadAnimation;
	
	[HideInInspector, SerializeField]
	HandInfo handInfo;

	[HideInInspector, SerializeField]
	bool loaded = false;
	enum Hand
	{
		Left,
		Right
	}

	[HideInInspector, SerializeField]
	Animator animator;
	[HideInInspector, SerializeField]
	Transform controller;
	[HideInInspector, SerializeField]
	InteractablePhysicsData physData;
	[HideInInspector, SerializeField]
	XRBaseInteractable interactable;

#if UNITY_EDITOR

	void OnEnable()
    {
		Load();
	}

	
	private void OnDisable()
	{
		Unload();
	}

	private void Load()
	{
		if (loaded)
			return;
		if (!enablePreview)
			return;

		loaded = true;

		interactable = GetComponent<XRBaseInteractable>();
		if (!interactable)
		{
			Debug.LogError("Hand Preview has to be attached to interactable");
			DestroyImmediate(this);
			return;
		}

		string[] guids = AssetDatabase.FindAssets("t: " + typeof(InteractablePreviewData));
		if (guids.Length > 0)
		{
			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			previewData = AssetDatabase.LoadAssetAtPath<InteractablePreviewData>(path);
		}
		else
		{
			Debug.LogWarning("Interactable Preview requires at an instance of InteractablePreviewData to exist in the assets folder");
		}

		if (enableHand && previewData && previewData.LeftHandAnimatedPrefab && previewData.RightHandAnimatedPrefab)
		{
			Animator prefab = hand == Hand.Left ? previewData.LeftHandAnimatedPrefab : previewData.RightHandAnimatedPrefab;
			animator = Instantiate(prefab, Vector3.zero, Quaternion.identity);
			animator.name = gameObject.name + " preview hand please ignore";

			if (previewData.OverrideMaterial)
			{
				var renderer = prefab.GetComponentInChildren<Renderer>();
				if (renderer)
					renderer.sharedMaterial = previewData.OverrideMaterial;
			}

			physData = GetComponent<InteractablePhysicsData>();
			if (physData)
			{
				guids = AssetDatabase.FindAssets("t: " + typeof(HandInfo));
				if (guids.Length > 0)
				{
					string path = AssetDatabase.GUIDToAssetPath(guids[0]);
					handInfo = AssetDatabase.LoadAssetAtPath<HandInfo>(path);
					
					if (handInfo)
					{
						if (handInGrabPosition)
						{
							int poseIndex = handInfo.FindPoseIndex(physData.HandGrabPose);
							if (poseIndex != 0)
							{
								bool isFullHandPose = handInfo.IsFullHandPose(poseIndex);
								int poseLayer;
								string poseIndexID;
								if (isFullHandPose)
								{
									poseLayer = handInfo.FullHandPoseLayer;
									poseIndexID = "Pose Index";
								}
								else
								{
									poseLayer = handInfo.PartHandPoseLayer;
									poseIndexID = "Part Hand Pose Index";
								}

								animator.speed = 0;
								animator.SetFloat(poseIndexID, poseIndex);
								animator.SetLayerWeight(poseLayer, 1);
								animator.Update(0);
							}
						}
						
					}
				}
				else
				{
					Debug.LogWarning("Interactable Preview requires an instance of HandInfo to exist in the assets folder for animation mirroring to work");
				}
			}
		}

		if (enableController && previewData && previewData.LeftControllerPrefab && previewData.RightControllerPrefab)
		{
			Transform prefab = hand == Hand.Left ? previewData.LeftControllerPrefab : previewData.RightControllerPrefab;
			controller = Instantiate(prefab, Vector3.zero, Quaternion.identity);
			controller.name = gameObject.name + " preview controller please ignore";

			if (previewData.OverrideMaterial)
			{
				var renderer = prefab.GetComponentInChildren<Renderer>();
				if (renderer)
					renderer.sharedMaterial = previewData.ControllerOverrideMaterial;
			}

			physData = GetComponent<InteractablePhysicsData>();
		}
	}

	private void OnValidate()
	{
		reload = true;
		SceneView.RepaintAll();
	}

	private void Update()
	{
		if (reload)
		{
			reload = false;
			Unload();
			Load();
		}
		if (constantReloadAnimation)
		{
			ReloadAnimation();
		}

		SetHandPosition();
	}

	void SetHandPosition()
	{
		if (loaded && animator)
		{
			Transform trackTransform = transform;

			if (handInGrabPosition)
			{
				if (physData)
				{
					if (hand == Hand.Left && physData.PhysicsLeftHandAttachPoint)
					{
						trackTransform = physData.PhysicsLeftHandAttachPoint;
					}
					else if (physData.PhysicsRightHandAttachPoint)
						trackTransform = physData.PhysicsRightHandAttachPoint;
				}
			}
			else if (interactable)
				trackTransform = interactable.GetAttachTransform(null);
			
			animator.transform.position = trackTransform.position;
			animator.transform.rotation = trackTransform.rotation;
		}
		if (loaded && controller)
		{
			Transform trackTransformPos = transform;
			Transform trackTransformRot = transform;

			if (handInGrabPosition)
			{
				if (physData && interactable)
				{
					if (physData.UniformAttachTransform)
					{
						trackTransformPos = interactable.GetAttachTransform(null);
					}
					else
					{
						if (hand == Hand.Left && physData.PhysicsLeftHandAttachPoint)
						{
							trackTransformPos = physData.PhysicsLeftHandAttachPoint;
						}
						else if (physData.PhysicsRightHandAttachPoint)
							trackTransformPos = physData.PhysicsRightHandAttachPoint;
					}
					
					trackTransformRot = interactable.GetAttachTransform(null);
				}
			}
			else if (interactable)
			{
				trackTransformPos = interactable.GetAttachTransform(null);
				trackTransformRot = interactable.GetAttachTransform(null);
			}

			controller.transform.position = trackTransformPos.position;
			controller.transform.rotation = trackTransformRot.rotation;
		}
	}

	private void Unload()
	{
		if (!loaded)
			return;

		if (animator)
		{
			DestroyImmediate(animator.gameObject);
			animator = null;
		}
		if (controller)
		{
			DestroyImmediate (controller.gameObject);
			controller = null;
		}
		loaded = false;
	}

	private void OnDestroy()
	{
		Unload();
	}
#endif

	void ReloadAnimation()
	{
		if (handInGrabPosition && handInfo != null && loaded)
		{
			int poseIndex = handInfo.FindPoseIndex(physData.HandGrabPose);
			if (poseIndex != 0)
			{
				bool isFullHandPose = handInfo.IsFullHandPose(poseIndex);
				int poseLayer;
				string poseIndexID;
				if (isFullHandPose)
				{
					poseLayer = handInfo.FullHandPoseLayer;
					poseIndexID = "Pose Index";
				}
				else
				{
					poseLayer = handInfo.PartHandPoseLayer;
					poseIndexID = "Part Hand Pose Index";
				}

				animator.speed = 0;
				animator.SetInteger(poseIndexID, poseIndex);
				animator.SetLayerWeight(poseLayer, 1);
				animator.Update(0);
			}
		}
	}
}
