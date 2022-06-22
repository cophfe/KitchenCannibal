//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Scripts/Input/PlayerControlMap.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControlMap : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControlMap()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControlMap"",
    ""maps"": [
        {
            ""name"": ""DefaultControls"",
            ""id"": ""2fb75018-1a36-408c-bbf0-d9cda56baf06"",
            ""actions"": [
                {
                    ""name"": ""LeftMouseButton"",
                    ""type"": ""Button"",
                    ""id"": ""14704795-a681-40d3-8f13-a1e8fc08307c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""RightMouseButton"",
                    ""type"": ""Button"",
                    ""id"": ""56d76572-4854-4646-bed9-91e9f40b594e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""e75a4711-f539-4286-85bb-efe6343aee66"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MousePosDelta"",
                    ""type"": ""Value"",
                    ""id"": ""507651f6-ff77-4a3c-8905-1fc87f3d8c73"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""MousePos"",
                    ""type"": ""Value"",
                    ""id"": ""79be43d7-2cec-4e01-9a3b-d7fe0c75ad7e"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c7e9cd5b-c0c2-4ba4-99ff-f96f7fcc2fb4"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftMouseButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4775e786-f54e-4f68-867d-2d5d7a682cc5"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightMouseButton"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""724d8126-bb43-4e9d-aabd-c3dcedb0581f"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a2914702-fa1d-48ed-bb34-5a1639db0273"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePosDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c2e7f35a-18b8-4971-9729-cb77215a5411"",
                    ""path"": ""<Mouse>/position"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MousePos"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // DefaultControls
        m_DefaultControls = asset.FindActionMap("DefaultControls", throwIfNotFound: true);
        m_DefaultControls_LeftMouseButton = m_DefaultControls.FindAction("LeftMouseButton", throwIfNotFound: true);
        m_DefaultControls_RightMouseButton = m_DefaultControls.FindAction("RightMouseButton", throwIfNotFound: true);
        m_DefaultControls_Interact = m_DefaultControls.FindAction("Interact", throwIfNotFound: true);
        m_DefaultControls_MousePosDelta = m_DefaultControls.FindAction("MousePosDelta", throwIfNotFound: true);
        m_DefaultControls_MousePos = m_DefaultControls.FindAction("MousePos", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // DefaultControls
    private readonly InputActionMap m_DefaultControls;
    private IDefaultControlsActions m_DefaultControlsActionsCallbackInterface;
    private readonly InputAction m_DefaultControls_LeftMouseButton;
    private readonly InputAction m_DefaultControls_RightMouseButton;
    private readonly InputAction m_DefaultControls_Interact;
    private readonly InputAction m_DefaultControls_MousePosDelta;
    private readonly InputAction m_DefaultControls_MousePos;
    public struct DefaultControlsActions
    {
        private @PlayerControlMap m_Wrapper;
        public DefaultControlsActions(@PlayerControlMap wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftMouseButton => m_Wrapper.m_DefaultControls_LeftMouseButton;
        public InputAction @RightMouseButton => m_Wrapper.m_DefaultControls_RightMouseButton;
        public InputAction @Interact => m_Wrapper.m_DefaultControls_Interact;
        public InputAction @MousePosDelta => m_Wrapper.m_DefaultControls_MousePosDelta;
        public InputAction @MousePos => m_Wrapper.m_DefaultControls_MousePos;
        public InputActionMap Get() { return m_Wrapper.m_DefaultControls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultControlsActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultControlsActions instance)
        {
            if (m_Wrapper.m_DefaultControlsActionsCallbackInterface != null)
            {
                @LeftMouseButton.started -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.performed -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnLeftMouseButton;
                @LeftMouseButton.canceled -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnLeftMouseButton;
                @RightMouseButton.started -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnRightMouseButton;
                @RightMouseButton.performed -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnRightMouseButton;
                @RightMouseButton.canceled -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnRightMouseButton;
                @Interact.started -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnInteract;
                @MousePosDelta.started -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePosDelta;
                @MousePosDelta.performed -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePosDelta;
                @MousePosDelta.canceled -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePosDelta;
                @MousePos.started -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePos;
                @MousePos.performed -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePos;
                @MousePos.canceled -= m_Wrapper.m_DefaultControlsActionsCallbackInterface.OnMousePos;
            }
            m_Wrapper.m_DefaultControlsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftMouseButton.started += instance.OnLeftMouseButton;
                @LeftMouseButton.performed += instance.OnLeftMouseButton;
                @LeftMouseButton.canceled += instance.OnLeftMouseButton;
                @RightMouseButton.started += instance.OnRightMouseButton;
                @RightMouseButton.performed += instance.OnRightMouseButton;
                @RightMouseButton.canceled += instance.OnRightMouseButton;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @MousePosDelta.started += instance.OnMousePosDelta;
                @MousePosDelta.performed += instance.OnMousePosDelta;
                @MousePosDelta.canceled += instance.OnMousePosDelta;
                @MousePos.started += instance.OnMousePos;
                @MousePos.performed += instance.OnMousePos;
                @MousePos.canceled += instance.OnMousePos;
            }
        }
    }
    public DefaultControlsActions @DefaultControls => new DefaultControlsActions(this);
    public interface IDefaultControlsActions
    {
        void OnLeftMouseButton(InputAction.CallbackContext context);
        void OnRightMouseButton(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnMousePosDelta(InputAction.CallbackContext context);
        void OnMousePos(InputAction.CallbackContext context);
    }
}