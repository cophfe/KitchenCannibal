using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class MeatGrinder : MonoBehaviour
{
	[SerializeField]
	Transform blade0;
	[SerializeField]
	Transform blade1;
	[SerializeField]
	CollisionTriggerEvent recordTrigger;
	[SerializeField]
	CollisionTriggerEvent addTrigger;
	[SerializeField]
	LayerMask grindableLayers;
	[SerializeField]
	Collider[] ignoreColliders;
	[SerializeField]
	HingeJoint handleJoint;
	[SerializeField]
	float angleToMeatSpeed = 1/360.0f;
	[SerializeField]
	Rigidbody meatPrefab;
	[SerializeField]
	Rigidbody boneyMeatPrefab;
	[SerializeField]
	Rigidbody slowBody;
	[SerializeField]
	Transform meatStart;
	[SerializeField]
	Transform meatEnd;
	[SerializeField]
	float backwardsDampeningModifier = 10;
	[SerializeField]
	UnityEvent onMeatAdded;
	[SerializeField]
	UnityEvent onMeatProcessed;

	List<Grindable> recordedGrindables;
	public AudioSource audioSource;

	float heldMeat = 0;
	float processedMeat = 0;
	float angle = 0;
	bool hasBoneShards = false;
	bool currentMeatHasBoneShards = false;

	float lastDeltaAngle = 0;
	float drag;
	Rigidbody meat = null;
	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		recordedGrindables = new List<Grindable>();
		angle = handleJoint.angle;
		drag = slowBody.drag;
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
			//Debug.Log(other.attachedRigidbody + "ENTERED");

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
			}
		}
	}

	private void OnAddEnter(Collider other)
	{
		if ((1 << other.gameObject.layer & grindableLayers.value) != 0)
		{
			var grindable = other.attachedRigidbody.GetComponentInParent<Grindable>();
			if (grindable && grindable.RecordedColliders > 0 && recordedGrindables.Contains(grindable))
			{
				grindable.RecordedColliders = 0;
				recordedGrindables.Remove(grindable);

				Process(grindable);
			}
		}
	}

	void Process(Grindable grindable)
	{
		onMeatAdded?.Invoke();
		heldMeat += grindable.ingredientAmount;
		hasBoneShards |= grindable.hasBoneShards;
		//if (meat && hasBoneShards)
		//{
		//	var newMeat = Instantiate(boneyMeatPrefab);
		//	newMeat.detectCollisions = false;
		//	newMeat.isKinematic = true;
		//	var interactable = newMeat.GetComponent<XRBaseInteractable>();
		//	if (interactable != null)
		//		interactable.enabled = false;
		//
		//	newMeat.transform.position = meat.transform.position;
		//	newMeat.transform.rotation = meat.transform.rotation;
		//	Destroy(meat);
		//	meat = newMeat;
		//	hasBoneShards = false;
		//}
		Destroy(grindable.gameObject);
		//(switch with return to pool, if do this will also need to undo ignore collision and reset transforms)
	}

	private void OnAddExit(Collider other)
	{
	}

	private void FixedUpdate()
	{
		float currentAngle = handleJoint.angle;
		float deltaAngle = Mathf.DeltaAngle(angle, currentAngle);

		if (deltaAngle < 0)
		{
			slowBody.drag = drag * backwardsDampeningModifier;
			//Debug.Log(angle + ": Moving backwards");
		}
		else
		{
			slowBody.drag = drag;
			//Debug.Log(angle + ": Moving forwards");

			if (heldMeat > 0 && heldMeat + processedMeat >= 0.98f)
			{
				float newHeld = Mathf.Max(0, heldMeat - deltaAngle * angleToMeatSpeed);
				processedMeat += heldMeat - newHeld;
				heldMeat = newHeld;

				UpdateMeat();
			}
		}

		if (heldMeat + processedMeat >= 0.98f && lastDeltaAngle < 0.0001f && deltaAngle > 0.0001f)
			OnStartGrind();
		if (deltaAngle < 0.0001f && lastDeltaAngle > 0.0001f)
			OnEndGrind();

		lastDeltaAngle = deltaAngle;

		angle = currentAngle;

		Vector3 axis = transform.TransformDirection(handleJoint.axis);
		if (blade0)
			blade0.Rotate(axis, angle);
		if (blade1)
			blade1.Rotate(axis, -angle);
	}

	void UpdateMeat()
	{
		if (processedMeat >= 0.98f)
		{
			processedMeat %= 1;
		
			if (meat != null)
			{
				currentMeatHasBoneShards = false;
				meat.position = meatEnd.position;
				meat.rotation = meatEnd.rotation;
				meat.detectCollisions = true;
				meat.isKinematic = false;
				var interactable = meat.GetComponent<XRBaseInteractable>();
				if (interactable != null)
					interactable.enabled = true;
				meat = null;
			}
		}
		else if (processedMeat > 0)
		{
			if (meat == null)
			{
				meat = Instantiate(hasBoneShards ? boneyMeatPrefab : meatPrefab);
				currentMeatHasBoneShards = hasBoneShards;
				hasBoneShards = false;
				//need to fix bug. this will cause other bug tho
				meat.detectCollisions = false;
				meat.isKinematic = true;
				var interactable = meat.GetComponent<XRBaseInteractable>();
				if (interactable != null)
					interactable.enabled = false;
				
				onMeatProcessed?.Invoke();
			}

			meat.position = Vector3.Lerp(meatStart.position, meatEnd.position, processedMeat);
			meat.rotation = Quaternion.Lerp(meatStart.rotation, meatEnd.rotation, processedMeat);
		}
		
	}


	public void OnStartGrind()
	{
		if (currentMeatHasBoneShards)
		{
			audioSource.clip = GameManager.Instance.audioManager.GetClip(SoundSources.Limb, 0);
		}
		else
		{
			audioSource.clip = GameManager.Instance.audioManager.GetClip(SoundSources.Limb, 6);
		}

		audioSource.loop = true;
		audioSource.Play();
	}

	public void OnEndGrind()
    {
		audioSource.loop = false;
		audioSource.Stop();
	}
}
