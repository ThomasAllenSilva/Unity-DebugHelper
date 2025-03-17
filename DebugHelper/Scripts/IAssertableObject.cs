//=============================================================================
// Copyright (c) 2025, Thomas Allen Silva 
// Licensed under the MIT License. See LICENSE file for full license details.
//=============================================================================

/// <summary>
/// Must be implemented by any object that you would like to be asserted
/// </summary>
public interface IAssertableObject 
{
    /// <summary> Asserts that the object is correctly configured. Should throw an exception if it's not </summary>
    void AssertObject(UnityEngine.Object contextObject);
}
