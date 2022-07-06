using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CustomGrabInteractable), typeof(Rigidbody))]
public class Bone : MonoBehaviour
{
	[SerializeField]
	ParticleSystem blood;
	[SerializeField]
	Ingredient limbIngredient;
	[SerializeField]
	CustomGrabInteractable limbInteractable;
	[SerializeField]
	InteractionLayerMask layerMask;
	[SerializeField]
	InteractionLayerMask nothingLayerMask;

	[SerializeField]
	Transform breakAttach;
	[SerializeField]
	Transform breakLAttach;
	[SerializeField]
	Transform breakRAttach;

	CustomGrabInteractable boneInteractable;
	Rigidbody rb;
	FixedJoint joint;

	public AudioSource audioSource = null;

	private void OnEnable()
	{
		joint = GetComponent<FixedJoint>();
		rb = GetComponent<Rigidbody>();
		boneInteractable = GetComponent<CustomGrabInteractable>();

		boneInteractable.EnableSelecting = false;
		boneInteractable.lastSelectExited.AddListener(OnSelectExit);
		boneInteractable.firstSelectEntered.AddListener(OnSelectEnter);

		limbInteractable.firstSelectEntered.AddListener(OnSelectLimbEnter);
		limbInteractable.lastSelectExited.AddListener(OnSelectLimbExit);
	}

	private void OnDisable()
	{
		boneInteractable.firstSelectEntered.RemoveListener(OnSelectEnter);
		boneInteractable.lastSelectExited.RemoveListener(OnSelectExit);

		limbInteractable.firstSelectEntered.RemoveListener(OnSelectLimbEnter);
		limbInteractable.lastSelectExited.RemoveListener(OnSelectLimbExit);
	}

	void OnSelectLimbEnter(SelectEnterEventArgs args)
	{
		boneInteractable.interactionLayers = layerMask;
		boneInteractable.EnableSelecting = true;

	}
	void OnSelectLimbExit(SelectExitEventArgs args)
	{
		boneInteractable.interactionLayers = nothingLayerMask;
		boneInteractable.EnableSelecting = false;
	}


	void OnSelectEnter(SelectEnterEventArgs args)
	{
		
	}
	
	void OnSelectExit(SelectExitEventArgs args)
	{
		if (!joint)
		{
			enabled = false;
			transform.parent = null;

		}
	}

	private void OnJointBreak(float breakForce)
	{
		limbInteractable.lastSelectExited.RemoveListener(OnSelectLimbExit);

		var physData = boneInteractable.GetComponent<InteractablePhysicsData>();
		physData.SetAttachData(breakAttach, breakLAttach, breakRAttach);
		physData.SetRestricted(false);

		limbInteractable.FirstInteractorTakesPriority = false;
		
		//reset bone selection
		if (boneInteractable.isSelected )
		{
			var selector = boneInteractable.interactorsSelecting[0];
			boneInteractable.interactionManager.SelectExit(selector, boneInteractable);
			boneInteractable.interactionManager.SelectEnter(selector, boneInteractable);

		}

		limbIngredient.hasBoneShards = false;
		if (blood)
			blood.Play();
		audioSource.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Bone, 2));
	}
}
