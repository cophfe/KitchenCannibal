using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractablePreviewData", menuName = "ScriptableObjects/InteractablePreviewData", order = 3)]
public class InteractablePreviewData : ScriptableObject
{
	[field: SerializeField]
	public Animator LeftHandAnimatedPrefab { get; private set; }
	[field: SerializeField]
	public Animator RightHandAnimatedPrefab { get; private set; }
	[field: SerializeField]
	public Transform LeftControllerPrefab { get; private set; }
	[field: SerializeField]
	public Transform RightControllerPrefab { get; private set; }
	[field: SerializeField]
	public Material OverrideMaterial { get; private set; }
	[field: SerializeField]
	public Material ControllerOverrideMaterial { get; private set; }

}
