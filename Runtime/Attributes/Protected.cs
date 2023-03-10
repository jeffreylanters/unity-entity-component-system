namespace ElRaccoone.EntityComponentSystem {

  /// Describes a protected property within a component.
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Protected : UnityEngine.PropertyAttribute { }

#if UNITY_EDITOR && ECS_DEFINED_COM_UNITY_UGUI
  [UnityEditor.CustomPropertyDrawer (typeof (Protected))]
  public class ProtectedDrawer : UnityEditor.PropertyDrawer {
    public override void OnGUI (UnityEngine.Rect position, UnityEditor.SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
      var wasEnabled = UnityEngine.GUI.enabled;
      UnityEngine.GUI.enabled = false;
      UnityEditor.EditorGUI.PropertyField (position, serializedProperty, label);
      UnityEngine.GUI.enabled = wasEnabled;
    }
  }
#endif
}