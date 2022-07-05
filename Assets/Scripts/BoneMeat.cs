using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CustomGrabInteractable), typeof(Ingredient))]
public class BoneMeat : MonoBehaviour
{
	[SerializeField]
	InteractionLayerMask everythingMask;
	[SerializeField]
	InteractionLayerMask nothingMask;
	[SerializeField]
	Transform boneParent;
	[SerializeField]
	AudioClip pickBone;

	CustomGrabInteractable meat;
	Ingredient ingredient;
	CustomGrabInteractable[] bonePieces;
	AudioSource src;

	int pieceCount = 0;
	private void Awake()
	{
		src = GetComponent<AudioSource>();
		meat = GetComponent<CustomGrabInteractable>();
		ingredient = GetComponent<Ingredient>();

		 bonePieces = boneParent.gameObject.GetComponentsInChildren<CustomGrabInteractable>();
		foreach (var piece in bonePieces)
		{
			piece.interactionLayers = nothingMask;
			piece.firstSelectEntered.AddListener(OnPickupBonePiece);
		}

		pieceCount = bonePieces.Length;


		meat.firstSelectEntered.AddListener(OnPickupMeat);
		meat.lastSelectExited.AddListener(OnDropMeat);
	}

	void OnPickupMeat(SelectEnterEventArgs args)
	{
		if (args.interactableObject == null || bonePieces == null)
			return;

		foreach (var piece in bonePieces)
		{
			if (piece)
				piece.interactionLayers = everythingMask;
		}
	}
	void OnDropMeat(SelectExitEventArgs args)
	{
		if (args.interactableObject == null || bonePieces == null)
			return;

		foreach (var piece in bonePieces)
		{
			if (piece)
				piece.interactionLayers = nothingMask;
		}
	}

	void OnPickupBonePiece(SelectEnterEventArgs args)
	{
		if (args.interactableObject == null || bonePieces == null)
			return;

		pieceCount--;
		var inter = args.interactableObject as CustomGrabInteractable;
		if (inter)
		{
			inter.firstSelectEntered.RemoveListener(OnPickupBonePiece);
			inter.HandDetectDistanceModifer *= 0.6f;
		}
		else
			Debug.Log("Could not find interactable component");

		if (src)
			src.PlayOneShot(pickBone);

		for (int i = 0; i < bonePieces.Length; i++)
		{
			if (bonePieces[i] == inter)
			{
				bonePieces[i] = null;
				break;
			}

		}

		if (pieceCount <= 0)
		{
			ingredient.hasBoneShards = false;
			meat.FirstInteractorTakesPriority = false;
			bonePieces = null;
		}
	}
}
