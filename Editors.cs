#if UNITY_EDITOR

using UnityEditor;
using UnityPackages.EntityComponentSystem;

namespace UnityPackages.EntityComponentSystem {

  public static class Editors {

    [CustomPropertyDrawer (typeof (ECS.Protected))]
    public class ProtectedPropertyDrawer : PropertyDrawer {
      public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
        EditorGUI.BeginDisabledGroup (true);
        EditorGUI.PropertyField (position, serializedProperty, label, true);
        EditorGUI.EndDisabledGroup ();
      }
    }

    [CustomPropertyDrawer (typeof (ECS.Reference))]
    public class ReferencePropertyDrawer : PropertyDrawer {
      public override void OnGUI (UnityEngine.Rect position, SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
        EditorGUI.BeginDisabledGroup (true);
        EditorGUI.PropertyField (position, serializedProperty, label, true);
        EditorGUI.EndDisabledGroup ();
      }
    }
  }
}

#endif