namespace ElRaccoone.EntityComponentSystem {
  /// <summary>
  /// Describes a injectedable system, service or controller.
  /// </summary>
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Injected : System.Attribute {
    /// <summary>
    /// Type of an entity system.
    /// </summary>
    static readonly System.Type entitySystemType = typeof (IEntitySystem);

    /// <summary>
    /// Type of a service.
    /// </summary>
    static readonly System.Type serviceType = typeof (IService);

    /// <summary>
    /// Type of a controller.
    /// </summary>
    static readonly System.Type controllerType = typeof (IController);

    /// <summary>
    /// Sets the attributes values on an object.
    /// </summary>
    /// <param name="target">The target object.</param>
    public static void SetAttributeValues (System.Object target) {
      var targetType = target.GetType ();
      var fields = targetType.GetFields (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      foreach (var field in fields) {
        if (System.Attribute.GetCustomAttribute (field, typeof (Injected)) == null) {
          continue;
        }
        var fieldInterfaces = field.FieldType.GetInterfaces ();
        foreach (var fieldInterface in fieldInterfaces) {
          if (fieldInterface == entitySystemType) {
            field.SetValue (target, Controller.Instance.GetSystem (field.FieldType));
          } else if (fieldInterface == serviceType) {
            field.SetValue (target, Controller.Instance.GetService (field.FieldType));
          } else if (fieldInterface == controllerType) {
            field.SetValue (target, Controller.Instance);
          }
        }
      }
    }
  }
}