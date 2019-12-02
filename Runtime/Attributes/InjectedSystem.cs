namespace UnityPackages.EntityComponentSystem {

  /// Describes a injectedable property within a system.
  public class InjectedSystem : System.Attribute {

    /// Sets the attributes values on a system.
    public static void SetAttributeValues (ISystem system) {
      var _entitySystemFields = system.GetType ().GetFields (
        System.Reflection.BindingFlags.Instance |
        System.Reflection.BindingFlags.NonPublic |
        System.Reflection.BindingFlags.Public);
      foreach (var _entitySystemField in _entitySystemFields)
        if (System.Attribute.GetCustomAttribute (_entitySystemField, typeof (Inject)) != null)
          _entitySystemField.SetValue (system, Controller.Instance.GetSystem (_entitySystemField.FieldType));
    }
  }
}