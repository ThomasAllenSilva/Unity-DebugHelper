//=============================================================================
// Copyright (c) 2025, Thomas Allen Silva 
// Licensed under the MIT License. See LICENSE file for full license details.
//=============================================================================

#if DEBUG_HELPER && UNITY_EDITOR
using System;

using UnityEditor;

using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Used to run assertions (Scene and project wide)
/// </summary>
[InitializeOnLoad]
internal sealed class ObjectAssertionEditorValidator
{
    private static ObjectAssertionSettings _assertionSettings;

    private const string SETTINGS_PATH = "Assets/Plugins/Thomas/DebugHelper/Settings/ObjectAssertionSettings.asset";

    static ObjectAssertionEditorValidator()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.hierarchyChanged -= OnHierarchyChanged;
        EditorApplication.hierarchyChanged += OnHierarchyChanged;
    }

    private static void OnHierarchyChanged()
    {
        if (Application.isPlaying)
        {
            ValidateCurrentLoadedAssertionRunners_HierarchyChanged();
        }
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        _assertionSettings = AssetDatabase.LoadAssetAtPath<ObjectAssertionSettings>(SETTINGS_PATH);

        if (state == PlayModeStateChange.ExitingEditMode)
        {
            try
            {
                if (_assertionSettings == null)
                {
                    throw new Exception($"Failed to load Assertion Settings. Make it's located in the correct path: {SETTINGS_PATH}\n");
                }

                AssertProjectAssets();

                ValidateCurrentLoadedAssertionRunners();
            }

            catch (Exception e)
            {
                string dialogMessage = "One or more exceptions occurred while running assertion methods. If you continue playing, things might blow up unexpectedly. Do you want to proceed?";

                bool result = EditorUtility.DisplayDialog("Object Assertion Exception", dialogMessage, "Yes", "No");

                if (result == false)
                {
                    EditorApplication.isPlaying = false;

                    throw;
                }

                else
                {
                    DebugHelper.LogError(e.Message);
                }
            }
        }
    }

    private static void AssertProjectAssets()
    {
        // If no assertions are needed we just skip unnecessary asset loading.
        if (_assertionSettings.AssertScriptableObjects == false && _assertionSettings.AssertPrefabs == false)
        {
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Prefab t:ScriptableObject", _assertionSettings.Paths);

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);

            if (typeof(ScriptableObject).IsAssignableFrom(assetType) && _assertionSettings.AssertScriptableObjects == false)
            {
                continue;
            }

            else if (assetType == typeof(GameObject) && _assertionSettings.AssertPrefabs == false)
            {
                continue;
            }

            UnityEngine.Object asset = AssetDatabase.LoadMainAssetAtPath(assetPath);

            if (asset == null)
            {
                continue;
            }

            if (asset is GameObject prefab)
            {
                AssertPrefab(prefab, assetPath);
            }

            else if (asset is ScriptableObject scriptableObject)
            {
                AssertScriptableObject(scriptableObject, assetPath);
            }
        }
    }

    private static void AssertPrefab(GameObject prefab, string assetPath)
    {
        IAssertableObject[] assertions = prefab.GetComponentsInChildren<IAssertableObject>(true);

        foreach (IAssertableObject assertion in assertions)
        {
            try
            {
                assertion.AssertObject(prefab);
            }
            catch (Exception ex)
            {
                throw new Exception($"Prefab Assertion FAILED. '{prefab.name}' at path '{assetPath}'. \n\nMessage: {ex.Message}\n");
            }
        }
    }

    private static void AssertScriptableObject(ScriptableObject scriptableObject, string assetPath)
    {
        if (scriptableObject is IAssertableObject assertion)
        {
            try
            {
                assertion.AssertObject(scriptableObject);
            }
            catch (Exception ex)
            {
                throw new Exception($"ScriptableObject Assertion FAILED. '{scriptableObject.name}' at path '{assetPath}'. \n\nMessage: {ex.Message}\n");
            }
        }
    }

    private static void ValidateCurrentLoadedAssertionRunners()
    {
        foreach (SceneObjectAssertionRunner sceneDebugAssertionRunner in DebugHelper.GetObjectsOfType<SceneObjectAssertionRunner>())
        {
            sceneDebugAssertionRunner.AssertCurrentScene();
        }
    }

    private static void ValidateCurrentLoadedAssertionRunners_HierarchyChanged()
    {
        foreach (SceneObjectAssertionRunner sceneDebugAssertionRunner in DebugHelper.GetObjectsOfType<SceneObjectAssertionRunner>())
        {
            if (sceneDebugAssertionRunner.AssertObjectsOnHierarchyChanged)
            {
                sceneDebugAssertionRunner.AssertCurrentScene();
            }
        }
    }

    [MenuItem("GameObject/Debug/Scene Assertion Runner", false, 100)]
    private static void CreateDebugAssertionRunner()
    {
        Scene activeScene = SceneManager.GetActiveScene();

        SceneObjectAssertionRunner[] instances = DebugHelper.GetObjectsOfType<SceneObjectAssertionRunner>();

        foreach (SceneObjectAssertionRunner instance in instances)
        {
            if (instance.gameObject.scene == activeScene)
            {
                DebugHelper.LogWarning($"A {nameof(SceneObjectAssertionRunner)} already exists in the active scene.");

                return;
            }
        }

        GameObject gameObject = new GameObject(nameof(SceneObjectAssertionRunner), typeof(SceneObjectAssertionRunner));
        gameObject.name = $"{activeScene.name} Scene - Assertion Runner";
        gameObject.isStatic = true;

        //ignore raycast layer
        gameObject.layer = 2;
        gameObject.transform.parent = null;

        Undo.RegisterCreatedObjectUndo(gameObject, $"Create {gameObject}");

        Selection.activeGameObject = gameObject;
    }
}
#endif
