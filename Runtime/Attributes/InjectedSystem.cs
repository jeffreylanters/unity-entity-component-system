namespace UnityPackages.EntityComponentSystem {

  /// Describes a injectedable property within a system.
  public class InjectedSystem : System.Attribute {

    /// Sets the attributes values on a system.
    public static void SetAttributeValues (IEntitySystem system) {
      var _systemFields = system.GetType ().GetFields (
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Public);
      foreach (var _systemField in _systemFields)
        if (System.Attribute.GetCustomAttribute (_systemField, typeof (InjectedSystem)) != null)
          _systemField.SetValue (system, Controller.Instance.GetSystem (_systemField.FieldType));
    }
  }
}