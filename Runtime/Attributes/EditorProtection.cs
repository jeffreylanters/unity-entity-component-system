namespace UnityPackages.EntityComponentSystem {

  /// Describes a protected property within a component.
  public class EditorProtection : UnityEngine.PropertyAttribute { }

#if UNITY_EDITOR
  [UnityEditor.CustomPropertyDrawer (typeof (EditorProtection))]
  public class EditorProtectionDrawer : UnityEditor.PropertyDrawer {
    public override void OnGUI (UnityEngine.Rect position, UnityEditor.SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
      label.tooltip =
        "This is a managed protection to '" + serializedProperty.type + " " + serializedProperty.name + "', " +
        "only a system should modify this public property's value.";
      UnityEditor.EditorGUI.BeginDisabledGroup (true);
      UnityEditor.EditorGUI.PropertyField (position, serializedProperty, label, false);
      UnityEditor.EditorGUI.EndDisabledGroup ();
    }
  }
#endif
}