using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using UnityEngine.Animations;

//handles input for a single hand
public class HandManager : MonoBehaviour
{
	[Header("Values")]
	[SerializeField] HandInfo handInfo;

	[Header("References")]
	[SerializeField] Animator controllerAnimator;
	[SerializeField] Animator handAnimator;
	[SerializeField] SkinnedMeshRenderer controllerRenderer;
	[SerializeField] Transform target;

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
	int poseIndexID;

	//input values
	float gripPercent;
	float triggerPercent;
	int buttonsTouched = 0;

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
	float poseAmountCurrent = 0;
	int currentPoseIndex = 0;
	int lastPoseIndex = 0;

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
	
	private void Start()
	{
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
		poseIndexID = Animator.StringToHash("Pose Index");

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
		controllerRenderer.GetMaterials(materialList);
		controllerMaterial = materialList[0];
		fullOpacity = controllerMaterial.GetFloat(opacityId);
		SetControllerVisible(false);
	}

	void OnTriggerPress(InputAction.CallbackContext ctx)
	{
		if (controllerVisible)
			controllerAnimator.SetFloat(triggerID, ctx.ReadValue<float>());

		triggerPercent = ctx.ReadValue<float>();
		FindFingersTarget();
	}
	void OnGripPress(InputAction.CallbackContext ctx)
	{
		if (controllerVisible)
			controllerAnimator.SetFloat(gripID, ctx.ReadValue<float>());

		gripPercent = ctx.ReadValue<float>();
		FindFingersTarget();
	}
	void OnJoystick(InputAction.CallbackContext ctx)
	{
		controllerAnimator.SetFloat(joystickXID, ctx.ReadValue<Vector2>().x); 
		controllerAnimator.SetFloat(joystickYID, ctx.ReadValue<Vector2>().y);
	}
	void OnButton1(InputAction.CallbackContext ctx)
	{
		controllerAnimator.SetFloat(button1ID, ctx.performed ? 1 : 0);
	}
	void OnButton2(InputAction.CallbackContext ctx)
	{
		controllerAnimator.SetFloat(button2ID, ctx.performed ? 1 : 0);
	}

	void OnButton1Touched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			buttonsTouched++;
		else
			buttonsTouched--;
		FindFingersTarget();
	}
	void OnButton2Touched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			buttonsTouched++;
		else
			buttonsTouched--;
		FindFingersTarget();
	}
	void OnJoystickTouched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			buttonsTouched++;
		else
			buttonsTouched--;
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
		controllerRenderer.enabled = value;
		controllerAnimator.enabled = value;

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
		bool thumbPressed = buttonsTouched > 0;
		bool gripPressed = gripPercent > handInfo.InputThreshold;
		bool triggerPressed = triggerPercent > handInfo.InputThreshold;
		
		bool triggerDown = triggerPercent > 0.5f;
		bool gripDown = gripPercent> 0.5f;

		//will be set to 1 if needed
		poseAmountTarget = 0;

		if (gripPressed)
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
			handAnimator.SetFloat(pinkyID, pinkyCurrent);
			handAnimator.SetFloat(ringID, ringCurrent);
			handAnimator.SetFloat(middleID, middleCurrent);
			handAnimator.SetFloat(indexID, indexCurrent);
			handAnimator.SetFloat(thumbID, thumbCurrent);
		}

		float poseDiff = Mathf.Abs(poseAmountCurrent - poseAmountTarget);
		poseAmountCurrent = Mathf.MoveTowards(poseAmountCurrent, poseAmountTarget, poseDiff * Time.deltaTime * handInfo.PoseSpeed * poseSpeedModifier);
		handAnimator.SetLayerWeight(handInfo.PoseLayer, poseAmountCurrent);
		if (currentPoseIndex != lastPoseIndex)
		{
			poseSpeedModifier = handInfo.FindPoseSpeedModifier(currentPoseIndex);
			lastPoseIndex = currentPoseIndex;
			handAnimator.SetInteger(poseIndexID, currentPoseIndex);
		}
	}

	void UpdateControllerVisibility()
	{
		float distanceToTargetSq = (target.position - transform.position).sqrMagnitude;

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
	
	private void Update()
	{
		UpdateFingers();
		UpdateControllerVisibility();
	}
}