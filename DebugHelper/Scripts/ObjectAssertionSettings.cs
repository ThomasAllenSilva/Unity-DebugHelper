//=============================================================================
// Copyright (c) 2025, Thomas Allen Silva 
// Licensed under the MIT License. See LICENSE file for full license details.
//=============================================================================

using UnityEngine;

/// <summary>
/// Settings that are used to assert project assets
/// </summary>
[CreateAssetMenu(fileName = nameof(ObjectAssertionSettings), menuName = "DebugHelper/ObjectAssertionSettings")]
internal sealed class ObjectAssertionSettings : ScriptableObject
{
    [field: SerializeField] public bool AssertPrefabs { get; private set; } = true;

    [field: Space(5), SerializeField] public bool AssertScriptableObjects { get; private set; } = true;

    [field: Header("Folder paths to search for ScriptableObjects and Prefabs within the project. \nEnsure you add the specific folders where your assets are located, as broader searches may be slower.")]
    [field: SerializeField] public string[] Paths { get; private set; }
}
