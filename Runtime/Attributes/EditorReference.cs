namespace UnityPackages.EntityComponentSystem {
  // TODO move this into a normal attribute and find the references to runtime!?
  /// Describes a reference property within a component.
  public class EditorReference : UnityEngine.PropertyAttribute { }

#if UNITY_EDITOR
  [UnityEditor.CustomPropertyDrawer (typeof (EditorReference))]
  public class EditorReferenceDrawer : UnityEditor.PropertyDrawer {
    public override void OnGUI (UnityEngine.Rect position, UnityEditor.SerializedProperty serializedProperty, UnityEngine.GUIContent label) {
      label.text = "@ " + serializedProperty.displayName;
      label.tooltip =
        "This is a managed reference to '" + serializedProperty.type + " " + serializedProperty.name + "', " +
        "make sure a gameObject with a name like this is a child of this gameObject.";

      UnityEditor.EditorGUI.BeginDisabledGroup (true);
      UnityEditor.EditorGUI.PropertyField (position, serializedProperty, label, false);
      UnityEditor.EditorGUI.EndDisabledGroup ();

      if (UnityEngine.Application.isPlaying == true)
        return;

      var _flattenObjectNameRegex = new System.Text.RegularExpressions.Regex ("[^a-zA-Z0-9]");
      var _matchComponentNameRegex = new System.Text.RegularExpressions.Regex (@"PPtr<\$(.*?)>");
      var _selfReference = (EditorReference) attribute;
      var _selfTransform = ((UnityEngine.MonoBehaviour) serializedProperty.serializedObject.targetObject).transform;
      var _childTransforms = _selfTransform.GetComponentsInChildren<UnityEngine.Transform> ();
      var _targetGameObjectName = _flattenObjectNameRegex.Replace (serializedProperty.name, "").ToLower ().Trim ();
      var _targetTypeName = _matchComponentNameRegex.Match (serializedProperty.type).Groups[1].Value;

      // Clear the object reference
      serializedProperty.objectReferenceValue = null;

      // Loop all the children
      foreach (var _childTransform in _childTransforms) {
        var _childGameObjectName = _flattenObjectNameRegex.Replace (_childTransform.gameObject.name, "").ToLower ().Trim ();

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
#endif
}