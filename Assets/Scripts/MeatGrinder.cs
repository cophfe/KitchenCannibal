using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatGrinder : MonoBehaviour
{
	[SerializeField]
	CollisionTriggerEvent recordTrigger;
	[SerializeField]
	CollisionTriggerEvent addTrigger;
	[SerializeField]
	LayerMask grindableLayers;
	[SerializeField]
	Collider[] ignoreColliders;

	List<Grindable> recordedGrindables;

	private void Start()
	{
		recordedGrindables = new List<Grindable>();
	}

	private void OnEnable()
	{
		recordTrigger.OnTriggerEnterCallback += OnRecordEnter;
		recordTrigger.OnTriggerExitCallback += OnRecordExit;

		addTrigger.OnTriggerEnterCallback += OnAddEnter;
		addTrigger.OnTriggerExitCallback += OnAddExit;
	}
	private void OnDisable()
	{
		recordTrigger.OnTriggerEnterCallback -= OnRecordEnter;
		recordTrigger.OnTriggerExitCallback -= OnRecordExit;

		addTrigger.OnTriggerEnterCallback -= OnAddEnter;
		addTrigger.OnTriggerExitCallback -= OnAddExit;
	}


	private void OnRecordEnter(Collider other)
	{
		if ((1 << other.gameObject.layer & grindableLayers.value) != 0)
		{
			Debug.Log(other.attachedRigidbody + "ENTERED");

			var grindable = other.attachedRigidbody.GetComponentInParent<Grindable>();
			if (grindable)
			{
				if(grindable.RecordedColliders == 0)
				{
					recordedGrindables.Add(grindable);
					grindable.RecordedColliders++;
					foreach (var col in grindable.GrindColliders)
					{
						foreach (var igCol in ignoreColliders)
						{
							Physics.IgnoreCollision(col, igCol);
						}
					}
				}
				else
				{
					grindable.RecordedColliders++;
				}
				
				Debug.Log("entering: " + grindable.RecordedColliders);

			}
		}
	}

	private void OnRecordExit(Collider other)
	{
		if ((1 << other.gameObject.layer & grindableLayers.value) != 0)
		{
			var grindable = other.attachedRigidbody.GetComponentInParent<Grindable>();
			if (grindable && grindable.RecordedColliders > 0 && recordedGrindables.Contains(grindable))
			{
				grindable.RecordedColliders--;
				if (grindable.RecordedColliders <= 0)
				{
					recordedGrindables.Remove(grindable);
					foreach (var col in grindable.GrindColliders)
					{
						foreach (var igCol in ignoreColliders)
						{
							Physics.IgnoreCollision(col, igCol, false);
						}
					}
				}

				Debug.Log("exiting: " + grindable.RecordedColliders);
			}
		}
	}

	private void OnAddEnter(Collider other)
	{
	}

	private void OnAddExit(Collider other)
	{
	}
}
