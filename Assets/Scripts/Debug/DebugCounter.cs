using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugCounter : MonoBehaviour
{
	[SerializeField]
	float fpsInterval = 1.0f;

	TextMeshPro text;
	float fps = 0;

	long accumulator = 0;

	private void Start()
	{
		text = GetComponent<TextMeshPro>();
		if (text == null)
			enabled = false;
		else
			text.text = "fps: 0.00";
	}

	private void Update()
	{
		fps += Time.unscaledDeltaTime;
		accumulator++;

		if (fps >= fpsInterval)
		{
			float avFPS = accumulator / fps;
			text.text = System.String.Format("fps: {0:0.##}", avFPS);

			fps = 0;
			accumulator = 0;
		}

	}
}
