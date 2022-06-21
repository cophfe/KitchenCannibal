using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteAlways]
public class MirrorJoints : MonoBehaviour
{
	public Transform from;
	public Transform to;
	public bool mirror;

	private void Update()
	{
		if (mirror)
		{
			mirror = false;

			CloneJoints(from, to);
		}
	}

	void CloneJoints(Transform srcParent, Transform destParent)
	{
		var joint = srcParent.GetComponent<ConfigurableJoint>();
		var newJoint = destParent.GetComponent<ConfigurableJoint>();
		if (joint && newJoint)
		{
			newJoint.enableCollision = joint.enableCollision;
			newJoint.linearLimit = joint.linearLimit;
			newJoint.angularYLimit = joint.angularYLimit;
			newJoint.angularZLimit = joint.angularZLimit;
			newJoint.lowAngularXLimit = joint.lowAngularXLimit;
			newJoint.highAngularXLimit = joint.highAngularXLimit;
			
		}

		int count = srcParent.childCount;
		for (int i = 0; i < count; i++)
		{
			CloneJoints(srcParent.GetChild(i), destParent.GetChild(i));
		}
	}

}
