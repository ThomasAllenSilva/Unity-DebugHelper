//=============================================================================
// Copyright (c) 2025, Thomas Allen Silva.  
// Licensed under the MIT License. See LICENSE file for details.  
//=============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

using UnityEngine;
using UnityEngine.SceneManagement;

using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if UNITY_EDITOR == false
using System.IO;
#endif

/// <summary> Provides assertion and logging utilities for runtime debugging. The methods are conditionally compiled using the *Conditional Compilation Symbol* </summary>
public static class DebugHelper
{
    /// <summary> Compilation symbol. Must be defined in the project settings for these methods to be compiled </summary>
    public const string CONDITIONAL = "DEBUG_HELPER";

    private const string EXCEPTION_LOG_MESSAGE = "Exception occurred. See details below.";

    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogMessage(string message, Object contextObject = null, bool showMessageWindow = false)
    {
        Debug.Log($"{message}", contextObject);

        if (showMessageWindow)
        {
            DisplayMessageWindow(message);
        }
    }

    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogWarning(string message, Object contextObject = null)
    {
        Debug.LogWarning($"{message}", contextObject);
    }

    /// <summary> Logs an error message and optionally throws an exception </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogError(string message, Object contextObject = null, bool throwException = false)
    {
        if (throwException == true)
        {
            DisplayErrorWindow(message);

            if (contextObject != null)
            {
                Debug.LogError($"{EXCEPTION_LOG_MESSAGE} Click here to highlight the context object.\n", contextObject);
            }

            else
            {
                Debug.LogError($"{EXCEPTION_LOG_MESSAGE} No context object provided\n");
            }

#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Debug.Break();
            }
#endif

            throw new Exception($"{message}\n");
        }

        else
        {
            Debug.LogError($"{message}\n", contextObject);
        }
    }

    /// <summary> Logs all the items inside the collection, formatted as: 'Index: x, Value: y' </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogElements<T>(this IReadOnlyCollection<T> collection, string collectionName = "Collection")
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendLine($"{collectionName} Contents:");

        for (int i = 0; i < collection.Count; i++)
        {
            logMessage.AppendLine($"Index: {i}, Value: {collection.ElementAt(i)}");
        }

        LogMessage(logMessage.ToString());
    }

    /// <summary> Logs all the items inside the dictionary, formatted as: 'Key: x, Value: y' </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogElements<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, string dictionaryName = "Dictionary")
    {
        StringBuilder logMessage = new StringBuilder();

        logMessage.AppendLine($"{dictionaryName} Contents:");

        foreach (KeyValuePair<TKey, TValue> kvp in dictionary)
        {
            logMessage.AppendLine($"Key: {kvp.Key}, Value: {kvp.Value}");
        }

        LogMessage(logMessage.ToString());
    }

    /// <summary> Executes an assertion method only if the given condition is true </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_If(bool condition, Action assertionMethod)
    {
        if (condition == true)
        {
            assertionMethod();
        }
    }

    /// <summary> Asserts that a given condition is TRUE </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_That(bool condition, Object contextObject, string errorMessage)
    {
        Assert_IsTrue(condition, contextObject, errorMessage);
    }

    /// <summary> Requires a condition to be true before asserting the main condition </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_ThatIf(bool condition, bool requiredCondition, Object contextObject, string errorMessage)
    {
        if (requiredCondition == true)
        {
            Assert_IsTrue(condition, contextObject, errorMessage);
        }
    }

    /// <summary> Asserts that a given condition is TRUE </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsTrue(bool condition, Object contextObject, string errorMessage)
    {
        if (condition == false)
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that a given condition is FALSE </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsFalse(bool condition, Object contextObject, string errorMessage)
    {
        if (condition == true)
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the actual object's type can be treated as the expected type </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsAssignableFrom(Type expectedType, object actual, Object contextObject, string errorMessage)
    {
        bool isAssignable = actual.GetType().GetTypeInfo().IsAssignableFrom(expectedType.GetTypeInfo());

        Assert_IsTrue(isAssignable, contextObject, errorMessage);
    }

    /// <summary> Asserts that the string is not null or empty, and is not filled only with whitespace. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsNotNullOrEmpty(string aString, Object contextObject, string errorMessage)
    {
        bool isEmpty = string.IsNullOrEmpty(aString) || string.IsNullOrWhiteSpace(aString);

        Assert_IsFalse(isEmpty, contextObject, errorMessage);
    }

    /// <summary> Asserts that the object is not null using the == operator</summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsNotNull(object aObject, Object contextObject, string errorMessage)
    {
        bool isNull = aObject == null;

        Assert_IsFalse(isNull, contextObject, errorMessage);
    }

    /// <summary> Asserts that the Unity Object is not null using the == operator</summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IsNotNull(Object aObject, Object contextObject, string errorMessage)
    {
        bool isNull = aObject == null;

        Assert_IsFalse(isNull, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given objects are equal using the == operator</summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AreEqual(object aObject, object bObject, Object contextObject, string errorMessage)
    {
        bool areEqual = aObject == bObject;

        Assert_IsTrue(areEqual, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given Unity objects are equal using the == operator</summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AreEqual(Object aObject, Object bObject, Object contextObject, string errorMessage)
    {
        bool areEqual = aObject == bObject;

        Assert_IsTrue(areEqual, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given Unity objects are NOT equal </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AreNotEqual(Object aObject, Object bObject, Object contextObject, string errorMessage)
    {
        if (aObject == bObject || (aObject != null && aObject.Equals(bObject)))
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the given objects are NOT equal </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AreNotEqual(object aObject, object bObject, Object contextObject, string errorMessage)
    {
        if (aObject == bObject || (aObject != null && aObject.Equals(bObject)))
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the GameObject has the required component </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasComponent<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        if (gameObject.GetComponent<T>() == null)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that any children of the GameObject have the required component. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectChildrenHaveComponent<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        if (gameObject.GetComponentInChildren<T>(true) == null)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the GameObject's parent has the required component. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectParentHasComponent<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        if (gameObject.GetComponentsInParent<T>(true) == null)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the GameObject is ACTIVE in the hierarchy </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectIsActive<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        bool isActiveInHierarchy = gameObject.activeInHierarchy;

        Assert_IsTrue(isActiveInHierarchy, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject is INACTIVE in the hierarchy </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectIsInactive<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        bool isActiveInHierarchy = gameObject.activeInHierarchy;

        Assert_IsFalse(isActiveInHierarchy, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject's parent transform is null </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasNoParentTransform<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        if (gameObject.transform.parent != null)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the GameObject's parent transform is not null </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasAnyParentTransform<T>(GameObject gameObject, string errorMessage) where T : Component
    {
        if (gameObject.transform.parent == null)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the GameObject's parent transform is the specified one </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectParentTransform<T>(GameObject gameObject, Transform parentTransform, string errorMessage) where T : Component
    {
        if (gameObject.transform.parent != parentTransform)
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the renderer 'Shared Material' matches the specified one </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_RendererMaterial(Renderer renderer, Material expectedMaterial, string errorMessage)
    {
        if (renderer.sharedMaterial != expectedMaterial)
        {
            LogError(errorMessage, renderer, true);
        }
    }

    /// <summary>Asserts that the GameObject belongs to the specified layer </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectLayer(GameObject gameObject, int layerValue, string errorMessage)
    {
        if (((layerValue & (1 << gameObject.layer)) == 0))
        {
            LogError(errorMessage, gameObject, true);
        }
    }

    /// <summary> Asserts that the GameObject has the specified tag </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectTag(GameObject gameObject, string expectedTag, string errorMessage)
    {
        bool hasTag = gameObject.CompareTag(expectedTag);

        Assert_IsTrue(hasTag, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject has at least one Collider3D attached </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasCollider3D(GameObject gameObject, string errorMessage)
    {
        bool hasValidCollider3D = false;

        foreach (Collider collider3D in gameObject.GetComponents<Collider>())
        {
            if (collider3D.isTrigger == false)
            {
                hasValidCollider3D = true;

                break;
            }
        }

        Assert_IsTrue(hasValidCollider3D, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject has at least one Collider2D attached </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasCollider2D(GameObject gameObject, string errorMessage)
    {
        bool hasValidCollider2D = false;

        foreach (Collider2D collider2D in gameObject.GetComponents<Collider2D>())
        {
            if (collider2D.isTrigger == false)
            {
                hasValidCollider2D = true;

                break;
            }
        }

        Assert_IsTrue(hasValidCollider2D, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject has at least one Trigger3D attached </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasTrigger3D(GameObject gameObject, string errorMessage)
    {
        bool hasValidTrigger3D = false;

        foreach (Collider collider3D in gameObject.GetComponents<Collider>())
        {
            if (collider3D.isTrigger)
            {
                hasValidTrigger3D = true;

                break;
            }
        }

        Assert_IsTrue(hasValidTrigger3D, gameObject, errorMessage);
    }

    /// <summary> Asserts that the GameObject has at least one Trigger2D attached </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_GameObjectHasTrigger2D(GameObject gameObject, string errorMessage)
    {
        bool hasValidTrigger2D = false;

        foreach (Collider2D collider2D in gameObject.GetComponents<Collider2D>())
        {
            if (collider2D.isTrigger)
            {
                hasValidTrigger2D = true;

                break;
            }
        }

        Assert_IsTrue(hasValidTrigger2D, gameObject, errorMessage);
    }

    /// <summary> Asserts that Camera.main is not null </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_MainCameraExists(GameObject contextObject, string errorMessage)
    {
        if (Camera.main == null)
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the object of the specified type is loaded </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_ObjectIsLoaded<T>(Type objectType, GameObject contextObject, string errorMessage) where T : Object
    {
        if (GetObjectOfType<T>() == null)
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that there's at least one directional light loaded in the hierarchy </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_DirectionalLightExists(GameObject contextObject, string errorMessage)
    {
        Light[] lights = GetObjectsOfType<Light>();

        bool hasDirectional = false;

        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                hasDirectional = true;

                break;
            }
        }

        Assert_IsTrue(hasDirectional, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given animator is playing the specified animation clip on the specified layer. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AnimatorIsPlayingAnimation(Animator animator, int layerIndex, int animationHash, string errorMessage)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        bool isPlaying = stateInfo.shortNameHash == animationHash;

        Assert_IsTrue(isPlaying, animator, errorMessage);
    }

    /// <summary> Asserts that the specified boolean parameter value in the given Animator is FALSE. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AnimatorParamIsFalse(Animator animator, int paramHash, string errorMessage)
    {
        Assert_AnimatorHasParameterOfType(animator, paramHash, AnimatorControllerParameterType.Bool, $"Animator does not have a boolean parameter with hash {paramHash}.");

        bool parameterValue = animator.GetBool(paramHash);

        Assert_IsFalse(parameterValue, animator, errorMessage);
    }

    /// <summary> Asserts that the specified boolean parameter value in the given Animator is TRUE. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AnimatorParamIsTrue(Animator animator, int paramHash, string errorMessage)
    {
        Assert_AnimatorHasParameterOfType(animator, paramHash, AnimatorControllerParameterType.Bool, $"Animator does not have a boolean parameter with hash {paramHash}.");

        bool parameterValue = animator.GetBool(paramHash);

        Assert_IsTrue(parameterValue, animator, errorMessage);
    }

    /// <summary> Asserts that the given animator has a parameter of the specified type. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_AnimatorHasParameterOfType(Animator animator, int paramHash, AnimatorControllerParameterType parameterType, string errorMessage)
    {
        foreach (AnimatorControllerParameter animatorParameter in animator.parameters)
        {
            if (animatorParameter.nameHash == paramHash && animatorParameter.type == parameterType)
            {
                return;
            }
        }

        LogError(errorMessage, animator, true);
    }

    /// <summary> Asserts that the given Rigidbody2D body matches the expected type. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_Rigidbody2DBodyType(Rigidbody2D rigidbody, RigidbodyType2D expectedType, string errorMessage)
    {
        bool isExpectedType = rigidbody.bodyType == expectedType;

        Assert_IsTrue(isExpectedType, rigidbody, errorMessage);
    }

    /// <summary> Asserts that the given Rigidbody3D is kinematic. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_Rigidbody3DIsKinematic(Rigidbody rigidbody, string errorMessage)
    {
        bool isKinematic = rigidbody.isKinematic;

        Assert_IsTrue(isKinematic, rigidbody, errorMessage);
    }

    /// <summary> Asserts that the given Rigidbody3D is NOT kinematic. </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_Rigidbody3DIsNotKinematic(Rigidbody rigidbody, string errorMessage)
    {
        bool isKinematic = rigidbody.isKinematic;

        Assert_IsFalse(isKinematic, rigidbody, errorMessage);
    }

    /// <summary> Asserts that two numbers are approximately equal</summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_ApproximatelyEqual(float a, float b, Object contextObject, string errorMessage)
    {
        bool isApproximatelyEqual = Mathf.Approximately(a, b);

        Assert_IsTrue(isApproximatelyEqual, contextObject, errorMessage);
    }

    /// <summary> Asserts that a given collection does not contain null elements </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_CollectionDoesNotContainNullElement(IReadOnlyCollection<object> collection, Object contextObject, string errorMessage)
    {
        foreach (object item in collection)
        {
            if (item == null)
            {
                LogError(errorMessage, contextObject, true);

                break;
            }
        }
    }

    /// <summary> Asserts that the given collection has no repeated items. Also works for value types </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_CollectionAllElementsUnique(ICollection collection, Object contextObject, string errorMessage)
    {
        HashSet<object> uniqueValues = new HashSet<object>();

        foreach (object collectionItem in collection)
        {
            if (uniqueValues.Add(collectionItem) == false)
            {
                LogError(errorMessage, contextObject, true);

                break;
            }
        }
    }

    /// <summary> Asserts that the given collection is not empty (i.e., it contains at least one element) </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_CollectionIsNotEmpty(ICollection collection, Object contextObject, string errorMessage)
    {
        bool isNotEmpty = collection.Count > 0;

        Assert_IsTrue(isNotEmpty, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given collection does not contain the specified element </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_CollectionDoesNotContainElement(IReadOnlyCollection<object> collection, object element, Object contextObject, string errorMessage)
    {
        bool containsElement = collection.Contains(element);

        Assert_IsFalse(containsElement, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given collection contains the specified element </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_CollectionContainsElement(IReadOnlyCollection<object> collection, object element, Object contextObject, string errorMessage)
    {
        bool containsElement = collection.Contains(element);

        Assert_IsTrue(containsElement, contextObject, errorMessage);
    }

    /// <summary> Asserts that the given event action is not null and has at least one listener (excluding anonymous initialization methods). </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_EventActionHasListeners(Action action, Object contextObject, string errorMessage)
    {
        Delegate[] delegates = action.GetInvocationList();

        //From my tests it looks like that "<.ctor>b__" is what the compiler generates for the method name when initializing an event using delegate {};
        //Since we're specting real listeners here, we skip anonymous methods. However, I'm not 100% sure that this is completely reliable. 
        //So far so good..

        if (action == null || (delegates.Length == 1 && delegates[0].Method.Name.ToLower().Contains("<.ctor>b__")))
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the specified index is not less than 0 and not greater than the given collection count </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_IndexWithinCollectionBounds(ICollection collection, int index, Object contextObject, string errorMessage)
    {
        if (index < 0 || index >= collection.Count)
        {
            LogError(errorMessage, contextObject, true);
        }
    }

    /// <summary> Asserts that the given dictionary contains the specified key </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Assert_DictionaryHasKey(IDictionary dictionary, object key, Object contextObject, string errorMessage)
    {
        bool hasKey = dictionary.Contains(key);

        Assert_IsTrue(hasKey, contextObject, errorMessage);
    }

    /// <summary> Displays a GUI window with the message </summary>
    [Conditional(CONDITIONAL), MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void DisplayMessageWindow(string message)
    {
#if DEBUG_HELPER
        RuntimeMessageWindow.Show(message);
#endif
    }

    /// <summary> Displays a GUI window with the message. Used only with exceptions </summary>
    private static void DisplayErrorWindow(string errorMessage)
    {

#if DEBUG_HELPER
        if (Application.isPlaying == false)
        {
            return;
        }

        string customStackTrace = string.Join("\n", new StackTrace(true)
                         .GetFrames()
                         .Select(frame => $"{frame.GetMethod().DeclaringType?.Name}.{frame.GetMethod().Name}(): line {frame.GetFileLineNumber()}"));

        //Empty spaces so that the *StackTrace* message gets centered.. stupid I know...
        string fullMessage = $"{errorMessage}\n\n\n                                           StackTrace:\n\n{customStackTrace}";

        RuntimeExceptionWindow.Show(fullMessage);
#endif
    }

    /// <summary> Helper method for internal use intended to finding objects of the given type </summary>
    internal static T[] GetObjectsOfType<T>() where T : Object
    {
        T[] objects = (T[])Enumerable.Empty<T>();

#if UNITY_2023_1_OR_NEWER
        objects = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        objects = GameObject.FindObjectsOfType<T>(true);
#endif

        return objects;
    }

    /// <summary> Helper method for internal use intended to finding an object of the given type </summary>
    internal static T GetObjectOfType<T>() where T : Object
    {
        T aObject = null;

#if UNITY_2023_1_OR_NEWER
        aObject = GameObject.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
        aObject = GameObject.FindObjectOfType<T>(true);
#endif

        return aObject;
    }

#if DEBUG_HELPER
    ///<summary> Helper class for displaying exceptions into the game screen using GUI </summary>
    private abstract class RuntimeWindow : MonoBehaviour
    {
        protected GUIStyle _opaqueWindowStyle;
        protected GUIStyle _titleStyle;
        protected GUIStyle _textStyle;
        protected GUIStyle _subtitleStyle;

        protected string message;

        protected bool _stylesCached = false;
        protected bool showWindow;
        protected bool dropdownOpen = false;

        protected const int TITLE_FONT_SIZE = 24;
        protected const float HORIZONTAL_MARGIN = 20f;
        protected const float VERTICAL_MARGIN = 20f;

        protected Vector2 _scrollPosition;
        protected const float MESSAGE_BOX_HEIGHT = 300f;

        protected Rect _windowRect = new Rect(0, 0, 600, 400);

        protected static Texture2D _solidTexture;


        //Used to draw different windows. We can be drawing the message window and the exception window, so to avoid conflicts we create new ids for each one of them
        protected abstract int Id { get; }


        protected virtual TextAnchor MessageTextArchor => TextAnchor.UpperLeft;

        protected virtual int MessageFontSize => 18;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        private static void InitializeTexture()
        {
            _solidTexture = new Texture2D(1, 1);
            _solidTexture.SetPixel(0, 0, new Color(0.2f, 0.2f, 0.2f, 1f));
            _solidTexture.Apply();
        }

        private void OnGUI()
        {
            if (!_stylesCached)
            {
                _opaqueWindowStyle = new GUIStyle(GUI.skin.window)
                {
                    normal = { background = _solidTexture },
                    hover = { background = _solidTexture },
                    active = { background = _solidTexture },
                    focused = { background = _solidTexture },
                    onNormal = { background = _solidTexture },
                    onHover = { background = _solidTexture },
                    onActive = { background = _solidTexture },
                    onFocused = { background = _solidTexture }
                };

                _titleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = TITLE_FONT_SIZE,
                    alignment = TextAnchor.MiddleCenter
                };

                _textStyle = new GUIStyle(GUI.skin.textArea)
                {
                    fontSize = MessageFontSize,
                    alignment = MessageTextArchor,
                    wordWrap = true
                };

                _subtitleStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 20,
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };

                _stylesCached = true;
            }

            if (showWindow)
            {
                _windowRect = GUI.Window(Id, _windowRect, DrawWindow, string.Empty, _opaqueWindowStyle);

                ResetWindowIfOutOfBounds();
            }
        }

        protected abstract void DrawWindow(int id);

        private bool IsWindowOutOfBounds()
        {
            return _windowRect.xMax < 0 || _windowRect.yMax < 0 ||
                   _windowRect.xMin > Screen.width || _windowRect.yMin > Screen.height;
        }

        protected void ResetWindowIfOutOfBounds()
        {
            if (IsWindowOutOfBounds())
            {
                _windowRect = new Rect((Screen.width - 600) / 2, (Screen.height - 400) / 2, 600, 400);
            }
        }
    }

    private sealed class RuntimeExceptionWindow : RuntimeWindow
    {
        private static RuntimeExceptionWindow _instance;

        private string[] cachedSceneNames;

        protected override int Id => 0;

        public static void Show(string msg)
        {
            if (_instance == null)
            {
                GameObject go = new GameObject(nameof(RuntimeExceptionWindow));
                go.hideFlags = HideFlags.HideInHierarchy;
                _instance = go.AddComponent<RuntimeExceptionWindow>();
                DontDestroyOnLoad(go);
            }

            if (_instance.showWindow == true)
            {
                _instance._windowRect = new Rect((Screen.width - 600) / 2, (Screen.height - 400) / 2, 600, 400);

                return;
            }

            int sceneCount = SceneManager.sceneCount;
            _instance.cachedSceneNames = new string[sceneCount];
            for (int i = 0; i < sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                _instance.cachedSceneNames[i] = scene.name;
            }

            _instance._windowRect = new Rect((Screen.width - 600) / 2, (Screen.height - 400) / 2, 600, 400);
            _instance.message = msg;
            _instance.showWindow = true;
        }

        protected override void DrawWindow(int id)
        {
            GUILayout.BeginArea(new Rect(HORIZONTAL_MARGIN, VERTICAL_MARGIN,
                                           _windowRect.width - HORIZONTAL_MARGIN * 2,
                                           _windowRect.height - VERTICAL_MARGIN * 2));
            GUILayout.BeginVertical();

            GUILayout.Label("Runtime Exception", _titleStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("     Active scenes:", _subtitleStyle);
            string toggleLabel = dropdownOpen ? "-" : "+";
            if (GUILayout.Button(toggleLabel, GUILayout.Width(30)))
            {
                dropdownOpen = !dropdownOpen;
            }
            GUILayout.EndHorizontal();

            float additionalWindowHeight = 0.0f;

            if (dropdownOpen && cachedSceneNames != null)
            {
                foreach (string sceneName in cachedSceneNames)
                {
                    additionalWindowHeight += 30f;

                    GUILayout.Label(sceneName, _textStyle);
                }
            }

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(MESSAGE_BOX_HEIGHT));
            GUILayout.TextArea(message, _textStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                showWindow = false;
                _stylesCached = false;
                Destroy(gameObject);
            }

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Copy Error Message", GUILayout.Width(150), GUILayout.Height(30)))
            {
                GUIUtility.systemCopyBuffer = message;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndArea();

            _windowRect.height = 480.0f + additionalWindowHeight;

            GUI.DragWindow();
        }
    }

    private sealed class RuntimeMessageWindow : RuntimeWindow
    {
        private static RuntimeMessageWindow _instance;

        protected override int Id => 1;

        protected override TextAnchor MessageTextArchor => TextAnchor.MiddleCenter;

        protected override int MessageFontSize => 23;

        public static void Show(string msg)
        {
            if (Application.isPlaying == false)
            {
                return;
            }
            if (_instance == null)
            {
                GameObject go = new GameObject(nameof(RuntimeMessageWindow));
                go.hideFlags = HideFlags.HideInHierarchy;
                _instance = go.AddComponent<RuntimeMessageWindow>();

                DontDestroyOnLoad(go);

                _instance._windowRect = new Rect((Screen.width - 600) / 2, (Screen.height - 400) / 2, 600, 400);
            }


            _instance.message = msg;

            _instance.showWindow = true;
        }

        protected override void DrawWindow(int id)
        {
            GUILayout.BeginArea(new Rect(HORIZONTAL_MARGIN, VERTICAL_MARGIN,
                                           _windowRect.width - HORIZONTAL_MARGIN * 2,
                                           _windowRect.height - VERTICAL_MARGIN * 2));
            GUILayout.BeginVertical();

            GUILayout.Label("Runtime Message", _titleStyle);

            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Height(200));

            GUILayout.TextArea(message, _textStyle, GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();

            if (GUILayout.Button("Close", GUILayout.Height(30)))
            {
                showWindow = false;
                _stylesCached = false;
                Destroy(gameObject);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();

            _windowRect.height = 310.0f;

            GUI.DragWindow();
        }
    }
#endif

#if UNITY_EDITOR == false && UNITY_WEBGL == false && DEBUG_HELPER
    private readonly static string _logFileName = "RuntimeLogFile.txt";

    private readonly static string filePath;

    private readonly static bool _shouldWriteErrorsToApplicationLogFile = true;

    static DebugHelper()
    {
        filePath = Path.Combine(Application.persistentDataPath, _logFileName);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    private static void InitializeFileLogging()
    {
        if (_shouldWriteErrorsToApplicationLogFile == true)
        {
            File.WriteAllText(filePath, string.Empty + Environment.NewLine);

            Debug.unityLogger.logEnabled = true;

            Application.logMessageReceived += OnLogMessageReceived;
        }
    }

    private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
    {
        if(type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
        {
            string formattedMessage = FormatLogMessage(condition, stackTrace);

            LogToFile(formattedMessage);
        }
    }

    private static string FormatLogMessage(string condition, string stackTrace)
    {
        if (Debug.isDebugBuild)
        {
            string[] stackLines = stackTrace.Split('\n');

            if (stackLines.Length > 1)
            {
                string fileNameLine = stackLines[1].Trim();

                int startIndex = fileNameLine.LastIndexOf("/") + 1;

                int endIndex = fileNameLine.LastIndexOf(":");

                if (startIndex >= 0 && endIndex > startIndex)
                {
                    string fileName = fileNameLine.Substring(startIndex, endIndex - startIndex);

                    string lineNumber = fileNameLine.Substring(endIndex + 1);

                    return $"{fileName} ({lineNumber}): {condition}";
                }
            }

            return $"{condition} {stackTrace}";
        }

        return $"{condition} {stackTrace}";
    }


    [Conditional(CONDITIONAL)]
    public static void LogToFile(string message)
    {
        string logMessage = $"{DateTime.Now}: {message}";

        File.AppendAllText(filePath, logMessage + Environment.NewLine);

        Debug.Log($"Log written to file: {filePath}");
    }
#endif
}
