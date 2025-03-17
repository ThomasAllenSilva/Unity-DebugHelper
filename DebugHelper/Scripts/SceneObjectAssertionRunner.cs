//=============================================================================
// Copyright (c) 2025, Thomas Allen Silva 
// Licensed under the MIT License. See LICENSE file for full license details.
//=============================================================================

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Helper component that is required to assert scene objects. It also provides scene asserts that you can activate, which are specific to the scene.
/// </summary>
[DefaultExecutionOrder(-999999999)]
internal sealed class SceneObjectAssertionRunner : MonoBehaviour
{
    [Header("Runtime Settings")]
    [Tooltip("Determines whether the same object can be asserted more than once")]
    [SerializeField] private bool _assertRepeatedObjects = true;

    [field: Tooltip("Determines whether to rerun the objects assertion every time the hierarchy changes (EDITOR ONLY).")]
    [field: SerializeField] public bool AssertObjectsOnHierarchyChanged { get; private set; } = false;

    [Header("Scene Asserts")]
    [Space(5), SerializeField] private bool _hasMainCamera = false;
    [Space(5), SerializeField] private bool _audioListenerPresent = false;
    [Space(5), SerializeField] private bool _eventSystemExists = false;
    [Space(5), SerializeField] private bool _directionalLightAvailable = false;

    private readonly Dictionary<MonoBehaviour, IAssertableObject> _components = new Dictionary<MonoBehaviour, IAssertableObject>();

    private void Awake()
    {
        _components.Clear();
        _components.TrimExcess();

        AssertCurrentScene();
    }

    private void Start()
    {
        AssertCurrentScene();

#if DEBUG_HELPER == false
        Destroy(gameObject);
#endif
    }

    [Conditional(DebugHelper.CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AssertCurrentScene()
    {
        RunDefaultAsserts();

        MonoBehaviour[] monoBehaviours = DebugHelper.GetObjectsOfType<MonoBehaviour>();

        foreach (MonoBehaviour monoBehaviour in monoBehaviours)
        {
            if (monoBehaviour is IAssertableObject assertableObject)
            {
                if (Application.isPlaying)
                {
                    if (_components.ContainsKey(monoBehaviour) && _assertRepeatedObjects == false)
                    {
                        continue;
                    }

                    _components.TryAdd(monoBehaviour, assertableObject);
                }

                assertableObject.AssertObject(gameObject);
            }
        }
    }

    private void RunDefaultAsserts()
    {
        if (_hasMainCamera)
        {
            DebugHelper.Assert_MainCameraExists(gameObject, "Main camera not found! Ensure there's a camera tagged 'MainCamera' in the scene.");
        }

        if (_audioListenerPresent)
        {
            DebugHelper.Assert_ObjectIsLoaded<AudioListener>(typeof(AudioListener), gameObject, "AudioListener is missing. This component is needed in order to hear audio from the scene.");
        }

        if (_eventSystemExists)
        {
            DebugHelper.Assert_ObjectIsLoaded<EventSystem>(typeof(EventSystem), gameObject, "UI EventSystem is missing! UI elements will not respond to player input.");
        }

        if (_directionalLightAvailable)
        {
            DebugHelper.Assert_DirectionalLightExists(gameObject, "No directional light detected! Please add one.");
        }
    }

    private void OnValidate()
    {
#if DEBUG_HELPER 
        gameObject.hideFlags = HideFlags.None;
#else
        gameObject.hideFlags = HideFlags.NotEditable;
#endif
    }
}
