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

        var _selfTransform = ((MonoBehaviour) serializedProperty.serializedObject.targetObject).transform;
        var _childTransforms = _selfTransform.GetComponentsInChildren<Transform> ();
        var _targetGameObjectName = flattenObjectNameRegex.Replace (serializedProperty.name, "").ToLower ().Trim ();
        var _targetTypeName = matchComponentNameRegex.Match (serializedProperty.type).Groups[1].Value;

        foreach (var _childTransform in _childTransforms) {
          var _childGameObjectName = flattenObjectNameRegex.Replace (_childTransform.gameObject.name, "").ToLower ().Trim ();
          if (_childGameObjectName == _targetGameObjectName)
            switch (_targetTypeName) {
              case "GameObject":
                serializedProperty.objectReferenceValue = _childTransform.gameObject;
                return;
              case "Transform":
                serializedProperty.objectReferenceValue = _childTransform;
                return;
              default:
                serializedProperty.objectReferenceValue = _childTransform.GetComponent (_targetTypeName);
                return;
            }
        }
        serializedProperty.objectReferenceValue = null;
      }
    }
  }
}

#endif