using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DirectInteractor : XRDirectInteractor
{
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
}
