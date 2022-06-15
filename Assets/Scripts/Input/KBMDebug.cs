using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KBMDebug : MonoBehaviour
{
    [SerializeField] private float horizontalSensitivity = 1.0f;
    [SerializeField] private float verticalSensitivity = 1.0f;

    [SerializeField] private Camera camera = null;
    private bool canMoveCam = false;
    private Vector2 mouseDeltaPos = Vector2.zero;

    public void SetMoveCam(bool yes)
    {
        canMoveCam = yes;
        Cursor.lockState = yes ? CursorLockMode.Locked : CursorLockMode.None;
        if (yes)
            PlayerInput.OnGetMousePositionDelta += SetMouseDelta;
        else
            PlayerInput.OnGetMousePositionDelta -= SetMouseDelta;
    }

    public void SetMouseDelta(Vector2 delta)
    {
        mouseDeltaPos = delta;
    }

    private void Update()
    {
        if (canMoveCam)
            camera.transform.localRotation = Quaternion.Euler(camera.transform.localRotation.eulerAngles + new Vector3(horizontalSensitivity * -mouseDeltaPos.y, verticalSensitivity * mouseDeltaPos.x, 0));
    }
    
    private void OnEnable()
    {
        PlayerInput.OnMouseRightDownDo += SetMoveCam;
    }

    private void OnDisable()
    {
        PlayerInput.OnMouseRightDownDo -= SetMoveCam;
        
    }
}
