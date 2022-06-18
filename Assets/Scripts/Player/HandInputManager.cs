using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

//handles input for a single hand
public class HandInputManager : MonoBehaviour
{
	[Header("Values")]
	[SerializeField] float fingerSpeed = 1;
	[SerializeField, Range(0.0f, 1.0f)] float threshold = 0.03f;

	[Header("References")]
	[SerializeField] Animator controllerAnimator;
	[SerializeField] Animator handAnimator;

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

	//input values
	float gripPercent;
	float triggerPercent;
	int buttonsTouched = 0;

	float thumbTarget = 0;
	float indexTarget = 0;
	float middleTarget = 0;
	float ringTarget = 0;
	float pinkyTarget = 0;

	float thumbCurrent = 0;
	float indexCurrent = 0;
	float middleCurrent = 0;
	float ringCurrent = 0;
	float pinkyCurrent = 0;

	bool ControllerVisible { get => controllerVisible; set { SetControllerVisible(value); } }
	bool controllerVisible = false;

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
		Debug.Log("buttons touched: " + buttonsTouched);
	}
	void OnButton2Touched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			buttonsTouched++;
		else
			buttonsTouched--;
		FindFingersTarget();
		Debug.Log("buttons touched: " + buttonsTouched);
	}
	void OnJoystickTouched(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			buttonsTouched++;
		else
			buttonsTouched--;
		FindFingersTarget();
		Debug.Log("buttons touched: " + buttonsTouched);
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

		if (value)
		{
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

		if (gripPercent > threshold)
		{
			if (triggerPercent > threshold)
			{
				float pointerValue = triggerPercent;
				indexTarget = pointerValue;

				if (buttonsTouched > 0)
				{
					if (gripPercent > 0.5f)
						thumbTarget = gripPercent;
					else
						thumbTarget = triggerPercent;
				}
				else
				{
					if (triggerPercent > 0.5f)
						thumbTarget = -0.3f;
					else
						thumbTarget = -0.1f;
				}
					
			}
			else
			{
				if (gripPercent > 0.5f)
				{
					if (buttonsTouched > 0)
						indexTarget = -0.3f;
					else
						indexTarget = -0.1f;
				}
				else
					indexTarget = 0;

				if (buttonsTouched > 0)
				{
					thumbTarget = gripPercent;
				}
				else
					thumbTarget = -0.1f;
			}	

			middleTarget = gripPercent;
			ringTarget = gripPercent;
			pinkyTarget = gripPercent;
		}
		else if (triggerPercent > threshold)
		{
			float pointerValue = 0.2f + (triggerPercent * 0.4f);
			indexTarget = pointerValue;
			middleTarget = pointerValue / 3;
			ringTarget = pointerValue / 4;
			pinkyTarget = pointerValue / 5;
		}
		else
		{
			indexTarget = 0;
			middleTarget = 0;
			ringTarget = 0;
			pinkyTarget = 0;

			if (buttonsTouched > 0)
				thumbTarget = 0.0f;
			else
				thumbTarget = -0.1f;
		}
	}

	private void UpdateFingers()
	{
		float currentSpeed = Time.deltaTime * fingerSpeed;
		//move current to target values
		thumbCurrent = Mathf.MoveTowards(thumbCurrent, thumbTarget, Mathf.Abs(thumbCurrent - thumbTarget) * currentSpeed);
		indexCurrent =	Mathf.MoveTowards(indexCurrent, indexTarget	, Mathf.Abs(indexCurrent - indexTarget) * currentSpeed);
		middleCurrent =	Mathf.MoveTowards(middleCurrent, middleTarget	, Mathf.Abs(middleCurrent - middleTarget) * currentSpeed);
		ringCurrent =	Mathf.MoveTowards(ringCurrent, ringTarget	, Mathf.Abs(ringCurrent - ringTarget) * currentSpeed);
		pinkyCurrent =	Mathf.MoveTowards(pinkyCurrent, pinkyTarget	, Mathf.Abs(pinkyCurrent - pinkyTarget) * currentSpeed);

		//moves hand values to current values
		handAnimator.SetFloat(pinkyID, pinkyCurrent);
		handAnimator.SetFloat(ringID, ringCurrent);
		handAnimator.SetFloat(middleID, middleCurrent);
		handAnimator.SetFloat(indexID, indexCurrent);
		handAnimator.SetFloat(thumbID, thumbCurrent);
	}

	private void Update()
	{
		UpdateFingers();
	}
}
