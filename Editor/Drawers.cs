#if UNITY_EDITOR

using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityPackages.EntityComponentSystem;

namespace UnityPackages.EntityComponentSystem {

  public static class Editors {

    private static Regex flattenObjectNameRegex = new Regex ("[^a-zA-Z0-9]");
    private static Regex matchComponentNameRegex = new Regex (@"PPtr<\$(.*?)>");

    [CustomPropertyDrawer (typeof (ECS.Protected))]
    public class ProtectedPropertyDrawer : PropertyDrawer {
      public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
        label.tooltip =
          "This is a managed protection to '" + serializedProperty.type + " " + serializedProperty.name + "', " +
          "only a system should modify this public property's value.";

        EditorGUI.BeginDisabledGroup (true);
        EditorGUI.PropertyField (position, serializedProperty, label, false);
        EditorGUI.EndDisabledGroup ();
      }
    }

    [CustomPropertyDrawer (typeof (ECS.Reference))]
    public class ReferencePropertyDrawer : PropertyDrawer {
      public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
        label.text = "@ " + serializedProperty.displayName;
        label.tooltip =
          "This is a managed reference to '" + serializedProperty.type + " " + serializedProperty.name + "', " +
          "make sure a gameObject with a name like this is a child of this gameObject.";

        EditorGUI.BeginDisabledGroup (true);
        EditorGUI.PropertyField (position, serializedProperty, label, false);
        EditorGUI.EndDisabledGroup ();

        if (Application.isPlaying == true)
          return;

        var _selfReference = (ECS.Reference) attribute;
        var _selfTransform = ((MonoBehaviour) serializedProperty.serializedObject.targetObject).transform;
        var _childTransforms = _selfTransform.GetComponentsInChildren<Transform> ();
        var _targetGameObjectName = flattenObjectNameRegex.Replace (serializedProperty.name, "").ToLower ().Trim ();
        var _targetTypeName = matchComponentNameRegex.Match (serializedProperty.type).Groups[1].Value;

        // Clear the object reference
        if (serializedProperty.isArray == true) {
          serializedProperty.ClearArray ();
          //! PROBLEM: it seems like we don't get a reference to the 
          //!   array it self, but only to it's children...
          //!   therefore we can't munipulate the array
        } else if (serializedProperty.isArray == false) {
          serializedProperty.objectReferenceValue = null;
        }

        // Loop all the children
        foreach (var _childTransform in _childTransforms) {
          var _childGameObjectName = flattenObjectNameRegex.Replace (_childTransform.gameObject.name, "").ToLower ().Trim ();

          // Match if we're looking at the right object
          if (_childGameObjectName != _targetGameObjectName)
            continue;

          // Get the object reference
          if (_targetTypeName == "GameObject")
            serializedProperty.objectReferenceValue = _childTransform.gameObject;
          else if (_targetTypeName == "Transform")
            serializedProperty.objectReferenceValue = _childTransform;
          else // Just try to get the component
            serializedProperty.objectReferenceValue = _childTransform.GetComponent (_targetTypeName);

          // Our job is done :)
          return;
        }
      }
    }
  }
}

#endif