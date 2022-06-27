using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

//handles input for a single hand
public class HandManager : MonoBehaviour
{
	[field: SerializeField] 
	public Animator ControllerAnimator { get; private set; }
	[field: SerializeField] 
	public Animator HandAnimator { get; private set;  }

	[field: SerializeField] 
	public SkinnedMeshRenderer ControllerRenderer { get; private set; }
	[field: SerializeField] 
	public ActiveRagdollHand PhysicsHand { get; private set; }
	[field: SerializeField]
	public ActionBasedController Controller { get; private set; }

	[SerializeField]
	XRRayInteractor teleporter = null;
	[SerializeField]
	XRDirectInteractor interactor = null;
	[SerializeField]
	LineRenderer teleporterRenderer = null;

	[Header("Values")]
	[SerializeField] HandInfo handInfo = null;
	[SerializeField] string grabLayer = "";
	[SerializeField] bool isLeftHand = true;

	[Header("Inputs")]
	//for hand and controller
	[SerializeField] InputActionProperty grip;
	[SerializeField] InputActionProperty trigger;
	
	//for controller
	[SerializeField] InputActionProperty button1;
	[SerializeField] InputActionProperty button2;
	[SerializeField] InputActionProperty joystick;

	//for hand
	[SerializeField] InputActionProperty button1Touched;
	[SerializeField] InputActionProperty button2Touched;
	[SerializeField] InputActionProperty joystickTouched;
	[SerializeField] InputActionProperty triggerTouched;
	[SerializeField] InputActionProperty gripTouched;
	
	//for teleport 
	[SerializeField] InputActionProperty thumbstick;
	[SerializeField] InputActionProperty teleportActivate;
	[SerializeField] InputActionProperty teleportCancel;

	//for tracking fix
	[SerializeField] InputActionProperty trackingState;

	//controller also
	int triggerID;
	int gripID;
	int joystickXID;
	int joystickYID;
	int button1ID;
	int button2ID;

	int thumbID;
	int indexID;
	int middleID;
	int ringID;
	int pinkyID;
	int fullHandPoseIndexID;
	int partHandPoseIndexID;

	//input values
	float gripPercent;
	float triggerPercent;
	int thumbButtonsTouched = 0;

	float thumbTarget = 0;
	float indexTarget = 0;
	float middleTarget = 0;
	float ringTarget = 0;
	float pinkyTarget = 0;
	float poseAmountTarget = 0;

	float thumbCurrent = 0;
	float indexCurrent = 0;
	float middleCurrent = 0;
	float ringCurrent = 0;
	float pinkyCurrent = 0;
	
	//pose amount in current used pose layer
	float poseAmountCurrent = 0;
	//pose amount in other non used pose layer
	float otherPoseAmountCurrent = 0;
	int currentPoseIndex = 0;
	int lastPoseIndex = 0;
	int overridePose = 0;
	int poseLayer;
	int otherPoseLayer;

	//pose indexes
	int okIndex;
	int fingerGunIndex;
	float poseSpeedModifier = 1.0f;

	//Controller stuff
	public bool ControllerVisible { get => controllerVisible; set { SetControllerVisible(value); } }
	bool controllerVisible = true;
	//controller opacity
	int opacityId;
	float fullOpacity;
	Material controllerMaterial;

	//teleport stuff
	bool isTeleporting;
	bool canTeleport = true;
	Vector2 teleportDirection;
	//tracking
	int lastTrackingState = -1;

	//Interaction
	GrabState grabState = GrabState.NotGrabbing;
	int interactableLayerIndex;
	int grabLayerIndex;

	public enum GrabState
	{
		NotGrabbing,
		StartingGrab,
		MovingToInteractable,
		Grabbed
	}
	struct InteractableInfo {
		public IXRSelectInteractable interactable;
		public InteractablePhysicsData physicsData;
	}
	InteractableInfo grabbedInteractable;

	FixedJoint handJoint;
	SkinnedMeshRenderer physicsRenderer;
	List<Collider> interactorColliders;

	public bool CanTeleport
	{
		get => canTeleport && teleporter != null;
		set
		{
			if (value == CanTeleport)
				return;

			canTeleport = value;

			if (!value && isTeleporting)
			{
				EnableTeleporting(false);
			}
		}
	}

	public PlayerController PlayerController { get; set; }

	private void Start()
	{
		EnableTeleporting(false);
		triggerID = Animator.StringToHash("Trigger");
		gripID = Animator.StringToHash("Grip");
		joystickXID = Animator.StringToHash("Joy X");
		joystickYID = Animator.StringToHash("Joy Y");
		button1ID = Animator.StringToHash("Button 1");
		button2ID = Animator.StringToHash("Button 2");

		thumbID = Animator.StringToHash("Thumb");
		indexID = Animator.StringToHash("Index");
		middleID = Animator.StringToHash("Middle");
		ringID = Animator.StringToHash("Ring");
		pinkyID = Animator.StringToHash("Pinky");
		fullHandPoseIndexID = Animator.StringToHash("Pose Index");
		partHandPoseIndexID = Animator.StringToHash("Part Hand Pose Index");

		okIndex = handInfo.FindPoseIndex("OK");
		fingerGunIndex = handInfo.FindPoseIndex("Finger Gun");

		trigger.action.performed += OnTriggerPress;
		trigger.action.canceled += OnTriggerPress;
		grip.action.performed += OnGripPress;
		grip.action.canceled += OnGripPress;
		button1Touched.action.performed += OnButton1Touched;
		button1Touched.action.canceled += OnButton1Touched;
		button2Touched.action.performed += OnButton2Touched;
		button2Touched.action.canceled += OnButton2Touched;
		joystickTouched.action.performed += OnJoystickTouched;
		joystickTouched.action.canceled += OnJoystickTouched;

		triggerTouched.action.performed += OnTriggerTouched;
		triggerTouched.action.canceled += OnTriggerTouched;
		gripTouched.action.performed +=	OnGripTouched;
		gripTouched.action.canceled += OnGripTouched;

		FindFingersTarget();
		thumbCurrent = thumbTarget;
		indexCurrent = indexTarget;
		middleCurrent = middleTarget;
		ringCurrent = ringTarget;
		pinkyCurrent = pinkyTarget;
		poseAmountCurrent = poseAmountTarget;

		//find controller stuff
		opacityId = Shader.PropertyToID("Opacity");
		var materialList = new List<Material>();
		ControllerRenderer.GetMaterials(materialList);
		controllerMaterial = materialList[0];
		fullOpacity = controllerMaterial.GetFloat(opacityId);
		SetControllerVisible(false);

		//teleport stuff
		teleportActivate.action.performed += OnTeleportActivate;
		teleportCancel.action.performed += OnTeleportCancel;
		joystick.action.performed += OnTeleportDirection;

		trackingState.action.performed += OnTrackingState;
		trackingState.action.canceled += OnTrackingState;

		var target = PhysicsHand.TargetHandModel;
		PhysicsHand.Teleport(target.position, target.rotation);

		//interaction stuff
		interactor.selectEntered.AddListener(OnGrabObject);
		interactor.selectExited.AddListener(OnDropObject);

		grabLayerIndex = LayerMask.NameToLayer(grabLayer);
		interactableLayerIndex = LayerMask.NameToLayer(handInfo.InteractableLayer);

		physicsRenderer = PhysicsHand.GetComponentInChildren<SkinnedMeshRenderer>();
		interactorColliders = new List<Collider>();
	}

	public GrabState GetGrabState()
	{
		return grabState;
	}

	void OnGrabObject(SelectEnterEventArgs args)
	{
		grabbedInteractable.interactable = args.interactableObject;
		grabbedInteractable.physicsData = args.interactableObject.transform.GetComponent<InteractablePhysicsData>();

		if (grabbedInteractable.physicsData != null)
		{
			if (grabbedInteractable.physicsData.CurrentlyInteractingHand != null)
			{
				grabbedInteractable.physicsData.QueuedHand = this;
				return;
			}

			grabbedInteractable.physicsData.CurrentlyInteractingHand = this;

			foreach (var collider in interactorColliders)
			{
				collider.gameObject.layer = interactableLayerIndex;
			}
			interactorColliders.Clear();

			if (!grabbedInteractable.physicsData.DoPhysicsInteractions)
			{
				var colliders = grabbedInteractable.physicsData.AllColliders;

				foreach (var collider in colliders)
				{
					if (collider == null)
						continue;

					collider.gameObject.layer = grabLayerIndex;
					interactorColliders.Add(collider);
				}
			}
		}
		grabState = GrabState.StartingGrab;
	}

	void OnDropObject(SelectExitEventArgs args)
	{
		var data = args.interactableObject.transform.GetComponent<InteractablePhysicsData>();
		if (data)
		{
			if (data.QueuedHand == this)
			{
				return;
			}
			else if (data.CurrentlyInteractingHand == this && data.QueuedHand != null)
			{
				data.CurrentlyInteractingHand = null;
				SelectEnterEventArgs selectEnterEventArgs = new SelectEnterEventArgs()
				{
					interactableObject = grabbedInteractable.interactable
				};
				data.QueuedHand.OnGrabObject(selectEnterEventArgs);
				data.QueuedHand = null;
				interactorColliders.Clear();
			}
		}

		data.CurrentlyInteractingHand = null;
		grabState = GrabState.NotGrabbing;

		grabbedInteractable.physicsData = null;
		grabbedInteractable.interactable = null;

		PhysicsHand.SetWorldForce(true, true, true);
		PhysicsHand.SetPalmOverride(null);
		overridePose = 0;
		FindFingersTarget();
	
		if (handJoint != null)
			handJoint.connectedBody = null;
	}

	bool ShouldMoveToInteractable()
	{
		if (grabbedInteractable.interactable != null && grabbedInteractable.physicsData != null)
		{
			var interactable = grabbedInteractable.interactable.GetAttachTransform(interactor);
			var interactableTarget = interactor.GetAttachTransform(grabbedInteractable.interactable);

			return grabbedInteractable.physicsData.RestrictedMovement || Vector3.SqrMagnitude(interactableTarget.position - interactable.position) < handInfo.MoveToInteractableDistance * grabbedInteractable.physicsData.MoveToDistanceModifier;
		}

		return false;
	}

	void MoveToInteractable()
	{
		overridePose = handInfo.FindPoseIndex(grabbedInteractable.physicsData.HandGrabPose);
		FindFingersTarget();
	
			if (isLeftHand)
			PhysicsHand.SetPalmOverride(grabbedInteractable.physicsData.PhysicsLeftHandAttachPoint);
		else
			PhysicsHand.SetPalmOverride(grabbedInteractable.physicsData.PhysicsRightHandAttachPoint);

		PhysicsHand.SetWorldForce(true, false, true);
		grabState = GrabState.MovingToInteractable;
	}

	bool ShouldAttachPhysicsToHand()
	{
		if (grabbedInteractable.interactable != null)
		{
			var attachPoint = interactor.GetAttachTransform(null);
			var target = PhysicsHand.GetPalmTarget();

			return Vector3.SqrMagnitude(attachPoint.position - target.position) < handInfo.InteractableAttachDistance
				&& Quaternion.Angle(attachPoint.rotation, target.rotation) < handInfo.InteractableAttachAngle;
		}
		return false;
	}

	void AttachToInteractable()
	{
		if (grabbedInteractable.physicsData)
		{
			handJoint = PhysicsHand.gameObject.AddComponent<FixedJoint>();
			handJoint.enablePreprocessing = false;
			handJoint.connectedBody = grabbedInteractable.physicsData.InteractableBody;
		}

		grabState = GrabState.Grabbed;
	}


	#region HandDisconnect
	private void OnTrackingState(InputAction.CallbackContext ctx)
	{
		int currentState = ctx.ReadValue<int>();

		//Debug.Log(gameObject.name + " current: " + currentState + ", prev: " + lastTrackingState);
		if (currentState > lastTrackingState)
		{
			//DOES NOT WORK
			var target = PhysicsHand.TargetHandModel;
			PhysicsHand.Teleport(target.position, target.rotation);
		}
		lastTrackingState = currentState;
	}
	#endregion

	#region Teleport
	void OnTeleportDirection(InputAction.CallbackContext ctx)
	{
		teleportDirection = ctx.ReadValue<Vector2>();
	}

	private void OnTeleportActivate(InputAction.CallbackContext ctx)
	{
		EnableTeleporting(true);
	}

	private void OnTeleportCancel(InputAction.CallbackContext ctx)
	{
		EnableTeleporting(false);
	}

	public void EnableTeleporting(bool value)
	{
		if (teleporter == null || !CanTeleport)
		{
			isTeleporting = false;
			return;
		}
		if (value == teleporter.enabled)
			return;

		interactor.enabled = !value;
		Controller.enableInputActions = !value;
		teleporter.enabled = value;
		teleporterRenderer.enabled = value;
		isTeleporting = value;
		FindFingersTarget();
	}
	#endregion

	#region Animation
	void OnTriggerPress(InputAction.CallbackContext ctx)
	{
		if (controllerVisible)
			ControllerAnimator.SetFloat(triggerID, ctx.ReadValue<float>());

		triggerPercent = ctx.ReadValue<float>();
		FindFingersTarget();
	}
	void OnGripPress(InputAction.CallbackContext ctx)
	{
		if (controllerVisible)
			ControllerAnimator.SetFloat(gripID, ctx.ReadValue<float>());

		gripPercent = ctx.ReadValue<float>();
		FindFingersTarget();
	}
	void OnJoystick(InputAction.CallbackContext ctx)
	{
		ControllerAnimator.SetFloat(joystickXID, ctx.ReadValue<Vector2>().x);
		ControllerAnimator.SetFloat(joystickYID, ctx.ReadValue<Vector2>().y);
	}
	void OnButton1(InputAction.CallbackContext ctx)
	{
		ControllerAnimator.SetFloat(button1ID, ctx.performed ? 1 : 0);
	}
	void OnButton2(InputAction.CallbackContext ctx)
	{
		ControllerAnimator.SetFloat(button2ID, ctx.performed ? 1 : 0);
	}

	void OnButton1Touched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			thumbButtonsTouched++;
		else
			thumbButtonsTouched--;
		FindFingersTarget();
	}
	void OnButton2Touched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			thumbButtonsTouched++;
		else
			thumbButtonsTouched--;
		FindFingersTarget();
	}
	void OnJoystickTouched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			thumbButtonsTouched++;
		else
			thumbButtonsTouched--;
		FindFingersTarget();
	}
	void OnTriggerTouched(InputAction.CallbackContext ctx)
	{
		FindFingersTarget();
	}
	void OnGripTouched(InputAction.CallbackContext ctx)
	{ 
		FindFingersTarget();
	}


	void SetControllerVisible(bool value)
	{
		if (value == controllerVisible)
			return;

		controllerVisible = value;
		ControllerRenderer.enabled = value;
		ControllerAnimator.enabled = value;

		if (value)
		{
			controllerMaterial.SetFloat(opacityId, 0);

			joystick.action.performed += OnJoystick;

			button1.action.performed += OnButton1;
			button1.action.canceled += OnButton1;

			button2.action.performed += OnButton2;
			button2.action.canceled += OnButton2;
		}
		else
		{
			joystick.action.performed -= OnJoystick;

			button1.action.performed -= OnButton1;
			button1.action.canceled -= OnButton1;

			button2.action.performed -= OnButton2;
			button2.action.canceled -= OnButton2;
		}
	}

	private void FindFingersTarget()
	{
		bool thumbPressed = thumbButtonsTouched > 0;
		bool gripPressed = gripPercent > handInfo.InputThreshold;
		bool triggerPressed = triggerPercent > handInfo.InputThreshold;
		
		bool triggerDown = triggerPercent > 0.5f;
		bool gripDown = gripPercent> 0.5f;

		//will be set to 1 if needed
		poseAmountTarget = 0;

		if (isTeleporting)
		{
			indexTarget = -0.1f;
			thumbTarget = -0.3f;
			middleTarget = 1;
			ringTarget = 1;
			pinkyTarget = 1;

			poseAmountTarget = 1;
			currentPoseIndex = fingerGunIndex;
		}
		else if (gripPressed)
		{
			if (triggerPressed)
			{
				float pointerValue = triggerPercent;
				indexTarget = pointerValue;

				if (thumbPressed)
				{
					//if everything is pressed, do fist
					thumbTarget = gripPercent;
				}
				else
				{
					//if everything but thumb is pressed dow thumbs up
					if (triggerDown)
						thumbTarget = -0.3f;
					else
						thumbTarget = -0.1f;
				}

			}
			else
			{
				if (gripDown)
				{
					//if grip is down, trigger is not down, and thumb is down, point finger
					if (thumbPressed)
					{
						thumbTarget = gripPercent;
						indexTarget = -0.3f;
					}
					//if grip is down, trigger is not down, and thumb is not down, do finger guns
					else
					{
						indexTarget = -0.1f;
						thumbTarget = -0.3f;

						poseAmountTarget = 1;
						currentPoseIndex = fingerGunIndex;
					}
				}
				else
				{
					indexTarget = 0;
					//if nothing is down except thumbs, just move thumb down based on grip amount
					if (thumbPressed)
					{
						thumbTarget = gripPercent;
					}
					//otherwise put thumbs in default position
					else
						thumbTarget = -0.1f;
				}


			}
			//move fingers based on grip amount
			middleTarget = gripPercent;
			ringTarget = gripPercent;
			pinkyTarget = gripPercent;
		}
		else if (triggerPressed)
		{
			//if not gripping, trigger somewhat down, and thumb down, do OK sign
			if (triggerPercent > 0.2f && thumbPressed)
			{
				currentPoseIndex = okIndex;
				poseAmountTarget = 1;
			}
			else
			{
				if (thumbPressed)
					thumbTarget = 0.0f;
				else
					thumbTarget = -0.1f;
			}

			//if not gripping but trigger is down, close index finger
			float pointerValue = 0.2f + (triggerPercent * 0.4f);
			indexTarget = pointerValue;
			middleTarget = pointerValue / 3;
			ringTarget = pointerValue / 4;
			pinkyTarget = pointerValue / 5;

			
		}
		else
		{
			//just move everything to default
			indexTarget = 0;
			middleTarget = 0;
			ringTarget = 0;
			pinkyTarget = 0;

			//except thumbs o course
			if (thumbPressed)
				thumbTarget = 0.0f;
			else
				thumbTarget = -0.1f;
		}

		//if pose is overriden just ignore all of this lol
		if (overridePose != 0)
		{
			currentPoseIndex = overridePose;
			poseAmountTarget = 1;
		}
	}

	private void UpdateFingers()
	{
		float currentSpeed = Time.deltaTime * handInfo.FingerSpeed;

		float thumbDiff = Mathf.Abs(thumbCurrent - thumbTarget); 
		float indexDiff = Mathf.Abs(indexCurrent - indexTarget); 
		float middleDiff = Mathf.Abs(middleCurrent - middleTarget);
		float ringDiff = Mathf.Abs(ringCurrent - ringTarget);
		float pinkyDiff = Mathf.Abs(pinkyCurrent - pinkyTarget); 
		

		//if close enough to correct do not set animator values because slow
		//this has not been tested for performance so it is kinda stupid
		if (thumbDiff +
			indexDiff +
			middleDiff +
			ringDiff +
			pinkyDiff > 0.02f)
		{
			//move current to target values
			thumbCurrent = Mathf.MoveTowards(thumbCurrent, thumbTarget, thumbDiff * Time.deltaTime * handInfo.ThumbSpeed);
			indexCurrent = Mathf.MoveTowards(indexCurrent, indexTarget, indexDiff * currentSpeed);
			middleCurrent = Mathf.MoveTowards(middleCurrent, middleTarget, middleDiff * currentSpeed);
			ringCurrent = Mathf.MoveTowards(ringCurrent, ringTarget, ringDiff * currentSpeed);
			pinkyCurrent = Mathf.MoveTowards(pinkyCurrent, pinkyTarget, pinkyDiff * currentSpeed);

			//moves hand values to current values
			HandAnimator.SetFloat(pinkyID, pinkyCurrent);
			HandAnimator.SetFloat(ringID, ringCurrent);
			HandAnimator.SetFloat(middleID, middleCurrent);
			HandAnimator.SetFloat(indexID, indexCurrent);
			HandAnimator.SetFloat(thumbID, thumbCurrent);
		}

		if (currentPoseIndex != lastPoseIndex)
		{
			poseSpeedModifier = handInfo.FindPoseSpeedModifier(currentPoseIndex);
			lastPoseIndex = currentPoseIndex;

			if (handInfo.IsFullHandPose(currentPoseIndex))
			{
				if (handInfo.FullHandPoseLayer == otherPoseLayer)
				{
					float amt = poseAmountCurrent;
					poseAmountCurrent = otherPoseAmountCurrent;
					otherPoseAmountCurrent = amt;
				}
				poseLayer = handInfo.FullHandPoseLayer;
				otherPoseLayer = handInfo.PartHandPoseLayer;
				HandAnimator.SetInteger(fullHandPoseIndexID, currentPoseIndex);
			}
			else
			{
				if (handInfo.PartHandPoseLayer == otherPoseLayer)
				{
					float amt = poseAmountCurrent;
					poseAmountCurrent = otherPoseAmountCurrent;
					otherPoseAmountCurrent = amt;
				}
				poseLayer = handInfo.PartHandPoseLayer;
				otherPoseLayer = handInfo.FullHandPoseLayer;
				HandAnimator.SetInteger(partHandPoseIndexID, currentPoseIndex);
			}
		}

		float poseDiff = Mathf.Abs(poseAmountCurrent - poseAmountTarget);
		poseAmountCurrent = Mathf.MoveTowards(poseAmountCurrent, poseAmountTarget, poseDiff * Time.deltaTime * handInfo.PoseSpeed * poseSpeedModifier);
		otherPoseAmountCurrent = Mathf.MoveTowards(otherPoseAmountCurrent, 0, otherPoseAmountCurrent * Time.deltaTime * handInfo.PoseSpeed * poseSpeedModifier);

		HandAnimator.SetLayerWeight(poseLayer, poseAmountCurrent);
		HandAnimator.SetLayerWeight(otherPoseLayer, otherPoseAmountCurrent);
	}

	void UpdateControllerVisibility()
	{
		float distanceToTargetSq = (PhysicsHand.transform.position - transform.position).sqrMagnitude;

		if (distanceToTargetSq > handInfo.ControllerVisibleStart * handInfo.ControllerVisibleStart)
		{
			SetControllerVisible(true);
			distanceToTargetSq = Mathf.Sqrt(distanceToTargetSq);
			float opacity = fullOpacity * Mathf.Clamp01((distanceToTargetSq - handInfo.ControllerVisibleStart) / (handInfo.ControllerVisibleRange));
			controllerMaterial.SetFloat(opacityId, opacity);
		}
		else
		{
			SetControllerVisible(false);
		}
	}
	#endregion

	private void Update()
	{
		UpdateFingers();
		UpdateControllerVisibility();

		//teleporting
		if (!isTeleporting || thumbstick.action.triggered)
			return;
		if (!teleporter.TryGetCurrent3DRaycastHit(out RaycastHit hit))
		{
			EnableTeleporting(false);
			return;
		}
		if ((1 << hit.collider.gameObject.layer & PlayerController.TeleportSurfaceMask.value) == 0)
		{
			EnableTeleporting(false);
			return;
		}

		PlayerController.Locomotor.QueueTeleportRequest(new TeleportRequest()
		{ destinationPosition = hit.point });
		EnableTeleporting(false);
	}

	private void FixedUpdate()
	{
		switch (grabState)
		{
			case GrabState.StartingGrab:
				if (ShouldMoveToInteractable())
				{
					MoveToInteractable();
				}
				break;
			case GrabState.MovingToInteractable:
				if (ShouldAttachPhysicsToHand())
				{
					AttachToInteractable();
				}
				break;
			case GrabState.NotGrabbing:

				int len = interactorColliders.Count;
				for (int i = 0; i < len; i++)
				{
					if (!physicsRenderer.bounds.Intersects(interactorColliders[i].bounds))
					{
						if (interactorColliders[i].gameObject.layer == grabLayerIndex)
							interactorColliders[i].gameObject.layer = interactableLayerIndex;

						interactorColliders.RemoveAt(i);
						i--;
						len--;
					}
				}
				break;
			default:
				break;
		}
	}
}