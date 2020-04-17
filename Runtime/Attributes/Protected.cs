namespace UnityPackages.EntityComponentSystem {

  /// Describes a protected property within a component.
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Protected : UnityEngine.PropertyAttribute { }

#if UNITY_EDITOR
  [UnityEditor.CustomPropertyDrawer (typeof (Protected))]
  public class ProtectedDrawer : UnityEditor.PropertyDrawer {
    public override void OnGUI (UnityEngine.Rect position, UnityEditor.SerializedProperty serializedProperty, UnityEngine.GUIContent label) { }

    public override float GetPropertyHeight (UnityEditor.SerializedProperty property, UnityEngine.GUIContent label) {
      // https://forum.unity.com/threads/getpropertyheight-and-arrays.443235/
      return -1;
    }
  }
#endif
}
