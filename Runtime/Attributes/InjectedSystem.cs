namespace UnityPackages.EntityComponentSystem {

  /// Describes a injectedable property within a system.
  public class InjectedSystem : System.Attribute {

    /// Sets the attributes values on an object.
    public static void SetAttributeValues (System.Object target) {
      var _fields = target.GetType ().GetFields (
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Public);
      foreach (var _field in _fields)
        if (System.Attribute.GetCustomAttribute (_field, typeof (InjectedSystem)) != null)
          _field.SetValue (target, Controller.Instance.GetSystem (_field.FieldType));
    }
  }
}
