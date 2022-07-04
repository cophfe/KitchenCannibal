using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(RotationDetector), typeof(CustomGrabInteractable), typeof(HingeJoint))]
public class DoorClose : MonoBehaviour
{
	[SerializeField]
	AudioSource doorSource;
	[SerializeField]
	float closedForce = 20;
	[SerializeField]
	float closedDamper= 0.5f;
	[SerializeField]
	float slamForce = 100;
	[SerializeField]
	float slamDamper = 0.5f;

	RotationDetector rd;
	CustomGrabInteractable inter;
	HingeJoint joint;
	
	bool slamming = false;
	JointSpring slam;
	JointSpring normal;
	JointSpring closed;
	public bool Closed { get; private set; }

	bool selected;
    void Start()
    {
		if (!doorSource)
			doorSource = GetComponent<AudioSource>();
		Closed = true;
		rd = GetComponent<RotationDetector>();
		inter = GetComponent<CustomGrabInteractable>();
		joint = GetComponent<HingeJoint>();

		inter.firstSelectEntered.AddListener(OnSelect);
		inter.lastSelectExited.AddListener(OnDeselect);

		normal = joint.spring;

		closed.spring = closedForce;
		closed.damper = closedDamper;
		closed.targetPosition = normal.targetPosition;

		slam.spring = slamForce;
		slam.damper = slamDamper;
		slam.targetPosition = normal.targetPosition;
	}

	private void FixedUpdate()
	{
		if (!selected)
		{
			if (!Closed && !rd.Rotated)
				OnClose();
			else if (Closed && rd.Rotated)
				OnOpen();
		}
	}

	void OnSelect(SelectEnterEventArgs args)
	{
		if (Closed)
			OnOpen();
		selected = true;
	}

	void OnDeselect(SelectExitEventArgs args)
	{
		selected = false;
		if (!rd.Rotated)
			OnClose();
	}

	void OnClose()
	{
		if (slamming)
		{
			inter.enabled = true;
			slamming = false;
		}
		else
		{
			joint.spring = closed;
		}

		doorSource?.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Fridge, 6));
		Closed = true;
	}

	void OnOpen()
	{
		if (slamming)
			return;

		Closed = false;
		joint.spring = normal;
		doorSource?.PlayOneShot(GameManager.Instance.audioManager.GetClip(SoundSources.Fridge, 0));
	}

	public void Slam()
	{
		Closed = false;
		inter.enabled = false;
		slamming = true;
		joint.spring = slam;
	}

}
