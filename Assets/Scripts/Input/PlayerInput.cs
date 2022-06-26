using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Make scroll wheel affect distance an object is away
// Make E be interact/drop button
// Make "Grab" function with objects

public class PlayerInput : MonoBehaviour
{
    private PlayerControlMap inputMap = null;

    public delegate void MouseLeftDown(bool down);
    public static event MouseLeftDown OnMouseLeftDownDo;

    public delegate void MouseRightDown(bool down);
    public static event MouseRightDown OnMouseRightDownDo;

    public delegate void MousePositionDelta(Vector2 mousePositionDelta);
    public static event MousePositionDelta OnGetMousePositionDelta;

    private void Awake()
    {
        inputMap = new PlayerControlMap();
        
    }
    private void Start()
    {
        inputMap.DefaultControls.LeftMouseButton.started += ctx => OnMouseLeftDownDo?.Invoke(true);
        inputMap.DefaultControls.LeftMouseButton.canceled += ctx => OnMouseLeftDownDo?.Invoke(false);

        inputMap.DefaultControls.RightMouseButton.started += ctx => OnMouseRightDownDo?.Invoke(true);
        inputMap.DefaultControls.RightMouseButton.canceled += ctx => OnMouseRightDownDo?.Invoke(false);
    }

    void OnleftClick(bool down)
    { 
        Debug.Log("Left Click: " + down);
    }

    private void Update()
    {
        OnGetMousePositionDelta?.Invoke(inputMap.DefaultControls.MousePosDelta.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        inputMap.Enable();
        OnMouseLeftDownDo += OnleftClick;
    }
    
    private void OnDisable()
    {
        inputMap.Disable();
        OnMouseLeftDownDo -= OnleftClick;
    }

}
