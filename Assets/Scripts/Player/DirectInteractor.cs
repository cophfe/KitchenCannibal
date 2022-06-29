using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectInteractor : XRDirectInteractor
{
	public System.Action<Collider> onTriggerEnter;

	new private void OnTriggerEnter(Collider other)
	{
		if (enabled)
			onTriggerEnter?.Invoke(other);
		base.OnTriggerEnter(other);
	}

	public List <IXRInteractable> UnsortedValidTargets => unsortedValidTargets; 

	public IXRInteractable GetClosesetValidTarget(out float distanceSq)
	{
		IXRInteractable target = null;
		float dist = 1000000;
		foreach (var uvT in unsortedValidTargets)
		{
			float dist2 = uvT.GetDistanceSqrToInteractor(this);
			if (dist2 < dist)
			{
				target = uvT;
				dist = dist2;
			}
		}

		distanceSq = dist;
		return target;
	}

	public virtual void OnSelectForceExit(IXRSelectInteractable interactable)
	{
		interactionManager.SelectExit(this, interactable);
	}

	public void OnSelectForceEnter(IXRSelectInteractable interactable)
	{
		interactionManager.SelectEnter(this, interactable);
	}

	public void SwapSelectedInteractable(IXRSelectInteractable interactable)
	{
		if (hasSelection)
		{
			
			interactionManager.SelectExit(this, interactablesSelected[0]);
			interactionManager.SelectEnter(this, interactable);
		}
	}

	public override Transform GetAttachTransform(IXRInteractable interactable)
	{
		return base.GetAttachTransform(interactable);
	}
}
