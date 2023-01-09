//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Scripts/Input/GameInput.inputactions
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

public partial class @GameInput : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInput"",
    ""maps"": [
        {
            ""name"": ""PlayerActions"",
            ""id"": ""422efb79-80b1-468f-a742-b22fc2ba0ec7"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""9e24b7ad-8d72-41c9-8a55-b907a9fefe9b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Abilities"",
                    ""type"": ""Button"",
                    ""id"": ""d8c9918e-b163-4297-9ce4-c443e0ec9055"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""bed7c00a-2fe9-47eb-a944-f4533615f15b"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6c8fbfbe-0c52-4f5a-9570-01540450d89f"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Abilities"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15cc1aae-11a4-4401-89e7-7d5076e27574"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Abilities"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""342effef-5c9d-4e98-8fc0-93ecd74cf5a1"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Abilities"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""CameraActions"",
            ""id"": ""69e8b57b-cdf7-4e5f-91db-d28f5d4ea137"",
            ""actions"": [
                {
                    ""name"": ""CameraLock"",
                    ""type"": ""Value"",
                    ""id"": ""32c2a592-4df0-42d1-951f-7fade405ce40"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""861dae71-d76b-4594-bb46-1f8e80087232"",
                    ""path"": ""<Keyboard>/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""CameraLock"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerActions
        m_PlayerActions = asset.FindActionMap("PlayerActions", throwIfNotFound: true);
        m_PlayerActions_Move = m_PlayerActions.FindAction("Move", throwIfNotFound: true);
        m_PlayerActions_Abilities = m_PlayerActions.FindAction("Abilities", throwIfNotFound: true);
        // CameraActions
        m_CameraActions = asset.FindActionMap("CameraActions", throwIfNotFound: true);
        m_CameraActions_CameraLock = m_CameraActions.FindAction("CameraLock", throwIfNotFound: true);
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

    // PlayerActions
    private readonly InputActionMap m_PlayerActions;
    private IPlayerActionsActions m_PlayerActionsActionsCallbackInterface;
    private readonly InputAction m_PlayerActions_Move;
    private readonly InputAction m_PlayerActions_Abilities;
    public struct PlayerActionsActions
    {
        private @GameInput m_Wrapper;
        public PlayerActionsActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerActions_Move;
        public InputAction @Abilities => m_Wrapper.m_PlayerActions_Abilities;
        public InputActionMap Get() { return m_Wrapper.m_PlayerActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActionsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActionsActions instance)
        {
            if (m_Wrapper.m_PlayerActionsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnMove;
                @Abilities.started -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnAbilities;
                @Abilities.performed -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnAbilities;
                @Abilities.canceled -= m_Wrapper.m_PlayerActionsActionsCallbackInterface.OnAbilities;
            }
            m_Wrapper.m_PlayerActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Abilities.started += instance.OnAbilities;
                @Abilities.performed += instance.OnAbilities;
                @Abilities.canceled += instance.OnAbilities;
            }
        }
    }
    public PlayerActionsActions @PlayerActions => new PlayerActionsActions(this);

    // CameraActions
    private readonly InputActionMap m_CameraActions;
    private ICameraActionsActions m_CameraActionsActionsCallbackInterface;
    private readonly InputAction m_CameraActions_CameraLock;
    public struct CameraActionsActions
    {
        private @GameInput m_Wrapper;
        public CameraActionsActions(@GameInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @CameraLock => m_Wrapper.m_CameraActions_CameraLock;
        public InputActionMap Get() { return m_Wrapper.m_CameraActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActionsActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActionsActions instance)
        {
            if (m_Wrapper.m_CameraActionsActionsCallbackInterface != null)
            {
                @CameraLock.started -= m_Wrapper.m_CameraActionsActionsCallbackInterface.OnCameraLock;
                @CameraLock.performed -= m_Wrapper.m_CameraActionsActionsCallbackInterface.OnCameraLock;
                @CameraLock.canceled -= m_Wrapper.m_CameraActionsActionsCallbackInterface.OnCameraLock;
            }
            m_Wrapper.m_CameraActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CameraLock.started += instance.OnCameraLock;
                @CameraLock.performed += instance.OnCameraLock;
                @CameraLock.canceled += instance.OnCameraLock;
            }
        }
    }
    public CameraActionsActions @CameraActions => new CameraActionsActions(this);
    public interface IPlayerActionsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAbilities(InputAction.CallbackContext context);
    }
    public interface ICameraActionsActions
    {
        void OnCameraLock(InputAction.CallbackContext context);
    }
}
