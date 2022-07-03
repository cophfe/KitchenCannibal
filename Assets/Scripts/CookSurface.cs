using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CookSurface : MonoBehaviour
{
	[SerializeField]
	RotationDetector[] rotationDetectors;
	[field: SerializeField]
	public UnityEvent<Cookable> OnCookableEnter { get; set; }
	
	List<Cookable> cookables = new List<Cookable>();
	int rotatedCount = 0;

	private void Awake()
	{
		foreach (var detector in rotationDetectors)
		{
			detector.OnRotated.AddListener(OnRotated);
			detector.OnStopRotated.AddListener(OnUnRotated);
		}

		enabled = false;
	}

	void OnRotated()
	{
		rotatedCount++;

		if (rotatedCount > 0)
			enabled = true;
	}
	void OnUnRotated()
	{
		rotatedCount--;
		if (rotatedCount <= 0)
			enabled = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		var r = other.attachedRigidbody;
		if (!r)
			return;
		var c = r.GetComponent<Cookable>();
		if (!c)
			return;
	
		cookables.Add(c);
	}

	private void OnTriggerExit(Collider other)
	{
		var r = other.attachedRigidbody;
		if (!r)
			return;
		var c = r.GetComponent<Cookable>();
		if (!c)
			return;

		if (cookables.Contains(c))
		{
			cookables.Remove(c);
		}
	}

	private void FixedUpdate()
	{
		foreach (var c in cookables)
		{
			if (c)
			{
				if (!c.PreviousCookState)
					OnCookableEnter?.Invoke(c);
				c.Cooking |= true;
			}
		}
	}
}
