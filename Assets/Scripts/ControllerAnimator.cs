using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerAnimator : MonoBehaviour
{
	[SerializeField] Animator animator;
	[SerializeField] ActionBasedController controller;

	private void Start()
	{
		Debug.Log("hello!");
		int trigger = Animator.StringToHash("Trigger");
		//controller.activateActionValue.action.performed += ctx => animator.SetFloat(trigger, 0.5f);
		controller.activateActionValue.action.performed += ctx => animator.SetFloat(trigger, ctx.ReadValue<float>());
	}
}
