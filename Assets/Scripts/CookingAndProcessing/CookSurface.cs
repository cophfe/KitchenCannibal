using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CookSurface : MonoBehaviour
{
	static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");

	[SerializeField]
	Renderer cookSurfaceRenderer;
	[SerializeField]
	float emissionAmountCooking = 0.1f;
	[SerializeField]
	float emissionChangeSpeed = 1.0f;
	[SerializeField]
	RotationDetector[] rotationDetectors;
	[field: SerializeField]
	public UnityEvent<Cookable> OnCookableEnter { get; set; }
	
	List<Cookable> cookables = new List<Cookable>();
	int rotatedCount = 0;

	float currentEmission = 0;
	Material surfaceMaterial;

	bool cooking = false;

	private void Awake()
	{
		foreach (var detector in rotationDetectors)
		{
			detector.OnRotated.AddListener(OnRotated);
			detector.OnStopRotated.AddListener(OnUnRotated);
		}

		surfaceMaterial = cookSurfaceRenderer.material;
		surfaceMaterial.SetColor(emissionColor, Color.black);
		//this should be true but just in case
		cookSurfaceRenderer.sharedMaterial = surfaceMaterial;
		cooking = false;
	}

	void OnRotated()
	{
		rotatedCount++;

		if (rotatedCount > 0)
			cooking = true;
	}
	void OnUnRotated()
	{
		rotatedCount--;
		if (rotatedCount <= 0)
			cooking = false;
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
		if (!cooking)
			return;

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

	private void Update()
	{
		float target = cooking ? emissionAmountCooking : 0;
		if (currentEmission == target)
			return;
		currentEmission = Mathf.MoveTowards(currentEmission, target, Time.deltaTime * emissionChangeSpeed);
		surfaceMaterial.SetColor(emissionColor, currentEmission * Color.white);
		
	}
}
