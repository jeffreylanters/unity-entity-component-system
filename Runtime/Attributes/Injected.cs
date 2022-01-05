namespace ElRaccoone.EntityComponentSystem {

  /// <summary>
  /// Describes a injectedable system, service or controller.
  /// </summary>
  [System.AttributeUsage (System.AttributeTargets.Field)]
  public class Injected : System.Attribute {

    /// <summary>
    /// Sets the attributes values on an object using reflection.
    /// </summary>
    /// <param name="target">The target object.</param>
    internal static void SetAttributeValues (System.Object target) {
      // Finds all fields with the Asset attribute.
      var type = target.GetType ();
      var fields = type.GetFields (System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
      foreach (var field in fields) {
        // Getting the field attribute where the type is Injected.
        if (System.Attribute.GetCustomAttribute (field, typeof (Injected)) != null) {
          // For each field we'll check if it is assignable from any of the 
          // following classes and assign the field to any of these references.
          // TODO -- This is not workign yet!!!!
          if (field.FieldType.IsAssignableFrom (typeof (EntitySystem)) == true) {
            field.SetValue (target, Controller.Instance.GetSystem (field.FieldType));
          } else if (field.FieldType.IsAssignableFrom (typeof (Service)) == true) {
            field.SetValue (target, Controller.Instance.GetService (field.FieldType));
          } else if (field.FieldType.IsAssignableFrom (typeof (Controller)) == true) {
            field.SetValue (target, Controller.Instance);
          }
        }
      }
    }
  }
}