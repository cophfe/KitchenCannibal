using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ControllerAnimator : MonoBehaviour
{
	[SerializeField] Animator animator;

	[SerializeField] InputActionProperty grip;
	[SerializeField] InputActionProperty trigger;
	[SerializeField] InputActionProperty button1;
	[SerializeField] InputActionProperty button2;
	[SerializeField] InputActionProperty joystick;
	[SerializeField] InputActionProperty button3;
	private void Start()
	{
		//left primary secondary & joy work
		//right joy works
		//everything else doesnt work (button 3 unconfirmed)

		int triggerVal = Animator.StringToHash("Trigger");
		trigger.action.performed += ctx => animator.SetFloat(triggerVal, ctx.ReadValue<float>());
		int gripVal = Animator.StringToHash("Grip");
		grip.action.performed += ctx => animator.SetFloat(gripVal, ctx.ReadValue<float>());
		int joystickXVal = Animator.StringToHash("Joy X");
		int joystickYVal = Animator.StringToHash("Joy Y");
		joystick.action.performed += ctx => { animator.SetFloat(joystickXVal, ctx.ReadValue<Vector2>().x); animator.SetFloat(joystickYVal, ctx.ReadValue<Vector2>().y); };
		int button1Val = Animator.StringToHash("Button 1");
		button1.action.performed += ctx => animator.SetFloat(button1Val, 1);
		button1.action.canceled += ctx => animator.SetFloat(button1Val, 0);
		int button2Val = Animator.StringToHash("Button 2");
		button2.action.performed += ctx => animator.SetFloat(button2Val, 1);
		button2.action.canceled += ctx => animator.SetFloat(button2Val, 0);
		int button3Val = Animator.StringToHash("Button 3");
		button3.action.performed += ctx => animator.SetFloat(button3Val, 1);
		button3.action.canceled += ctx => animator.SetFloat(button3Val, 0);

	}
}
