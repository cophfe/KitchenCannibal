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

	//usually only 1 object (in my game) is marked unselectable, so this allows me to get the second closest interactable in that case
	public IXRInteractable[] Get2ClosestValidTargets(out float distSq1, out float distSq2)
	{
		IXRInteractable t1 = null;
		IXRInteractable t2 = null;

		float d1 = 1000000;
		float d2 = 1000000;

		foreach (var uvT in unsortedValidTargets)
		{
			float dist2 = uvT.GetDistanceSqrToInteractor(this);
			if (dist2 < d1)
			{
				t2 = t1;
				t1 = uvT;

				d2 = d1;
				d1 = dist2;
			}
			else if (dist2 < d2)
			{
				t2 = uvT;
				d2 = dist2;
			}
		}

		distSq1 = d1;
		distSq2 = d2;

		return new IXRInteractable[2]{ t1, t2};
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
