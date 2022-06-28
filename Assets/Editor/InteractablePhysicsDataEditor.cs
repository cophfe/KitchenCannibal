using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(InteractablePhysicsData))]
[CanEditMultipleObjects]
public class InteractablePhysicsDataEditor : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (targets.Length == 1)
		{
			var physData = target as InteractablePhysicsData;
			var preview = physData.GetComponent<InteractableHandPreview>();

			if (preview)
			{
				if (GUILayout.Button("Remove Hand Preview"))
				{
					Undo.DestroyObjectImmediate(preview);
				}
			}
			else if(GUILayout.Button("Add Hand Preview"))
			{
				preview = physData.gameObject.AddComponent<InteractableHandPreview>();
				Undo.RegisterCreatedObjectUndo(preview, "Undo Create Preview");
			}

		}
	}
}
